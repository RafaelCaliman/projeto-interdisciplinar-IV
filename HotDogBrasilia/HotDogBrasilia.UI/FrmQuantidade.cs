using System;
using System.Windows.Forms;

namespace HotDogBrasilia.UI
{
    public partial class FrmQuantidade : Form
    {
        public int Valor { get { return (int)nudValor.Value; } }

        public FrmQuantidade(string titulo, string pergunta, int minimo, int valorInicial)
        {
            InitializeComponent();
            Text = titulo;
            lblPergunta.Text = pergunta;
            nudValor.Minimum = minimo;
            nudValor.Value = Math.Max(minimo, Math.Min(valorInicial, (int)nudValor.Maximum));
        }

        private void FrmQuantidade_Shown(object sender, EventArgs e)
        {
            nudValor.Focus();
            nudValor.Select(0, nudValor.Text.Length);
        }
    }
}
