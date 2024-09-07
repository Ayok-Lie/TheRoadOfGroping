using RoadOfGroping.Common.DependencyInjection;

namespace RoadOfGroping.Repository.Auditing
{
    public interface IAuditPropertySetter : ITransientDependency
    {
        void SetCreationProperties(object targetObject);

        void SetDeletionProperties(object targetObject);

        void SetModificationProperties(object targetObject);
    }
}