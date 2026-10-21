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
        }

        private void mnuClientes_Click(object sender, EventArgs e)
        {
            using (var f = new FrmClientes()) { f.ShowDialog(this); }
        }

        private void mnuProdutos_Click(object sender, EventArgs e)
        {
            using (var f = new FrmProdutos()) { f.ShowDialog(this); }
        }
    }
}
