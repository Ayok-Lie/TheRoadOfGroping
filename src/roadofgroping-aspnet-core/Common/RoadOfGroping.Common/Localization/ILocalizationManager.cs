namespace RoadOfGroping.Common.Localization
{
    public interface ILocalizationManager
    {
        ILocalizationSource GetSource(string name);

        IReadOnlyList<ILocalizationSource> GetAllSources();
    }
}