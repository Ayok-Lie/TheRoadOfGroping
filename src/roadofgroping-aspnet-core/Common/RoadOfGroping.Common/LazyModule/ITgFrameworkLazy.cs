using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadOfGroping.Common.LazyModule
{
    public interface ITgFrameworkLazy
    {
        Lazy<T> LazyGetRequiredService<T>();

        Lazy<object> LazyGetRequiredService(Type serviceType);

        Lazy<T?> LazyGetService<T>() where T : class;

        Lazy<object?> LazyGetService(Type serviceType);

        Lazy<T> LazyGetService<T>(T defaultValue);

        Lazy<object> LazyGetService(Type serviceType, object defaultValue);

        Lazy<object> LazyGetService(Type serviceType, Func<IServiceProvider, object> factory);

        Lazy<T> LazyGetService<T>(Func<IServiceProvider, object> factory);
    }
}
