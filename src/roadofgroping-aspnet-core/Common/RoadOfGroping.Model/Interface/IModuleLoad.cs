using Microsoft.Extensions.DependencyInjection;

namespace RoadOfGroping.Model.Interface
{
    public interface IModuleLoad
    {
        List<IBaseModuleDescritor> GetModuleDescritors(IServiceCollection service, Type startupModuleType);
    }
}