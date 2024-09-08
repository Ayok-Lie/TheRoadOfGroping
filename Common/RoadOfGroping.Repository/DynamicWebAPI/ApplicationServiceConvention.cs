using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Repository.DynamicWebAPI
{
    /// <summary>
    /// 自定义应用程序模型约定，用于配置实现了 IApplicationService 接口的控制器。
    /// </summary>
    public class ApplicationServiceConvention : IApplicationModelConvention
    {
        /// <summary>
        /// 应用应用程序模型约定。
        /// </summary>
        /// <param name="application">应用程序模型。</param>
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                // 检查控制器是否实现了 IApplicationService 接口
                if (typeof(IApplicationService).IsAssignableFrom(controller.ControllerType))
                {
                    ConfigureApplicationService(controller);
                }
            }
        }

        /// <summary>
        /// 配置实现了 IApplicationService 接口的控制器。
        /// </summary>
        /// <param name="controller">控制器模型。</param>
        private void ConfigureApplicationService(ControllerModel controller)
        {
            ConfigureApiExplorer(controller);
            ConfigureSelector(controller);
            ConfigureParameters(controller);
            //ConfigureTags(controller);
        }

        /// <summary>
        /// 配置 API 资源管理器。
        /// </summary>
        /// <param name="controller">控制器模型。</param>
        private void ConfigureApiExplorer(ControllerModel controller)
        {
            // 设置控制器的 API 可见性
            if (!controller.ApiExplorer.IsVisible.HasValue)
            {
                controller.ApiExplorer.IsVisible = true;
            }

            // 设置控制器中所有动作的 API 可见性
            foreach (var action in controller.Actions)
            {
                if (!action.ApiExplorer.IsVisible.HasValue)
                {
                    action.ApiExplorer.IsVisible = true;
                }
            }
        }

        /// <summary>
        /// 配置选择器。
        /// </summary>
        /// <param name="controller">控制器模型。</param>
        private void ConfigureSelector(ControllerModel controller)
        {
            // 移除空的选择器
            RemoveEmptySelectors(controller.Selectors);

            // 如果控制器已经有选择器，则不进行配置
            if (controller.Selectors.Any(temp => temp.AttributeRouteModel != null))
            {
                return;
            }

            // 配置控制器中所有动作的选择器
            foreach (var action in controller.Actions)
            {
                ConfigureSelector(action);
            }
        }

        /// <summary>
        /// 配置动作的选择器。
        /// </summary>
        /// <param name="action">动作模型。</param>
        private void ConfigureSelector(ActionModel action)
        {
            // 移除空的选择器
            RemoveEmptySelectors(action.Selectors);

            // 如果动作没有选择器，则添加新的选择器
            if (action.Selectors.Count <= 0)
            {
                AddApplicationServiceSelector(action);
            }
            else
            {
                // 规范化选择器路由
                NormalizeSelectorRoutes(action);
            }
        }

        /// <summary>
        /// 配置参数。
        /// </summary>
        /// <param name="controller">控制器模型。</param>
        private void ConfigureParameters(ControllerModel controller)
        {
            foreach (var action in controller.Actions)
            {
                foreach (var parameter in action.Parameters)
                {
                    // 如果参数已经有绑定信息，则跳过
                    if (parameter.BindingInfo != null)
                    {
                        continue;
                    }

                    // 如果参数类型是类且不是字符串或 IFormFile，则设置绑定信息
                    if (parameter.ParameterType.IsClass &&
                        parameter.ParameterType != typeof(string) &&
                        parameter.ParameterType != typeof(IFormFile))
                    {
                        var httpMethods = action.Selectors.SelectMany(temp => temp.ActionConstraints).OfType<HttpMethodActionConstraint>().SelectMany(temp => temp.HttpMethods).ToList();
                        if (httpMethods.Contains("GET") ||
                            httpMethods.Contains("DELETE") ||
                            httpMethods.Contains("TRACE") ||
                            httpMethods.Contains("HEAD"))
                        {
                            continue;
                        }

                        parameter.BindingInfo = BindingInfo.GetBindingInfo(new[] { new FromBodyAttribute() });
                    }
                }
            }
        }

        /// <summary>
        /// 规范化选择器路由。
        /// </summary>
        /// <param name="action">动作模型。</param>
        private void NormalizeSelectorRoutes(ActionModel action)
        {
            foreach (var selector in action.Selectors)
            {
                // 如果选择器没有路由模型，则添加新的路由模型
                if (selector.AttributeRouteModel == null)
                {
                    selector.AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(CalculateRouteTemplate(action)));
                }

                // 如果选择器没有 HTTP 方法约束，则添加新的 HTTP 方法约束
                if (selector.ActionConstraints.OfType<HttpMethodActionConstraint>().FirstOrDefault()?.HttpMethods?.FirstOrDefault() == null)
                {
                    selector.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { GetHttpMethod(action) }));
                }
            }
        }

        /// <summary>
        /// 添加应用程序服务选择器。
        /// </summary>
        /// <param name="action">动作模型。</param>
        private void AddApplicationServiceSelector(ActionModel action)
        {
            var selector = new SelectorModel();
            selector.AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(CalculateRouteTemplate(action)));
            selector.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { GetHttpMethod(action) }));

            action.Selectors.Add(selector);
        }

        /// <summary>
        /// 去除应用程序服务后缀。
        /// </summary>
        /// <param name="controller"></param>
        private void ConfigureTags(ControllerModel controller)
        {
            var controllerName = controller.ControllerName;
            if (controllerName.EndsWith("AppService", StringComparison.OrdinalIgnoreCase))
            {
                controllerName = controllerName.Substring(0, controllerName.Length - "AppService".Length);
            }

            foreach (var action in controller.Actions)
            {
                foreach (var selector in action.Selectors)
                {
                    selector.EndpointMetadata.Add(new TagsAttribute(controllerName));
                }
            }
        }

        /// <summary>
        /// 计算路由模板。
        /// </summary>
        /// <param name="action">动作模型。</param>
        /// <returns>路由模板字符串。</returns>
        private string CalculateRouteTemplate(ActionModel action)
        {
            var routeTemplate = new StringBuilder();
            routeTemplate.Append("api");

            // 控制器名称部分
            var controllerName = action.Controller.ControllerName;
            if (controllerName.EndsWith("ApplicationService"))
            {
                controllerName = controllerName.Substring(0, controllerName.Length - "ApplicationService".Length);
            }
            else if (controllerName.EndsWith("AppService"))
            {
                controllerName = controllerName.Substring(0, controllerName.Length - "AppService".Length);
            }
            controllerName += "s";
            routeTemplate.Append($"/{controllerName}");

            // Action 名称部分
            var actionName = action.ActionName;
            if (actionName.EndsWith("Async"))
            {
                actionName = actionName.Substring(0, actionName.Length - "Async".Length);
            }
            if (!string.IsNullOrEmpty(actionName))
            {
                routeTemplate.Append($"/{actionName}");
            }

            return routeTemplate.ToString();
        }

        /// <summary>
        /// 获取 HTTP 方法。
        /// </summary>
        /// <param name="action">动作模型。</param>
        /// <returns>HTTP 方法字符串。</returns>
        private string GetHttpMethod(ActionModel action)
        {
            var actionName = action.ActionName;
            if (actionName.StartsWith("Get"))
            {
                return "GET";
            }

            if (actionName.StartsWith("Put") || actionName.StartsWith("UpdateAsync"))
            {
                return "PUT";
            }

            if (actionName.StartsWith("DeleteAsync") || actionName.StartsWith("Remove"))
            {
                return "DELETE";
            }

            if (actionName.StartsWith("Patch"))
            {
                return "PATCH";
            }

            return "POST";
        }

        /// <summary>
        /// 移除空的选择器。
        /// </summary>
        /// <param name="selectors">选择器列表。</param>
        private void RemoveEmptySelectors(IList<SelectorModel> selectors)
        {
            for (var i = selectors.Count - 1; i >= 0; i--)
            {
                var selector = selectors[i];
                if (selector.AttributeRouteModel == null &&
                    (selector.ActionConstraints == null || selector.ActionConstraints.Count <= 0) &&
                    (selector.EndpointMetadata == null || selector.EndpointMetadata.Count <= 0))
                {
                    selectors.Remove(selector);
                }
            }
        }
    }
}




