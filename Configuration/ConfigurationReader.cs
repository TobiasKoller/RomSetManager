using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public class ConfigurationReader : XmlReaderBase
    {
        private readonly string _directory;
        private Model.Configuration _configuration;

        public ConfigurationReader(string directory)
        {
            _directory = directory;
        }
        

        public Model.Configuration GetConfiguration()
        {
            _configuration = new Model.Configuration();

            XmlDocument.Load(Path.Combine(_directory, "Configuration.xml"));
            var bestMatchNode = XmlDocument.SelectSingleNode("//configuration/bestmatch");
            _configuration.BestMatch = new BestMatchReader().Get(bestMatchNode);

            return _configuration;
        }
        
    }
}
