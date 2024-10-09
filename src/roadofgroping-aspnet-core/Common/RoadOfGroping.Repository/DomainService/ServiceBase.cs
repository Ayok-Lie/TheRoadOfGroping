using System.Globalization;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using RoadOfGroping.Common.Localization;
using RoadOfGroping.Repository.UserSession;

namespace RoadOfGroping.Repository.DomainService
{
    /// <summary>
    /// 基础服务类，提供通用的功能供具体服务类继承
    /// </summary>
    public class ServiceBase
    {
        #region Properties

        /// <summary>
        /// 用户信息服务，通过此属性可以访问当前用户会话的信息
        /// </summary>
        public IUserSession UserService { get; private set; }

        /// <summary>
        /// 对象映射器，用于在不同对象之间进行映射转换
        /// </summary>
        public IMapper ObjectMapper { get; private set; }

        public ILocalizationManager LocalizationManager { get; private set; }

        private ILocalizationSource _localizationSource;
        protected string LocalizationSourceName { get; set; }

        protected ILocalizationSource LocalizationSource
        {
            get
            {
                if (LocalizationSourceName == null)
                {
                    throw new Exception("必须在获取 LocalizationSource 之前设置 LocalizationSourceName。");
                }

                if (_localizationSource == null || _localizationSource.Name != LocalizationSourceName)
                {
                    _localizationSource = LocalizationManager.GetSource(LocalizationSourceName);
                }

                return _localizationSource;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// 构造函数，通过依赖注入的服务提供者获取所需的服务
        /// </summary>
        /// <param name="serviceProvider">服务提供者，用于解析依赖项</param>
        /// <param name="localizationSourceName">本地化源的名称（可选）</param>
        public ServiceBase(IServiceProvider serviceProvider, string localizationSourceName = null)
        {
            // 从服务提供者中获取所需服务
            ObjectMapper = serviceProvider.GetRequiredService<IMapper>();
            UserService = serviceProvider.GetRequiredService<IUserSession>();
            LocalizationManager = serviceProvider.GetRequiredService<ILocalizationManager>();

            // 设置本地化源名称
            LocalizationSourceName = localizationSourceName;
        }

        #endregion

        #region Localization Methods

        /// <summary>
        /// 通过字符串名称获取本地化字符串
        /// </summary>
        /// <param name="name">本地化字符串的名称</param>
        /// <returns>对应的本地化字符串</returns>
        protected virtual string L(string name)
        {
            return LocalizationSource.GetString(name);
        }

        /// <summary>
        /// 通过字符串名称和参数获取格式化的本地化字符串
        /// </summary>
        /// <param name="name">本地化字符串的名称</param>
        /// <param name="args">用于格式化的参数</param>
        /// <returns>格式化后的本地化字符串</returns>
        protected virtual string L(string name, params object[] args)
        {
            return LocalizationSource.GetString(name, args);
        }

        /// <summary>
        /// 通过字符串名称和文化信息获取本地化字符串
        /// </summary>
        /// <param name="name">本地化字符串的名称</param>
        /// <param name="cultureInfo">指定的文化信息</param>
        /// <returns>对应的本地化字符串</returns>
        protected virtual string L(string name, CultureInfo cultureInfo)
        {
            return LocalizationSource.GetString(name, cultureInfo);
        }

        /// <summary>
        /// 通过字符串名称、文化信息和参数获取格式化的本地化字符串
        /// </summary>
        /// <param name="name">本地化字符串的名称</param>
        /// <param name="cultureInfo">指定的文化信息</param>
        /// <param name="args">用于格式化的参数</param>
        /// <returns>格式化后的本地化字符串</returns>
        protected virtual string L(string name, CultureInfo cultureInfo, params object[] args)
        {
            return LocalizationSource.GetString(name, cultureInfo, args);
        }


        #endregion
    }
}
