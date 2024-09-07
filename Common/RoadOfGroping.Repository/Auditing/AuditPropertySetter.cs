using RoadOfGroping.Common.DependencyInjection;
using RoadOfGroping.Common.Extensions;
using RoadOfGroping.Repository.UserSession;

namespace RoadOfGroping.Repository.Auditing
{
    public class AuditPropertySetter : IAuditPropertySetter, ITransientDependency
    {
        private readonly IUserSession _userSession;

        public AuditPropertySetter(IUserSession userSession)
        {
            _userSession = userSession;
        }

        public virtual void SetCreationProperties(object targetObject)
        {
            SetCreationInfo(targetObject);
            SetIsDeleter(targetObject);
        }

        public virtual void SetDeletionProperties(object targetObject)
        {
            SetDeletionInto(targetObject);
            SetIsDeleter(targetObject, true);
        }

        public virtual void SetModificationProperties(object targetObject)
        {
            SetModificationInfo(targetObject);
        }

        protected virtual void SetCreationInfo(object targetObject)
        {
            if (!(targetObject is ICreationAuditedEntity mayHaveCreatorObject))
            {
                return;
            }

            if (mayHaveCreatorObject.CreationTime == default)
            {
                ObjectPropertyHelper.TrySetProperty(mayHaveCreatorObject, x => x.CreationTime, () => DateTime.Now);
            }

            if ((!mayHaveCreatorObject.CreatorId.IsNullEmpty() && mayHaveCreatorObject.CreatorId != default) || _userSession.UserId.IsNullEmpty())
            {
                return;
            }

            ObjectPropertyHelper.TrySetProperty(mayHaveCreatorObject, x => x.CreatorId, () => _userSession.UserId);
        }

        protected virtual void SetDeletionInto(object targetObject)
        {
            if (!(targetObject is IDeletionAuditedEntity deletionAuditedObject))
            {
                return;
            }

            if (deletionAuditedObject.DeletionTime == null)
            {
                ObjectPropertyHelper.TrySetProperty(deletionAuditedObject, x => x.DeletionTime, () => DateTime.Now);
            }

            if (deletionAuditedObject.DeleterId != null || _userSession.UserId.IsNullEmpty())
            {
                return;
            }
            ObjectPropertyHelper.TrySetProperty(deletionAuditedObject, x => x.DeleterId, () => _userSession.UserId);
        }

        protected virtual void SetModificationInfo(object targetObject)
        {
            if (!(targetObject is IModificationAuditedEntity mayHaveModifierObject))
            {
                return;
            }

            if (mayHaveModifierObject.ModificationTime == null)
            {
                ObjectPropertyHelper.TrySetProperty(mayHaveModifierObject, x => x.ModificationTime, () => DateTime.Now);
            }

            if (mayHaveModifierObject.ModifierId != null || _userSession.UserId.IsNullEmpty())
            {
                return;
            }
            ObjectPropertyHelper.TrySetProperty(mayHaveModifierObject, x => x.ModifierId, () => _userSession.UserId);
        }

        protected virtual void SetIsDeleter(object targetObject, bool isDelete = false)
        {
            if (!(targetObject is IDeletionAuditedEntity softDelete))
            {
                return;
            }

            ObjectPropertyHelper.TrySetProperty(softDelete, x => x.IsDeleted, () => isDelete);
        }
    }
}