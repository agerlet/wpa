using System.IO;
using System.Reflection;
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
            var fullpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ;
            return new ConfigurationBuilder()
                .SetBasePath(fullpath)
                .AddJsonFile("dictionary/sections.json", optional: false, reloadOnChange: true)
                .AddJsonFile("dictionary/book-names.json", false, true)
                .Build();
        }
    }
}