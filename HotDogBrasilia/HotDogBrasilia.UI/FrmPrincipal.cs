using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotDogBrasilia.Data.Repositories;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.UI
{
    public partial class FrmPrincipal : Form
    {
        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AplicarPermissoes();
        }

        // Esconde os menus que o usuário logado não pode acessar.
        private void AplicarPermissoes()
        {
            bool podeClientes = Sessao.TemPermissao(Permissoes.ClienteVer);
            bool podeProdutos = Sessao.TemPermissao(Permissoes.ProdutoVer);

            mnuClientes.Visible = podeClientes;
            mnuProdutos.Visible = podeProdutos;

            // Se nenhum item de Cadastros sobrou, esconde o menu inteiro.
            mnuCadastros.Visible = podeClientes || podeProdutos;

            bool podeNovaVenda = Sessao.TemPermissao(Permissoes.VendaRegistrar);
            bool podeConsultarVendas = Sessao.TemPermissao(Permissoes.VendaVer);

            mnuNovaVenda.Visible = podeNovaVenda;
            mnuConsultarVendas.Visible = podeConsultarVendas;
            mnuVendas.Visible = podeNovaVenda || podeConsultarVendas;
        }

        private void mnuClientes_Click(object sender, EventArgs e)
        {
            using (var f = new FrmClientes()) { f.ShowDialog(this); }
        }

        private void mnuProdutos_Click(object sender, EventArgs e)
        {
            using (var f = new FrmProdutos()) { f.ShowDialog(this); }
        }

        private void mnuNovaVenda_Click(object sender, EventArgs e)
        {
            using (var f = new FrmVenda()) { f.ShowDialog(this); }
        }

        private void mnuConsultarVendas_Click(object sender, EventArgs e)
        {
            using (var f = new FrmVendas()) { f.ShowDialog(this); }
        }
    }
}
