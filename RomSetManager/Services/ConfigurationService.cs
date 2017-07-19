using System.IO;
using Configuration;

namespace RomSetManager.Services
{
    public class ConfigurationService
    {
        private Model.Configuration _configuration;
        private readonly ConfigurationReader _configurationReader;
        private ConfigurationWriter _configurationWriter;

        public ConfigurationService()
        {
            var configDir = "";
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "configuration.xml")))
                configDir = Path.Combine(Directory.GetCurrentDirectory());
            else
                configDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Config");

            _configurationReader = new ConfigurationReader(configDir);
            _configurationWriter = new ConfigurationWriter(configDir);

        }

        public Model.Configuration GetConfiguration(bool reloadFromSource=true)
        {
            if(_configuration==null || reloadFromSource)
                _configuration = _configurationReader.GetConfiguration();

            return _configuration;
        }

        public void UpdateConfiguration(Model.Configuration configuration)
        {
            _configuration = configuration;
            _configurationWriter.Write(configuration);
        }
        
    }
}