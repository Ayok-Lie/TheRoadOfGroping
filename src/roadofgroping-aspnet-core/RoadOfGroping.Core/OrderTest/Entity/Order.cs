using RoadOfGroping.Repository.Entities;

namespace RoadOfGroping.Core.OrderTest.Entity
{
    public class Order : Entity<long>
    {
        public string? Name { get; set; }

        public DateTime DateTime { get; set; }
    }
}