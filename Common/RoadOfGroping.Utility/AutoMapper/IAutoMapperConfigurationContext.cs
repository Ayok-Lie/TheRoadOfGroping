using AutoMapper;

namespace RoadOfGroping.Utility.AutoMapper
{
    public interface IAutoMapperConfigurationContext
    {
        IMapperConfigurationExpression MapperConfiguration { get; }

        IServiceProvider ServiceProvider { get; }
    }
}