using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadOfGroping.EntityFramework.Domain
{
    public abstract class Entity1 : IEntity1
    {
        public abstract object[] GetKeys();

        public override string ToString()
        {
            return $"[Entity: {GetType().Name}] Keys = {string.Join(",", GetKeys())}";
        }

        #region

        //private List<IDomainEvent> _domainEvents;
        //public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

        //public void AddDomainEvent(IDomainEvent eventItem)
        //{
        //    _domainEvents = _domainEvents ?? new List<IDomainEvent>();
        //    _domainEvents.Add(eventItem);
        //}

        //public void RemoveDomainEvent(IDomainEvent eventItem)
        //{
        //    _domainEvents?.Remove(eventItem);
        //}

        //public void ClearDomainEvents()
        //{
        //    _domainEvents?.Clear();
        //}

        #endregion
    }

    public abstract class Entity1<Tkey>
    : Entity1, IEntity1<Tkey>
    {
        protected Entity1()
        {
        }

        public override object[] GetKeys()
        {
            return new object[] { Id };
        }

        /// <summary>
        /// ID
        /// </summary>
        [MaxLength(36)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Tkey Id { get; set; }

        protected Entity1(Tkey id) => Id = id;

        public override string ToString()
        {
            return $"[Entity:{GetType().Name}] Id = {Id}";
        }
    }

    //public abstract class Entity1<TKey> : Entity1, IEntity1<TKey>
    //{
    //    private int? _requestedHashCode;
    //    public virtual TKey Id { get; protected set; }

    //    public override object[] GetKeys()
    //    {
    //        return new object[] { Id };
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (obj == null || !(obj is Entity1<TKey>))
    //            return false;

    //        if (Object.ReferenceEquals(this, obj))
    //            return true;

    //        if (this.GetType() != obj.GetType())
    //            return false;

    //        Entity1<TKey> item = (Entity1<TKey>)obj;

    //        if (item.IsTransient() || this.IsTransient())
    //            return false;
    //        else
    //            return item.Id.Equals(this.Id);
    //    }

    //    public override int GetHashCode()
    //    {
    //        if (!IsTransient())
    //        {
    //            if (!_requestedHashCode.HasValue)
    //                _requestedHashCode = this.Id.GetHashCode() ^ 31;

    //            return _requestedHashCode.Value;
    //        }
    //        else
    //            return base.GetHashCode();
    //    }

    //    //表示对象是否为全新创建的，未持久化的
    //    public bool IsTransient()
    //    {
    //        return EqualityComparer<TKey>.Default.Equals(Id, default);
    //    }

    //    public override string ToString()
    //    {
    //        return $"[Entity: {GetType().Name}] Id = {Id}";
    //    }

    //    public static bool operator ==(Entity1<TKey> left, Entity1<TKey> right)
    //    {
    //        if (Object.Equals(left, null))
    //            return (Object.Equals(right, null)) ? true : false;
    //        else
    //            return left.Equals(right);
    //    }

    //    public static bool operator !=(Entity1<TKey> left, Entity1<TKey> right)
    //    {
    //        return !(left == right);
    //    }
    //}
}