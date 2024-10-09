using Microsoft.Extensions.DependencyInjection;

namespace RoadOfGroping.Model.Interface
{
    public interface IModuleApplication : IModuleContainer
    {
        Type StartModuleType { get; }
        IServiceCollection Services { get; }

        IServiceProvider ServiceProvider { get; }

        void ConfigerService();

        void InitApplication(IServiceProvider serviceProvider);

        Task InitApplicationAsync(IServiceProvider serviceProvider);
    }
}