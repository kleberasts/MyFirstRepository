using Imposto.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TesteImposto.UnitTest
{
    [TestClass]
    public class NotaFiscalTeste
    {
        [TestMethod]
        public void TestarEmissaoDeNotaFiscal()
        {
            List<PedidoItem> itensPedido = new List<PedidoItem>();

            PedidoItem item1 = new PedidoItem
            {
                CodigoProduto = "1234",
                NomeProduto = "Produto 1",
                ValorItemPedido = 15.9,
                Brinde = false
            };

            PedidoItem item2 = new PedidoItem
            {
                CodigoProduto = "5678",
                NomeProduto = "Produto 2",
                ValorItemPedido = 24.7,
                Brinde = false
            };

            PedidoItem item3 = new PedidoItem
            {
                CodigoProduto = "9012",
                NomeProduto = "Produto 3",
                ValorItemPedido = 98.1,
                Brinde = false
            };

            PedidoItem brinde1 = new PedidoItem
            {
                CodigoProduto = "3456",
                NomeProduto = "Brinde 1",
                ValorItemPedido = 5.0,
                Brinde = true
            };

            itensPedido.Add(item1);
            itensPedido.Add(item2);
            itensPedido.Add(item3);
            itensPedido.Add(brinde1);

            Pedido pedido = new Pedido
            {
                NomeCliente = "Cliente 1",
                EstadoOrigem = "SP",
                EstadoDestino = "RJ",
                ItensDoPedido = itensPedido
            };

            NotaFiscalService service = new NotaFiscalService();
            service.GerarNotaFiscal(pedido);
        }

        [TestMethod]
        public void ConsultarEmissaoDeNotaFiscal()
        {
            NotaFiscalService service = new NotaFiscalService();
            List<NotaFiscal> listaNotas = service.ConsultarNotaFiscalPorCliente("Cliente 1");

            if (!listaNotas.Any())
                throw new ArgumentException("Nenhuma Proposta encontrada.");
        }
    }
}
