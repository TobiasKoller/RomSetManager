using System;
using System.Xml;
using System.Xml.Schema;

namespace Configuration
{
    public class XmlReaderBase
    {
        protected XmlDocument XmlDocument;

        protected XmlReaderBase()
        {
            XmlDocument = new XmlDocument();
        }

        public void Read(string xmlFile)
        {
            Read(xmlFile, null);
        }

        public void Read(string xmlFile, string xsdFile)
        {
            var settings = GetXmlSettings(xsdFile);

            using (var reader = System.Xml.XmlReader.Create(xmlFile, settings))
            {
                XmlDocument.Load(reader);

                if (!string.IsNullOrWhiteSpace(xsdFile))
                    XmlDocument.Validate(DocumentValidationHandler);
            }
        }

        /// <summary>
        /// Getting XmlSettings with xsdValidation.
        /// If xsdFile is null, no validation will be performed
        /// </summary>
        /// <param name="xsdFile"></param>
        /// <returns></returns>
        private XmlReaderSettings GetXmlSettings(string xsdFile)
        {
            if (string.IsNullOrWhiteSpace(xsdFile))
            {
                return new XmlReaderSettings
                {
                    IgnoreComments = true
                };
            }
            var schemaReader = new XmlTextReader(xsdFile);
            var schema = XmlSchema.Read(schemaReader, SchemaValidationHandler);

            var settings = new XmlReaderSettings { ValidationType = ValidationType.Schema };
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += DocumentValidationHandler;
            settings.IgnoreComments = true;
            settings.Schemas.Add(schema);

            return settings;
        }

        private void DocumentValidationHandler(object sender, ValidationEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SchemaValidationHandler(object sender, ValidationEventArgs e)
        {
            throw new NotImplementedException();
        }



        #region Helper Methods

        protected string GetInnerTextAsString(string xPath)
        {
            var node = XmlDocument.SelectSingleNode(xPath);
            return node == null ? string.Empty : GetAsString(node.InnerText);
        }

        protected string GetInnerTextAsString(XmlNode node)
        {
            return node == null ? string.Empty : GetAsString(node.InnerText);
        }

        protected int GetInnerTextAsInt(XmlNode node, int defaultValue)
        {
            return node == null ? defaultValue : GetAsInt(node.InnerText, defaultValue);
        }

        protected string GetAttributeAsString(XmlNode xmlNode, string attributeName)
        {
            if (xmlNode == null || xmlNode.Attributes == null)
                return string.Empty;

            var attribute = xmlNode.Attributes[attributeName];
            return attribute == null ? string.Empty : GetAsString(attribute.InnerText);
        }

        protected string GetAttributeAsString(string xPath, string attributeName)
        {
            var node = XmlDocument.SelectSingleNode(xPath);
            return GetAttributeAsString(node, attributeName);
        }

        protected string GetAttributeAsNullableString(XmlNode xmlNode, string attributeName)
        {
            if (xmlNode == null || xmlNode.Attributes == null)
                return null;

            var attribute = xmlNode.Attributes[attributeName];
            return attribute == null ? null : GetAsString(attribute.InnerText);
        }

        protected int? GetAttributeAsNullableInt(XmlNode xmlNode, string attributeName, int? defaultValue)
        {
            if (xmlNode == null || xmlNode.Attributes == null)
                return defaultValue;

            var attribute = xmlNode.Attributes[attributeName];
            return attribute == null ? defaultValue : GetAsNullableInt(attribute.InnerText, defaultValue);
        }

        protected int GetAttributeAsInt(XmlNode xmlNode, string attributeName, int defaultValue)
        {
            var value = GetAttributeAsNullableInt(xmlNode, attributeName, defaultValue);
            if (value == null)
                return defaultValue;

            return (int)value;
        }

        protected bool GetAttributeAsBool(XmlNode xmlNode, string attributeName, bool defaultValue)
        {
            var value = GetAttributeAsNullableString(xmlNode, attributeName);
            if (value == null)
                return defaultValue;

            return (value.ToLower() == "true" || value.ToLower() == "1");
        }

        protected bool GetAttributeAsBool(XmlNode xmlNode, string attributeName)
        {
            return GetAttributeAsBool(xmlNode, attributeName, false);
        }

        protected bool GetInnerTextAsBool(string xPath)
        {
            var node = XmlDocument.SelectSingleNode(xPath);
            return node != null && GetAsBoolean(node.InnerText);
        }

        private string GetAsString(string value)
        {
            return value ?? string.Empty;
        }

        private int? GetAsNullableInt(string value, int? defaultValue)
        {
            int result;
            return (int.TryParse(value, out result)) ? result : defaultValue;
        }

        private int GetAsInt(string value, int defaultValue)
        {
            int result;
            return (int.TryParse(value, out result)) ? result : defaultValue;
        }

        private bool GetAsBoolean(string value)
        {
            if (value == null)
                return false;
            var trimmedValue = value.Trim().ToLower();
            return trimmedValue == "1" || trimmedValue == "true";
        }

        #endregion


    }
}