namespace RoadOfGroping.Core.Services
{
    public class ValuesService : IValuesService, IDefaultDomainService
    {
        public string GetValues()
        {
            return "Hello from ValuesService";
        }
    }
}