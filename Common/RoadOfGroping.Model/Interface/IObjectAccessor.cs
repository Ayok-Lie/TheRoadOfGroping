namespace RoadOfGroping.Model.Interface
{
    public interface IObjectAccessor<T>
    {
        T? Value { get; set; }
    }
}