using RoadOfGroping.Model.Interface;

namespace RoadOfGroping.Model.Modules
{
    public class ObjectAccessor<T> : IObjectAccessor<T>
    {
        public T? Value { get; set; }

        public ObjectAccessor()
        { }

        public ObjectAccessor(T value)
        {
            Value = value;
        }
    }
}