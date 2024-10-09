using RoadOfGroping.Model.Interface;

namespace RoadOfGroping.Model.Modules
{
    public class DependOnAttribute : Attribute, IDependsAttrProvider
    {
        public Type[] DependedTypes { get; }

        public DependOnAttribute(params Type[] types)
        {
            DependedTypes = types ?? new Type[0];
        }

        public Type[] GetDependsModulesType()
        {
            return DependedTypes;
        }
    }
}