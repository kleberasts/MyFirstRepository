using System.Xml.Serialization;

namespace Imposto.Core
{
    [XmlRoot(ElementName = "NotaFiscalItem")]
    public class NotaFiscalItem
    {
        [XmlIgnore]
        public int Id { get; set; }

        [XmlIgnore]
        public int IdNotaFiscal { get; set; }

        [XmlElement(ElementName = "Cfop")]
        public string Cfop { get; set; }

        [XmlElement(ElementName = "TipoIcms")]
        public string TipoIcms { get; set; }

        [XmlElement(ElementName = "BaseIcms")]
        public double BaseIcms { get; set; }

        [XmlElement(ElementName = "AliquotaIcms")]
        public double AliquotaIcms { get; set; }

        [XmlElement(ElementName = "ValorIcms")]
        public double ValorIcms { get; set; }

        [XmlElement(ElementName = "BaseIpi")]
        public double BaseIpi { get; set; }

        [XmlElement(ElementName = "AliquotaIpi")]
        public double AliquotaIpi { get; set; }

        [XmlElement(ElementName = "ValorIpi")]
        public double ValorIpi { get; set; }

        [XmlElement(ElementName = "Desconto")]
        public double Desconto { get; set; }

        [XmlElement(ElementName = "NomeProduto")]
        public string NomeProduto { get; set; }

        [XmlElement(ElementName = "CodigoProduto")]
        public string CodigoProduto { get; set; }
    }
}
