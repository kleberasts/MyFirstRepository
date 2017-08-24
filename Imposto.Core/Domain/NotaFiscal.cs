using Imposto.Core.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace Imposto.Core
{
    [XmlRoot(ElementName = "NotaFiscal")]
    public class NotaFiscal
    {
        [XmlIgnore]
        public int Id { get; set; }

        [XmlElement(ElementName = "NumeroNotaFiscal")]
        public int NumeroNotaFiscal { get; set; }

        [XmlElement(ElementName = "Serie")]
        public int Serie { get; set; }

        [XmlElement(ElementName = "NomeCliente")]
        public string NomeCliente { get; set; }

        [XmlElement(ElementName = "EstadoDestino")]
        public string EstadoDestino { get; set; }

        [XmlElement(ElementName = "EstadoOrigem")]
        public string EstadoOrigem { get; set; }

        [XmlArray(ElementName = "ItensDaNotaFiscal")]
        public List<NotaFiscalItem> ItensDaNotaFiscal { get; set; }

        public NotaFiscal()
        {
            ItensDaNotaFiscal = new List<NotaFiscalItem>();
        }

        internal void EmitirNotaFiscal(Pedido pedido)
        {
            this.NumeroNotaFiscal = 99999;
            this.Serie = new Random().Next(Int32.MaxValue);
            this.NomeCliente = pedido.NomeCliente;

            this.EstadoDestino = pedido.EstadoDestino;
            this.EstadoOrigem = pedido.EstadoOrigem;

            foreach (PedidoItem itemPedido in pedido.ItensDoPedido)
            {
                NotaFiscalItem notaFiscalItem = new NotaFiscalItem();

                //Retorna qual a CFOP da Nota Fiscal
                notaFiscalItem.Cfop = CalcularCfop();

                if (this.EstadoDestino == this.EstadoOrigem)
                {
                    notaFiscalItem.TipoIcms = "60";
                    notaFiscalItem.AliquotaIcms = 0.18;
                }
                else
                {
                    notaFiscalItem.TipoIcms = "10";
                    notaFiscalItem.AliquotaIcms = 0.17;
                }

                //Se o destino do pedido é algum estado da região sudeste, concede 10% de desconto.
                if (this.EstadoDestino == "SP" || this.EstadoDestino == "RJ" || this.EstadoDestino == "MG" || this.EstadoDestino == "ES")
                    notaFiscalItem.Desconto = 10;

                if (notaFiscalItem.Cfop == "6.009")
                {
                    notaFiscalItem.BaseIcms = itemPedido.ValorItemPedido * 0.90; //redução de base
                }
                else
                {
                    notaFiscalItem.BaseIcms = itemPedido.ValorItemPedido;
                }

                notaFiscalItem.BaseIpi = itemPedido.ValorItemPedido;
                notaFiscalItem.AliquotaIpi = 10;

                if (itemPedido.Brinde)
                {
                    notaFiscalItem.TipoIcms = "60";
                    notaFiscalItem.AliquotaIcms = 0.18;

                    notaFiscalItem.AliquotaIpi = 0;
                }

                notaFiscalItem.ValorIcms = notaFiscalItem.BaseIcms * notaFiscalItem.AliquotaIcms;
                notaFiscalItem.ValorIpi = notaFiscalItem.BaseIpi * notaFiscalItem.AliquotaIpi;

                notaFiscalItem.NomeProduto = itemPedido.NomeProduto;
                notaFiscalItem.CodigoProduto = itemPedido.CodigoProduto;

                this.ItensDaNotaFiscal.Add(notaFiscalItem);
            }

            string msgRetorno = string.Empty;
            string idTemp = new Random().Next(100000, 999999).ToString();

            if (GerarXmlNF(idTemp, out msgRetorno))
            {
                new NotaFiscalRepository().Save(this);
                RenomearNfTemp(idTemp, this.Id.ToString());
            }

        }

        internal List<NotaFiscal> ConsultarNotaFiscalPorCliente(string nomeCliente)
        {
            return new NotaFiscalRepository().GetByCostumerName(nomeCliente);
        }

        #region Geração arquivo NF
        private bool GerarXmlNF(string idTemporario, out string msgRetorno)
        {
            bool retorno;

            try
            {
                string xml = Util.Util.Serialize<NotaFiscal>(this);
                XmlDocument xNF = new XmlDocument();
                xNF.LoadXml(xml);

                string caminhoGravaNF = ConfigurationManager.AppSettings["CaminhoGravarNF"];

                //Validando se a TAG CaminhoGravarNF está configurada no arquivo App.config
                if (string.IsNullOrEmpty(caminhoGravaNF))
                {
                    msgRetorno = "O caminho para gravação da Nota Fiscal não está configurado no arquivo App.Config. Por favor, entre em contato com o time de infraestrutura e solicite a configuração.";
                    return false;
                }

                if (!System.IO.Directory.Exists(caminhoGravaNF))
                    System.IO.Directory.CreateDirectory(caminhoGravaNF);

                xNF.Save(string.Concat(caminhoGravaNF, "NF", idTemporario, ".xml"));

                msgRetorno = "XML da Nota Fiscal gravado com sucesso!";

                retorno = true;
            }
            catch (Exception e)
            {
                msgRetorno = "Houve um erro ao gerar o XML da Nota Fiscal. Erro retornado: " + e.Message;
                retorno = false;
            }

            return retorno;
        }

        private void RenomearNfTemp(string idTemporario, string idNf)
        {
            string caminhoGravaNF = ConfigurationManager.AppSettings["CaminhoGravarNF"];

            System.IO.File.Copy(string.Concat(caminhoGravaNF, "NF", idTemporario, ".xml"), string.Concat(caminhoGravaNF, "NF", idNf, ".xml"));
            System.IO.File.Delete(string.Concat(caminhoGravaNF, "NF", idTemporario, ".xml"));
        }
        #endregion Geração arquivo NF

        #region Métodos Auxiliares
        private string CalcularCfop()
        {
            string cfop = string.Empty;

            if ((this.EstadoOrigem == "SP") && (this.EstadoDestino == "RJ"))
            {
                cfop = "6.000";
            }
            else if ((this.EstadoOrigem == "SP") && (this.EstadoDestino == "PE"))
            {
                cfop = "6.001";
            }
            else if ((this.EstadoOrigem == "SP") && (this.EstadoDestino == "MG"))
            {
                cfop = "6.002";
            }
            else if ((this.EstadoOrigem == "SP") && (this.EstadoDestino == "PB"))
            {
                cfop = "6.003";
            }
            else if ((this.EstadoOrigem == "SP") && (this.EstadoDestino == "PR"))
            {
                cfop = "6.004";
            }
            else if ((this.EstadoOrigem == "SP") && (this.EstadoDestino == "PI"))
            {
                cfop = "6.005";
            }
            else if ((this.EstadoOrigem == "SP") && (this.EstadoDestino == "RO"))
            {
                cfop = "6.006";
            }
            else if ((this.EstadoOrigem == "SP") && (this.EstadoDestino == "SE"))
            {
                cfop = "6.007";
            }
            else if ((this.EstadoOrigem == "SP") && (this.EstadoDestino == "TO"))
            {
                cfop = "6.008";
            }
            else if ((this.EstadoOrigem == "SP") && (this.EstadoDestino == "SE"))
            {
                cfop = "6.009";
            }
            else if ((this.EstadoOrigem == "SP") && (this.EstadoDestino == "PA"))
            {
                cfop = "6.010";
            }
            else if ((this.EstadoOrigem == "MG") && (this.EstadoDestino == "RJ"))
            {
                cfop = "6.000";
            }
            else if ((this.EstadoOrigem == "MG") && (this.EstadoDestino == "PE"))
            {
                cfop = "6.001";
            }
            else if ((this.EstadoOrigem == "MG") && (this.EstadoDestino == "MG"))
            {
                cfop = "6.002";
            }
            else if ((this.EstadoOrigem == "MG") && (this.EstadoDestino == "PB"))
            {
                cfop = "6.003";
            }
            else if ((this.EstadoOrigem == "MG") && (this.EstadoDestino == "PR"))
            {
                cfop = "6.004";
            }
            else if ((this.EstadoOrigem == "MG") && (this.EstadoDestino == "PI"))
            {
                cfop = "6.005";
            }
            else if ((this.EstadoOrigem == "MG") && (this.EstadoDestino == "RO"))
            {
                cfop = "6.006";
            }
            else if ((this.EstadoOrigem == "MG") && (this.EstadoDestino == "SE"))
            {
                cfop = "6.007";
            }
            else if ((this.EstadoOrigem == "MG") && (this.EstadoDestino == "TO"))
            {
                cfop = "6.008";
            }
            else if ((this.EstadoOrigem == "MG") && (this.EstadoDestino == "SE"))
            {
                cfop = "6.009";
            }
            else if ((this.EstadoOrigem == "MG") && (this.EstadoDestino == "PA"))
            {
                cfop = "6.010";
            }

            return cfop;
        }
        #endregion
    }
}
