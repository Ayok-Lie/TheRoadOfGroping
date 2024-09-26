using System.Globalization;
using Microsoft.Extensions.Localization;

namespace RoadOfGroping.Common.Localization
{
    public interface ILocalizationSource
    {
        string Name { get; }

        string GetString(string name);

        string GetString(string name, CultureInfo culture);

        string GetString(string name, params object[] args);

        string GetString(string name, CultureInfo culture, params object[] args);

        string GetStringOrNull(string name, bool tryDefaults = true);

        string GetStringOrNull(string name, CultureInfo culture, bool tryDefaults = true);

        List<string> GetStrings(List<string> names);

        List<string> GetStrings(List<string> names, CultureInfo culture);

        List<string> GetStringsOrNull(List<string> names, bool tryDefaults = true);

        List<string> GetStringsOrNull(List<string> names, CultureInfo culture, bool tryDefaults = true);

        IReadOnlyList<LocalizedString> GetAllStrings(bool includeDefaults = true);

        IReadOnlyList<LocalizedString> GetAllStrings(CultureInfo culture, bool includeDefaults = true);
    }
}