//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Xml.Linq;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.ApplicationModels;
//using Microsoft.AspNetCore.Mvc.ModelBinding;

//namespace RoadOfGroping.Repository.DynamicWebAPI
//{
//    public class ApplicationServiceConvention : IApplicationModelConvention
//    {
//        private readonly XDocument _xmlDocumentation;

//        public ApplicationServiceConvention()
//        {
//            // 加载 XML 文档文件
//            var assembly = Assembly.GetExecutingAssembly();
//            var assemblyName = assembly.GetName().Name;
//            var xmlFilePath = Path.Combine(AppContext.BaseDirectory, $"{assemblyName}.xml");

//            if (File.Exists(xmlFilePath))
//            {
//                _xmlDocumentation = XDocument.Load(xmlFilePath);
//            }
//        }

//        public void Apply(ApplicationModel application)
//        {
//            foreach (var controller in application.Controllers)
//            {
//                if (typeof(IApplicationService).IsAssignableFrom(controller.ControllerType))
//                {
//                    ConfigureApplicationService(controller);
//                }
//            }
//        }

//        private void ConfigureApplicationService(ControllerModel controller)
//        {
//            ConfigureApiExplorer(controller);
//            ConfigureSelector(controller);
//            ConfigureParameters(controller);
//            ConfigureTags(controller);
//            ConfigureDocumentation(controller);
//        }

