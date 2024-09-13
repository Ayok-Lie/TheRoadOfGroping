using RoadOfGroping.Common.DependencyInjection;
using RoadOfGroping.Repository.DomainService;
using RoadOfGroping.Repository.Entities;

namespace RoadOfGroping.EntityFramework.Repositorys
{
    public class Testauuu : AnotherDomainService<Test, int>, ITransientDependency, ITestauuu
    {
        public Testauuu(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override IQueryable<Test> GetIncludeQuery()
        {
            throw new NotImplementedException();
        }

        public Task GetTest()
        {
            var data = this.Query.ToList();
            return Task.CompletedTask;
        }

        public override Task ValidateOnCreateOrUpdate(Test entity)
        {
            throw new NotImplementedException();
        }

        public override Task ValidateOnDelete(Test entity)
        {
            throw new NotImplementedException();
        }
    }

    public interface ITestauuu : ITransientDependency
    {
        Task GetTest();
    }

    public class Test : Entity<int>
    {
        public string Name { get; set; }
    }
}