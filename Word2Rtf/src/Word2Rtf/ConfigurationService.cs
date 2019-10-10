using System.IO;
using Microsoft.Extensions.Configuration;

namespace Word2Rtf
{
    public interface IConfigurationService
    {
        IConfiguration GetConfiguration();
    }
    
    public class ConfigurationService : IConfigurationService
    {
        public ConfigurationService()
        {
        }

        public IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("dictionary/sections.json", optional: false, reloadOnChange: true)
                .AddJsonFile("dictionary/book-names.json", false, true)
                .Build();
        }
    }
}