//        private void ConfigureDocumentation(ControllerModel controller)
//        {
//            foreach (var action in controller.Actions)
//            {
//                var methodInfo = action.ActionMethod;
//                var methodDocumentation = GetMethodDocumentation(methodInfo);

//                if (!string.IsNullOrEmpty(methodDocumentation))
//                {
//                    action.ApiExplorer.Documentation = methodDocumentation;
//                }
//            }
//        }

//        private string GetMethodDocumentation(MethodInfo methodInfo)
//        {
//            if (_xmlDocumentation == null)
//            {
//                return null;
//            }

//            var methodName = $"{methodInfo.DeclaringType.FullName}.{methodInfo.Name}";
//            var methodElement = _xmlDocumentation.Descendants("member")
//                .FirstOrDefault(m => m.Attribute("name").Value == $"M:{methodName}");

//            if (methodElement != null)
//            {
//                var summaryElement = methodElement.Element("summary");
//                return summaryElement?.Value.Trim();
//            }

//            return null;
//        }

//        private void ConfigureApiExplorer(ControllerModel controller)
//        {
//            if (!controller.ApiExplorer.IsVisible.HasValue)
//            {
//                controller.ApiExplorer.IsVisible = true;
//            }

//            foreach (var action in controller.Actions)
//            {
//                if (!action.ApiExplorer.IsVisible.HasValue)
//                {
//                    action.ApiExplorer.IsVisible = true;
//                }
//            }
//        }

//        private void ConfigureSelector(ControllerModel controller)
//        {
//            RemoveEmptySelectors(controller.Selectors);

//            if (controller.Selectors.Any(temp => temp.AttributeRouteModel != null))
//            {
//                return;
//            }

//            foreach (var action in controller.Actions)
//            {
//                ConfigureSelector(action);
//            }
//        }

//        private void ConfigureSelector(ActionModel action)
//        {
//            RemoveEmptySelectors(action.Selectors);

//            if (action.Selectors.Count <= 0)
//            {
//                AddApplicationServiceSelector(action);
//            }
//            else
//            {
//                NormalizeSelectorRoutes(action);
//            }
//        }

//        private void ConfigureParameters(ControllerModel controller)
//        {
//            foreach (var action in controller.Actions)
//            {
//                foreach (var parameter in action.Parameters)
//                {
//                    if (parameter.BindingInfo != null)
//                    {
//                        continue;
//                    }

