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
        // O Program.cs lê esta propriedade para saber se volta para o login.
        public bool TrocarUsuario { get; private set; }

        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = "Hot-dog Brasília — " + Sessao.Usuario.Nome;
            AplicarPermissoes();
        }

        private void mnuTrocarUsuario_Click(object sender, EventArgs e)
        {
            TrocarUsuario = true;
            Close();
        }

        private void mnuSair_Click(object sender, EventArgs e)
        {
            Close();
        }

        // Vale para "Sair", "Trocar usuário" e para o X da janela:
        // registra a saída e limpa a sessão do usuário.
        private void FrmPrincipal_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Sessao.Usuario == null) return;

            Auditoria.Registrar("LOGOUT", "usuario", Sessao.Usuario.IdUser, null);
            Sessao.Encerrar();
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
