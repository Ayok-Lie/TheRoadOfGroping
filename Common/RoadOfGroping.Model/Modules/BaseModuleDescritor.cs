using System.Reflection;
using RoadOfGroping.Model.Interface;

namespace RoadOfGroping.Model.Modules
{
    public class BaseModuleDescritor : IBaseModuleDescritor
    {
        public Type ModuleType { get; }

        public IBaseModule Instance { get; }

        public BaseModuleDescritor(Type type, IBaseModule instance)
        {
            if (!type.GetTypeInfo().IsAssignableFrom(instance.GetType()))
            {
                throw new ArgumentException($"Given module instance ({instance.GetType().AssemblyQualifiedName}) is not an instance of given module type: {type.AssemblyQualifiedName}");
            }
            ModuleType = type;
            Instance = instance;
        }
    }
}