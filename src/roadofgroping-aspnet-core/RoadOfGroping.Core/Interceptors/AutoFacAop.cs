using Castle.DynamicProxy;

namespace RoadOfGroping.Core.Interceptors
{
    [AttributeUsage(AttributeTargets.All)]
    public class AutoFacAop : Attribute
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine("执行前");
            invocation.Proceed();
            Console.WriteLine("执行后");
        }
    }
}