//                    if (parameter.ParameterType.IsClass &&
//                        parameter.ParameterType != typeof(string) &&
//                        parameter.ParameterType != typeof(IFormFile))
//                    {
//                        var httpMethods = action.Selectors.SelectMany(temp => temp.ActionConstraints).OfType<HttpMethodActionConstraint>().SelectMany(temp => temp.HttpMethods).ToList();
//                        if (httpMethods.Contains("GET") ||
//                            httpMethods.Contains("DELETE") ||
//                            httpMethods.Contains("TRACE") ||
//                            httpMethods.Contains("HEAD"))
//                        {
//                            continue;
//                        }

//                        parameter.BindingInfo = BindingInfo.GetBindingInfo(new[] { new FromBodyAttribute() });
//                    }
//                }
//            }
//        }

//        private void NormalizeSelectorRoutes(ActionModel action)
//        {
//            foreach (var selector in action.Selectors)
//            {
//                if (selector.AttributeRouteModel == null)
//                {
//                    selector.AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(CalculateRouteTemplate(action)));
//                }

//                if (selector.ActionConstraints.OfType<HttpMethodActionConstraint>().FirstOrDefault()?.HttpMethods?.FirstOrDefault() == null)
//                {
//                    selector.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { GetHttpMethod(action) }));
//                }
//            }
//        }

//        private void AddApplicationServiceSelector(ActionModel action)
//        {
//            var selector = new SelectorModel();
//            selector.AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(CalculateRouteTemplate(action)));
//            selector.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { GetHttpMethod(action) }));

//            action.Selectors.Add(selector);
//        }

//        private void ConfigureTags(ControllerModel controller)
//        {
//            var controllerName = controller.ControllerName;
//            if (controllerName.EndsWith("AppService", StringComparison.OrdinalIgnoreCase))
//            {
//                controllerName = controllerName.Substring(0, controllerName.Length - "AppService".Length);
//            }

//            foreach (var action in controller.Actions)
//            {
//                foreach (var selector in action.Selectors)
//                {
//                    selector.EndpointMetadata.Add(new TagsAttribute(controllerName));
//                }
//            }
//        }

//        private string CalculateRouteTemplate(ActionModel action)
//        {
//            var routeTemplate = new StringBuilder();
//            routeTemplate.Append("api");

//            var controllerName = action.Controller.ControllerName;
//            if (controllerName.EndsWith("ApplicationService"))
//            {
//                controllerName = controllerName.Substring(0, controllerName.Length - "ApplicationService".Length);
//            }
//            else if (controllerName.EndsWith("AppService"))
//            {
//                controllerName = controllerName.Substring(0, controllerName.Length - "AppService".Length);
//            }
//            controllerName += "s";
//            routeTemplate.Append($"/{controllerName}");

//            var actionName = action.ActionName;
//            if (actionName.EndsWith("Async"))
//            {
//                actionName = actionName.Substring(0, actionName.Length - "Async".Length);
//            }
//            if (!string.IsNullOrEmpty(actionName))
//            {
//                routeTemplate.Append($"/{actionName}");
//            }

//            return routeTemplate.ToString();
//        }

//        private string GetHttpMethod(ActionModel action)
//        {
//            var actionName = action.ActionName;
//            if (actionName.StartsWith("Get"))
//            {
//                return "GET";
//            }

//            if (actionName.StartsWith("Put") || actionName.StartsWith("UpdateAsync"))
//            {
//                return "PUT";
//            }

//            if (actionName.StartsWith("DeleteAsync") || actionName.StartsWith("Remove"))
//            {
//                return "DELETE";
//            }

//            if (actionName.StartsWith("Patch"))
//            {
//                return "PATCH";
//            }

//            return "POST";
//        }

//        private void RemoveEmptySelectors(IList<SelectorModel> selectors)
//        {
//            for (var i = selectors.Count - 1; i >= 0; i--)
//            {
//                var selector = selectors[i];
//                if (selector.AttributeRouteModel == null &&
//                    (selector.ActionConstraints == null || selector.ActionConstraints.Count <= 0) &&
//                    (selector.EndpointMetadata == null || selector.EndpointMetadata.Count <= 0))
//                {
//                    selectors.Remove(selector);
//                }
//            }
//        }
//    }
//}
