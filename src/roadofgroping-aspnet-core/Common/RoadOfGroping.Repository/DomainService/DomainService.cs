using RoadOfGroping.Common.DependencyInjection;

namespace RoadOfGroping.Repository.DomainService
{
    public class DomainService : ServiceBase, IDomainService, ITransientDependency
    {
        public DomainService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}