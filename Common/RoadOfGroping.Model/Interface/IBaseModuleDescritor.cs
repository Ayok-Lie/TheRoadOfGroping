namespace RoadOfGroping.Model.Interface
{
    public interface IBaseModuleDescritor
    {
        public Type ModuleType { get; }

        public IBaseModule Instance { get; }
    }
}