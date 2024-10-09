using RoadOfGroping.Common.Collections;
using RoadOfGroping.Model.Interface;

namespace RoadOfGroping.Model.Modules
{
    public class BaseModuleLifecycleOptions
    {
        public ITypeList<IModuleLifecycleContributor> Contributors { get; }

        public BaseModuleLifecycleOptions()
        {
            Contributors = new TypeList<IModuleLifecycleContributor>();
        }
    }
}