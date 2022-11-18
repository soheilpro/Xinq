using System;
using System.Text;
using System.Xml;

namespace Xinq
{
    internal class XinqDocument : Document
    {
        private QueryCollection _queries;

        public QueryCollection Queries
        {
            get
            {
                return _queries;
            }
        }

        public XinqDocument()
        {
            _queries = new QueryCollection(this);
        }

        protected override void LoadDocument(string filename)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(filename);

            _queries = new QueryCollection(this);

            foreach (XmlNode node in xmlDocument.SelectNodes(@"xinq/queries/query"))
            {
                var query = new Query(this, node);
                _queries.Add(query);
            }
        }

        protected override void SaveDocument(string filename)
        {
            var xmlDocument = new XmlDocument();

            var xinqElement = xmlDocument.CreateElement("xinq");
            xmlDocument.AppendChild(xinqElement);

            var queriesElement = xmlDocument.CreateElement("queries");
            xinqElement.AppendChild(queriesElement);

            foreach (var query in _queries)
                query.Save(queriesElement);

            using (var xmlWriter = new XmlTextWriter(filename, Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;

                xmlDocument.Save(xmlWriter);
            }
        }
    }
}
