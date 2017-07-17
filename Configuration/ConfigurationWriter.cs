using System.IO;
using System.Linq;
using System.Xml;

namespace Configuration
{
    public class ConfigurationWriter
    {
        private string _configDir;

        public ConfigurationWriter(string configDir)
        {
            _configDir = configDir;
        }

        public void Write(Model.Configuration configuration)
        {
            var configFile = Path.Combine(_configDir, "Configuration.xml");
            //var configFile = Path.Combine(_configDir, "Configuration_update.xml");

            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true,
            };

            using (XmlWriter writer = XmlWriter.Create(configFile, settings))
            {
                writer.WriteStartElement("configuration");
                WriteBestMatch(writer, configuration);
                WriteSystems(writer, configuration);

                writer.WriteEndElement(); //configuration
            }

        }

        private void WriteBestMatch(XmlWriter writer, Model.Configuration configuration)
        {
            writer.WriteStartElement("bestmatch");

            writer.WriteStartElement("rom_source_directory");
            writer.WriteAttributeString("value", configuration.BestMatch.RomSourceDirectory);
            writer.WriteEndElement(); //rom_source_directory

            writer.WriteStartElement("rom_destination_directory");
            writer.WriteAttributeString("value", configuration.BestMatch.RomDestinationDirectory);
            writer.WriteEndElement(); //rom_destination_directory

            writer.WriteStartElement("preferences");

            writer.WriteStartElement("ignore_musthaves_for_one_rom");
            writer.WriteString(configuration.BestMatch.Preferences.IgnoreMustHaveForOneRom.ToString());
            writer.WriteEndElement(); //ignore_musthaves_for_one_rom

            writer.WriteStartElement("ignore_neveruse_for_one_rom");
            writer.WriteString(configuration.BestMatch.Preferences.IgnoreNeverUseForOneRom.ToString());
            writer.WriteEndElement(); //ignore_neveruse_for_one_rom

            writer.WriteStartElement("nameparts");

            foreach (var namePart in configuration.BestMatch.Preferences.NameParts.OrderBy(l => l.Position))
            {
                writer.WriteStartElement("namepart");
                writer.WriteAttributeString("name", namePart.Name);
                writer.WriteAttributeString("value", namePart.Value);
                writer.WriteAttributeString("system", namePart.System);
                writer.WriteAttributeString("type", namePart.Type.ToString());
                writer.WriteAttributeString("behaviour", namePart.Behaviour.ToString());
                writer.WriteAttributeString("position", namePart.Position.ToString());
                writer.WriteAttributeString("description", namePart.Description);
                writer.WriteEndElement(); //namepart
            }

            writer.WriteEndElement(); //nameparts
            writer.WriteEndElement(); //preferences

            writer.WriteEndElement(); //BestMatch
        }

        private void WriteSystems(XmlWriter writer, Model.Configuration configuration)
        {
            writer.WriteStartElement("systems");

            foreach (var configurationSystem in configuration.Systems)
            {
                writer.WriteStartElement("system");

                writer.WriteAttributeString("name",configurationSystem.Name);
                writer.WriteAttributeString("keep_compressed", configurationSystem.KeepCompressed.ToString());

                writer.WriteEndElement(); //system
            }

            writer.WriteEndElement(); //systems
        }
    }
}