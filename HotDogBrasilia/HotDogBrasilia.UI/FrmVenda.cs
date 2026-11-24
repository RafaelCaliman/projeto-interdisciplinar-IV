using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using HotDogBrasilia.Data.Repositories;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.UI
{
    public partial class FrmVenda : Form
    {
        private static readonly CultureInfo PtBr = new CultureInfo("pt-BR", false);

        private List<ItemVenda> carrinho = new List<ItemVenda>();

        private int? idClienteBalcao = null;

        public FrmVenda()
        {
            InitializeComponent();
            Tema.Grade(dgvItens);
            Tema.BotaoPrimario(btnFinalizar);
            Tema.BotaoSecundario(btnAdicionar);
            Tema.BotaoNeutro(btnRemover);
            Tema.BotaoNeutro(btnDescartar);
        }

        private void FrmVenda_Load(object sender, EventArgs e)
        {
            cboPagamento.Items.Add("PAGO");
            cboPagamento.Items.Add("PENDENTE");
            cboPagamento.SelectedIndex = 0;

            btnFinalizar.Enabled = Sessao.TemPermissao(Permissoes.VendaRegistrar);
            AtualizarCarrinho();

            try
            {
                cboCliente.DisplayMember = "Nome";
                cboCliente.ValueMember = "Id";
                cboCliente.DataSource = ClienteRepository.listarClientes(null);
                cboCliente.SelectedIndex = -1;

                CarregarProdutos();
            }
            catch (Exception ex)
            {
                Erros.Mostrar(ex);
                return;
            }

            CarregarClienteBalcao();
        }

        private void CarregarClienteBalcao()
        {
            try
            {
                Cliente balcao = ClienteRepository.clienteBalcao();
                idClienteBalcao = balcao == null ? (int?)null : balcao.Id;
            }
            catch (Exception ex)
            {
                Erros.Mostrar(ex);
                return;
            }

            if (idClienteBalcao == null)
            {
                MessageBox.Show("O Cliente balcão não foi encontrado no banco.\n"
                    + "Selecione o cliente da venda na lista.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SelecionarClienteBalcao();
        }

        private void SelecionarClienteBalcao()
        {
            if (idClienteBalcao == null)
                cboCliente.SelectedIndex = -1;
            else
                cboCliente.SelectedValue = idClienteBalcao.Value;
        }

        private void CarregarProdutos()
        {
            cboProduto.DisplayMember = "Nome";
            cboProduto.ValueMember = "IdProduto";
            cboProduto.DataSource = ProdutoRepository.listarProdutos(null);
            cboProduto.SelectedIndex = -1;
            lblDisponivel.Text = "";
        }

        private void AtualizarCarrinho()
        {
            dgvItens.DataSource = null;
            dgvItens.DataSource = new List<ItemVenda>(carrinho);
            if (dgvItens.Columns.Contains("IdItem"))
                dgvItens.Columns["IdItem"].Visible = false;
            if (dgvItens.Columns.Contains("IdProduto"))
                dgvItens.Columns["IdProduto"].Visible = false;
            if (dgvItens.Columns.Contains("NomeProduto"))
                dgvItens.Columns["NomeProduto"].HeaderText = "Produto";
            if (dgvItens.Columns.Contains("ValorUnitario"))
            {
                dgvItens.Columns["ValorUnitario"].HeaderText = "Valor unitário";
                dgvItens.Columns["ValorUnitario"].DefaultCellStyle.Format = "C2";
            }
            if (dgvItens.Columns.Contains("Subtotal"))
                dgvItens.Columns["Subtotal"].DefaultCellStyle.Format = "C2";
            dgvItens.ClearSelection();

            decimal total = 0;
            foreach (ItemVenda item in carrinho)
                total += item.Subtotal;
            lblTotal.Text = "Total: " + total.ToString("C2", PtBr);
        }

        private void NovaVenda()
        {
            carrinho.Clear();
            SelecionarClienteBalcao();
            cboPagamento.SelectedIndex = 0;
            nudQuantidade.Value = 1;
            CarregarProdutos();
            AtualizarCarrinho();
        }

        private void cboProduto_SelectedIndexChanged(object sender, EventArgs e)
        {
            var produto = cboProduto.SelectedItem as Produto;
            if (produto == null)
                lblDisponivel.Text = "";
            else
                lblDisponivel.Text = "Disponível: " + produto.Estoque
                    + "  |  " + produto.Valor.ToString("C2", PtBr);
        }

        private void btnAdicionar_Click(object sender, EventArgs e)
        {
            var produto = cboProduto.SelectedItem as Produto;
            if (produto == null)
            {
                MessageBox.Show("Selecione um produto.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int quantidade = (int)nudQuantidade.Value;

            ItemVenda item = carrinho.Find(i => i.IdProduto == produto.IdProduto);
            if (item == null)
            {
                carrinho.Add(new ItemVenda
                {
                    IdProduto = produto.IdProduto,
                    NomeProduto = produto.Nome,
                    Quantidade = quantidade,
                    ValorUnitario = produto.Valor
                });
            }
            else
            {
                item.Quantidade += quantidade;
            }

            nudQuantidade.Value = 1;
            AtualizarCarrinho();
        }

        private void btnRemover_Click(object sender, EventArgs e)
        {
            if (dgvItens.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um item na lista.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var item = dgvItens.SelectedRows[0].DataBoundItem as ItemVenda;
            carrinho.Remove(item);
            AtualizarCarrinho();
        }

        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.VendaRegistrar)) return;

            try
            {
                int? idCliente = cboCliente.SelectedValue == null
                    ? (int?)null : (int)cboCliente.SelectedValue;

                Venda venda = VendaRepository.registrarVenda(idCliente, carrinho,
                    cboPagamento.Text, Sessao.Usuario.IdUser);

                MessageBox.Show("Venda #" + venda.IdVenda + " registrada.\nTotal: "
                    + venda.Total.ToString("C2", PtBr), "Venda concluída",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                NovaVenda();
            }
            catch (Exception ex)
            {
                Erros.Mostrar(ex);
            }
        }

        private void btnDescartar_Click(object sender, EventArgs e)
        {
            if (carrinho.Count == 0) return;

            var resposta = MessageBox.Show("Descartar os itens desta venda?",
                "Descartar venda", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (resposta == DialogResult.Yes)
                NovaVenda();
        }

        private void FrmVenda_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (carrinho.Count == 0) return;

            var resposta = MessageBox.Show("A venda não foi finalizada. Deseja sair mesmo assim?",
                "Venda em aberto", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (resposta != DialogResult.Yes)
                e.Cancel = true;
        }
    }
}
