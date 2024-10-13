using System.IO;
using Microsoft.Extensions.Configuration;

namespace SematicKernelApp.Extensions
{
    public class ConfigExtensions
    {
        public static T FromConfig<T>(string sectionName)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<ConfigExtensions>()
                   .Build();
            return configuration.GetSection(sectionName).Get<T>()
                 ?? throw new InvalidDataException("Invalid semantic kernel configuration is empty");
        }
    }
}