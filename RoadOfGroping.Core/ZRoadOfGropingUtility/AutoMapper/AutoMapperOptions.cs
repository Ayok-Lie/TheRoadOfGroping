using AutoMapper;
using RoadOfGroping.Common.Collections;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.AutoMapper
{
    public class AutoMapperOptions
    {
        public List<Action<IAutoMapperConfigurationContext>> Configurators { get; }

        public ITypeList<Profile> ValidatingProfiles { get; set; }

        public AutoMapperOptions()
        {
            Configurators = new List<Action<IAutoMapperConfigurationContext>>();
            ValidatingProfiles = new TypeList<Profile>();
        }

        /// <summary>
        /// 模块中继承Profile文件反射获取mapper配置
        /// </summary>
        /// <typeparam name="TModule"></typeparam>
        /// <param name="validate"></param>
        public void AddMaps<TModule>(bool validate = false)
        {
            var assembly = typeof(TModule).Assembly;

            Configurators.Add(context =>
            {
                context.MapperConfiguration.AddMaps(assembly);
            });

            if (validate)
            {
                var profileTypes = assembly
                    .DefinedTypes
                    .Where(type => typeof(Profile).IsAssignableFrom(type) && !type.IsAbstract && !type.IsGenericType);

                foreach (var profileType in profileTypes)
                {
                    ValidatingProfiles.Add(profileType);
                }
            }
        }
    }
}

//https://blog.csdn.net/xiuyuandashen/article/details/132857095