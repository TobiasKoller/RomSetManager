using System.IO;
using Configuration;

namespace RomSetManager.Services
{
    public class ServiceProvider : IServiceProvider
    {
        public ServiceProvider()
        {
            ConfigurationService = new ConfigurationService();
        }

        public ConfigurationService ConfigurationService { get; set; }
    }

    public class ConfigurationService
    {
        private Model.Configuration _configuration;
        private ConfigurationReader _configurationReader;

        public ConfigurationService()
        {
            _configurationReader = new ConfigurationReader(Directory.GetCurrentDirectory());

        }

        public Model.Configuration GetConfiguration()
        {
            return _configurationReader.GetConfiguration();
        }
    }
}
