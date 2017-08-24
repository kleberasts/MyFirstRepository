using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Imposto.Core.Util
{
    public class Util
    {
        public static string Serialize<T>(T MyObject)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(T));
            var xml = string.Empty;

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, MyObject);
                    xml = sww.ToString(); // Your XML
                }
            }
            return xml;
        }

        public enum Estado
        {
            AC, AL, AM, AP, BA, CE, DF, ES, GO, MA, MG, MS, MT, PA, PB, PE, PI, PR, RJ, RN, RO, RR, RS, SC, SE, SP, TO
        }
    }
}
