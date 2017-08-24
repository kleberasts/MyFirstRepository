using Imposto.Core;
using Imposto.Core.Util;
using System;
using System.Data;
using System.Windows.Forms;

namespace TesteImposto
{
    public partial class FormImposto : Form
    {
        private Pedido pedido = new Pedido();

        public FormImposto()
        {
            InitializeComponent();
            dataGridViewPedidos.AutoGenerateColumns = true;                       
            dataGridViewPedidos.DataSource = GetTablePedidos();
            ResizeColumns();
        }

        #region Eventos
        private void buttonGerarNotaFiscal_Click(object sender, EventArgs e)
        {
            if (ValidaFormulario())
            {
                NotaFiscalService service = new NotaFiscalService();
                pedido.EstadoOrigem = txtEstadoOrigem.Text;
                pedido.EstadoDestino = txtEstadoDestino.Text;
                pedido.NomeCliente = textBoxNomeCliente.Text;

                DataTable table = (DataTable)dataGridViewPedidos.DataSource;

                foreach (DataRow row in table.Rows)
                {
                    pedido.ItensDoPedido.Add(
                        new PedidoItem()
                        {
                            Brinde = row["Brinde"] != DBNull.Value ? Convert.ToBoolean(row["Brinde"]) : false,
                            CodigoProduto = row["Codigo do produto"].ToString(),
                            NomeProduto = row["Nome do produto"].ToString(),
                            ValorItemPedido = Convert.ToDouble(row["Valor"].ToString())
                        });
                }

                service.GerarNotaFiscal(pedido);
                MessageBox.Show("Operação efetuada com sucesso");
                LimparFormulario();
            }
        }

        private void txtEstado_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                Util.Estado uf;
                if (!Enum.TryParse<Util.Estado>(((TextBox)sender).Text, out uf))
                {
                    MessageBox.Show("Estado não encontrado. Digite um estado válido.");
                    ((TextBox)sender).Text = string.Empty;
                    ((TextBox)sender).Focus();
                }
            }
        }

        void txtEstado_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }
        #endregion Eventos

        #region Métodos Auxiliares
        private void ResizeColumns()
        {
            double mediaWidth = dataGridViewPedidos.Width / dataGridViewPedidos.Columns.GetColumnCount(DataGridViewElementStates.Visible);

            for (int i = dataGridViewPedidos.Columns.Count - 1; i >= 0; i--)
            {
                var coluna = dataGridViewPedidos.Columns[i];
                coluna.Width = Convert.ToInt32(mediaWidth);
            }   
        }

        private object GetTablePedidos()
        {
            DataTable table = new DataTable("pedidos");
            table.Columns.Add(new DataColumn("Nome do produto", typeof(string)));
            table.Columns.Add(new DataColumn("Codigo do produto", typeof(string)));
            table.Columns.Add(new DataColumn("Valor", typeof(decimal)));
            table.Columns.Add(new DataColumn("Brinde", typeof(bool)));
                     
            return table;
        }

        private void LimparFormulario()
        {
            textBoxNomeCliente.Text = string.Empty;
            txtEstadoOrigem.Text = string.Empty;
            txtEstadoDestino.Text = string.Empty;

            pedido = new Pedido();
            dataGridViewPedidos.DataSource = GetTablePedidos();
        }

        private bool ValidaFormulario()
        {
            if (string.IsNullOrEmpty(textBoxNomeCliente.Text))
            {
                MessageBox.Show("Digite o nome do cliente. É necessário para a emissão da Nota Fiscal.");
                textBoxNomeCliente.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtEstadoOrigem.Text))
            {
                MessageBox.Show("Digite o Estado de Origem do produto. É necessário para a o cálculo dos impostos.");
                txtEstadoOrigem.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtEstadoDestino.Text))
            {
                MessageBox.Show("Digite o Estado de Origem do produto. É necessário para a o cálculo dos impostos.");
                txtEstadoDestino.Focus();
                return false;
            }

            if (dataGridViewPedidos.Rows.Count < 2)
            {
                MessageBox.Show("Digite ao menos um item de pedido.");
                return false;
            }

            return true;
        }
        #endregion Método Auxiliares
    }
}