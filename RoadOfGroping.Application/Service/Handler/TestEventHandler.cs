using Microsoft.Extensions.Logging;
using RoadOfGroping.Common.DependencyInjection;
using RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus;

namespace RoadOfGroping.Application.Service.Handler
{
    public class TestEventHandler : IEventHandle<TestDto>, ITransientDependency
    {
        public TestEventHandler(ILoggerFactory factory)
        {
        }

        public Task Handle(TestDto eto)
        {
            Console.WriteLine($"{typeof(TestDto).Name}--{eto.Name}--{eto.Description}");
            return Task.CompletedTask;
        }
    }
}