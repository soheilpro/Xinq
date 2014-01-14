using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Xinq
{
    internal class Query
    {
        private XinqDocument _document;
        private string _name;
        private string _comment;
        private string _text;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value)
                    return;

                _name = value;

                _document.IsDirty = true;
            }
        }

        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                if (_comment == value)
                    return;

                _comment = value;

                _document.IsDirty = true;
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text == value)
                    return;

                _text = value;

                _document.IsDirty = true;
            }
        }

        public string NormalizedText
        {
            get
            {
                var normalizedText = new StringBuilder();

                using (var reader = new StringReader(_text))
                {
                    while (true)
                    {
                        var line = reader.ReadLine();

                        if (line == null)
                            break;

                        line = line.Trim(' ', '\t');

                        if (line.Length == 0)
                            continue;

                        normalizedText.AppendLine(line);
                    }
                }

                return normalizedText.ToString();
            }
        }

        public Query(XinqDocument document)
        {
            _document = document;
        }

        public Query(XinqDocument document, XmlNode node) : this(document)
        {
            if (node.Attributes["name"] == null)
                throw new XmlException("Missing required attribute 'name'!");

            _name = node.Attributes["name"].Value;

            if (node.SelectSingleNode("comment") != null)
                _comment = node.SelectSingleNode("comment").InnerText;

            if (node.SelectSingleNode("text") == null)
                throw new XmlException("Missing required element 'text'!");

            _text = node.SelectSingleNode("text").InnerText;
        }

        public void Save(XmlNode parentNode)
        {
            var xmlDocument = parentNode.OwnerDocument;
            var queryElement = xmlDocument.CreateElement("query");

            var nameAttribute = xmlDocument.CreateAttribute("name");
            nameAttribute.Value = _name;
            queryElement.Attributes.Append(nameAttribute);

            if (!string.IsNullOrEmpty(_comment))
            {
                var commentElement = xmlDocument.CreateElement("comment");
                commentElement.AppendChild(xmlDocument.CreateCDataSection(_comment));
                queryElement.AppendChild(commentElement);
            }

            var textElement = xmlDocument.CreateElement("text");
            textElement.AppendChild(xmlDocument.CreateCDataSection(_text));
            queryElement.AppendChild(textElement);

            parentNode.AppendChild(queryElement);
        }
    }
}
