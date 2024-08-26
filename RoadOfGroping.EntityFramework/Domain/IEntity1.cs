namespace RoadOfGroping.EntityFramework.Domain
{
    public interface IEntity1
    {
        object[] GetKeys();
    }

    public interface IEntity1<TKey> : IEntity1
    {
        TKey Id { get; }
    }
}