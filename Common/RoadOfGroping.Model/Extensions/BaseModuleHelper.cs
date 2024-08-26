using System.Reflection;
using RoadOfGroping.Interface.Reflection;
using RoadOfGroping.Model.Interface;
using RoadOfGroping.Model.Modules;

namespace RoadOfGroping.Model.Extensions
{
    internal static class BaseModuleHelper
    {
        /// <summary>
        /// 加载模块
        /// </summary>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        public static List<Type> LoadModules(Type moduleType)
        {
            BaseModule.CheckModuleType(moduleType);
            var moduleTypes = new List<Type>();
            GetDependsAllModuleType(moduleType, moduleTypes);
            return moduleTypes;
        }

        //public static Assembly[] GetAllAssemblies(Type moduleType)
        //{
        //    var assemblies = new List<Assembly>();

        //    var additionalAssemblyDescriptors = moduleType
        //        .GetCustomAttributes()
        //        .OfType<IAdditionalModuleAssemblyProvider>();

        //    foreach (var descriptor in additionalAssemblyDescriptors)
        //    {
        //        foreach (var assembly in descriptor.GetAssemblies())
        //        {
        //            assemblies.AddIfNotContains(assembly);
        //        }
        //    }

        //    assemblies.Add(moduleType.Assembly);

        //    return assemblies.ToArray();
        //}

        public static void GetDependsAllModuleType(Type moduleType, List<Type> moduleTypes)
        {
            BaseModule.CheckModuleType(moduleType);
            if (moduleTypes.Contains(moduleType))
            {
                return;
            }

            moduleTypes.Add(moduleType);

            foreach (var dependModule in DependModuleTypes(moduleType))
            {
                GetDependsAllModuleType(dependModule, moduleTypes);
            }
        }

        public static List<Type> DependModuleTypes(Type moduleType)
        {
            BaseModule.CheckModuleType(moduleType);

            var dependencies = new List<Type>();

            var dependencyDescriptors = moduleType
                .GetCustomAttributes()
                .OfType<IDependsAttrProvider>();

            foreach (var descriptor in dependencyDescriptors)
            {
                foreach (var dependedModuleType in descriptor.GetDependsModulesType())
                {
                    dependencies.AddIfNotContains(dependedModuleType);
                }
            }

            return dependencies;
        }
    }
}