using RoadOfGroping.Utility.EventBus.Attributes;

namespace RoadOfGroping.Application.Service.Handler
{
    [EventDiscriptor("test", 1000, false)]
    public class TestDto
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}