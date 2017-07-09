using System.Xml;
using Model;

namespace Configuration
{
    public class BestMatchReader : XmlReaderBase
    {
        public BestMatch Get(XmlNode languagesNode)
        {
            var bestMatch = new BestMatch();
            var languageNodes = languagesNode.SelectNodes("//language");

            var languages = new LanguageList();

            foreach (XmlNode languageNode in languageNodes)
            {
                var language = new Language
                {
                    Name = GetAttributeAsString(languageNode, "name"),
                    Value = GetAttributeAsString(languageNode, "value"),
                    Position = GetAttributeAsInt(languageNode, "position", 1)
                };

                languages.Add(language);
            }

            bestMatch.LanguageList = languages;
            return bestMatch;
        }
    }
}