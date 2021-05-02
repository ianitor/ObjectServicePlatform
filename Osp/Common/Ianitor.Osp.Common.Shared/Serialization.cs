using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Ianitor.Osp.Common.Shared
{
    public static class Serialization
    {
        public static T Deserialize<T>(string xmlString, string rootElementName = null)
        {
            XmlRootAttribute xmlRootAttribute = null;
            if (!string.IsNullOrWhiteSpace(rootElementName))
            {
                xmlRootAttribute = new XmlRootAttribute(rootElementName);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T), xmlRootAttribute);

            using (var reader = new StringReader(xmlString))
            {
                return (T) serializer.Deserialize(reader);
            }
        }

        public static string Serialize<T>(T pieceRequest, string rootElementName = null)
        {
            XmlRootAttribute xmlRootAttribute = null;
            if (!string.IsNullOrWhiteSpace(rootElementName))
            {
                xmlRootAttribute = new XmlRootAttribute(rootElementName);
            }

            //Create our own namespaces for the output
            var emptyNamespaces = new XmlSerializerNamespaces(new[] {XmlQualifiedName.Empty});

            var settings = new XmlWriterSettings {Indent = false, OmitXmlDeclaration = true};

            XmlSerializer serializer = new XmlSerializer(typeof(T), xmlRootAttribute);

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, pieceRequest, emptyNamespaces);
                return stream.ToString();
            }

//            
//            {
//                serializer.Serialize(writer, pieceRequest, ns);
//                
//                writer.Flush();
//                return writer.ToString();
//            }
        }
    }
}