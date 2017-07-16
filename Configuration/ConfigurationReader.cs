using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Model;

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

            ReadBestMatch();

            ReadSystems();
            
            return _configuration;
        }


        private void ReadBestMatch()
        {
            var bestMatchNode = XmlDocument.SelectSingleNode("//configuration/bestmatch");
            var bestMatch = new BestMatch();
            var nameParts = bestMatchNode.SelectNodes("//preferences/nameparts/namepart");

            var preferences = new Preferences
            {
                IgnoreMustHaveForOneRom = GetInnerTextAsBool(bestMatchNode.SelectSingleNode("//preferences/ignore_musthaves_for_one_rom")),
                IgnoreNeverUseForOneRom = GetInnerTextAsBool(bestMatchNode.SelectSingleNode("//preferences/ignore_neveruse_for_one_rom"))
            };


            foreach (XmlNode namePartNode in nameParts)
            {
                var name = GetAttributeAsString(namePartNode, "name");
                var value = GetAttributeAsString(namePartNode, "value");
                var system = GetAttributeAsString(namePartNode, "system");
                var type = GetAttributeAsString(namePartNode, "type");
                var description = GetAttributeAsString(namePartNode, "description");
                var position = GetAttributeAsInt(namePartNode, "position", -1);
                var behaviour = GetAttributeAsString(namePartNode, "behaviour");

                var namePart = new NamePart(name, value, system, type, description, position, behaviour);
                preferences.NameParts.Add(namePart);
            }

            RearangeElements(preferences);


            bestMatch.Preferences = preferences;

            bestMatch.RomSourceDirectory = GetAttributeAsString(bestMatchNode.SelectSingleNode("//rom_source_directory"), "value");
            bestMatch.RomDestinationDirectory = GetAttributeAsString(bestMatchNode.SelectSingleNode("//rom_destination_directory"), "value");

            _configuration.BestMatch = bestMatch;
        }


        private void ReadSystems()
        {
            var systemsNodes = XmlDocument.SelectNodes("//configuration/systems/system");

            foreach (XmlNode systemsNode in systemsNodes)
            {
                var name = GetAttributeAsString(systemsNode, "name");
                var keepCompression = GetAttributeAsBool(systemsNode, "keep_compressed");

                var system = new Model.System
                {
                    Name = name,
                    KeepCompressed = keepCompression
                };

                _configuration.Systems.Add(system);
            }
            
        }


        private void RearangeElements(Preferences preferences)
        {
            var withoutPos = preferences.NameParts.Where(n => n.Position == -1).ToList();
            var withPos = preferences.NameParts.Where(n => n.Position > -1).OrderBy(o => o.Position).ToList();

            for (var i = 1; i <= withPos.Count; i++)
                withPos[i - 1].Position = i;

            var nextPos = withPos.Count;

            for (var i = 1; i <= withoutPos.Count; i++)
                withoutPos[i - 1].Position = nextPos + i;

        }
    }
}
