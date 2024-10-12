namespace RoadOfGroping.Repository.DomainService
{
    public class ApplicationService : ServiceBase, IApplicationService
    {
        public ApplicationService(IServiceProvider serviceProvider, string? localizationSourceName = null) : base(serviceProvider)
        {
            base.LocalizationSourceName = "Localization";
        }
    }
}