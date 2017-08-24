using System.Collections.Generic;

namespace Imposto.Core
{
    public class NotaFiscalService
    {
        public void GerarNotaFiscal(Pedido pedido)
        {
            NotaFiscal notaFiscal = new NotaFiscal();
            notaFiscal.EmitirNotaFiscal(pedido);
        }

        public List<NotaFiscal> ConsultarNotaFiscalPorCliente(string nomeCliente)
        {
            return new NotaFiscal().ConsultarNotaFiscalPorCliente(nomeCliente);
        }
    }
}
