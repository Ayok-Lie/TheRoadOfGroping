namespace RoadOfGroping.Repository.DomainService
{
    public class ApplicationService : ServiceBase
    {
        public ApplicationService(IServiceProvider serviceProvider, string? localizationSourceName = null) : base(serviceProvider)
        {
            base.LocalizationSourceName = "Localization";
        }
    }
}