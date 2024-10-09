namespace RoadOfGroping.Model.Interface
{
    public interface IModuleContainer
    {
        IReadOnlyList<IBaseModuleDescritor> Modules { get; }
    }
}