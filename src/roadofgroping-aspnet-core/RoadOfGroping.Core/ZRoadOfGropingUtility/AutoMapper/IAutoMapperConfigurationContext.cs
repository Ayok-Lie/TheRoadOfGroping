using AutoMapper;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.AutoMapper
{
    public interface IAutoMapperConfigurationContext
    {
        IMapperConfigurationExpression MapperConfiguration { get; }

        IServiceProvider ServiceProvider { get; }
    }
}