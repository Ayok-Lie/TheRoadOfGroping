namespace RoadOfGroping.Model.Interface
{
    public interface IApplicationServiceProvider
    {
        void Initialize(IServiceProvider serviceProvider);

        Task InitializeAsync(IServiceProvider serviceProvider);
    }
}