using System.Linq;
using System.Xml;
using Model;

namespace Configuration
{
    public class BestMatchReader : XmlReaderBase
    {
        public BestMatch Get(XmlNode bestMatchNode)
        {
            var bestMatch = new BestMatch();
            var nameParts = bestMatchNode.SelectNodes("//preferences/nameparts/namepart");

            var preferences = new Preferences();

            foreach (XmlNode namePartNode in nameParts)
            {
                var name = GetAttributeAsString(namePartNode, "name");
                var value = GetAttributeAsString(namePartNode, "value");
                var system = GetAttributeAsString(namePartNode, "system");
                var type = GetAttributeAsString(namePartNode, "type");
                var description = GetAttributeAsString(namePartNode, "description");
                var position = GetAttributeAsInt(namePartNode, "position",-1);
                var include = GetAttributeAsString(namePartNode, "include");

                var namePart = new NamePart(name, value, system, type, description, position, include);
                preferences.NameParts.Add(namePart);
            }

            RearangeElements(preferences);


            bestMatch.Preferences = preferences;
            
            bestMatch.RomSourceDirectory = GetAttributeAsString(bestMatchNode.SelectSingleNode("//rom_source_directory"), "value");
            bestMatch.RomDestinationDirectory = GetAttributeAsString(bestMatchNode.SelectSingleNode("//rom_destination_directory"), "value");

            return bestMatch;
        }

        private void RearangeElements(Preferences preferences)
        {
            var withoutPos = preferences.NameParts.Where(n => n.Position==-1).ToList();
            var withPos = preferences.NameParts.Where(n => n.Position > -1).OrderBy(o => o.Position).ToList();

            for (var i = 1; i <= withPos.Count; i++)
                withPos[i-1].Position = i;

            var nextPos = withPos.Count;

            for (var i = 1; i <= withoutPos.Count; i++)
                withoutPos[i-1].Position = nextPos+i;

        }
    }
}