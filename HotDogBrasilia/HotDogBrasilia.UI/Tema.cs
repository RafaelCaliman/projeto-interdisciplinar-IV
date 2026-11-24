using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HotDogBrasilia.UI
{
    public static class Tema
    {
        public static readonly Color Vermelho = Color.FromArgb(196, 48, 43);
        public static readonly Color VermelhoEscuro = Color.FromArgb(140, 30, 28);
        public static readonly Color Creme = Color.FromArgb(255, 224, 178);
        public static readonly Color FundoClaro = Color.FromArgb(248, 246, 244);
        public static readonly Color Borda = Color.FromArgb(220, 220, 220);
        public static readonly Color TextoEscuro = Color.FromArgb(64, 64, 64);
        public static readonly Color Sucesso = Color.FromArgb(27, 110, 46);
        public static readonly Color Erro = Color.FromArgb(160, 30, 30);
        public static readonly Color AmbarFundo = Color.FromArgb(255, 236, 179);
        public static readonly Color VermelhoFundo = Color.FromArgb(255, 205, 205);

        public static void Grade(DataGridView g)
        {
            g.ReadOnly = true;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.AllowUserToResizeRows = false;
            g.MultiSelect = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.RowHeadersVisible = false;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.BackgroundColor = Color.White;
            g.BorderStyle = BorderStyle.None;
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.GridColor = Borda;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 34;
            g.ColumnHeadersDefaultCellStyle.BackColor = FundoClaro;
            g.ColumnHeadersDefaultCellStyle.ForeColor = TextoEscuro;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = FundoClaro;
            g.RowTemplate.Height = 28;
            g.DefaultCellStyle.SelectionBackColor = Creme;
            g.DefaultCellStyle.SelectionForeColor = Color.Black;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(252, 251, 250);
        }

        public static void BotaoPrimario(Button b)
        {
            Botao(b, Vermelho, Color.White, Vermelho,
                Color.FromArgb(165, 38, 34), VermelhoEscuro, true);
        }

        public static void BotaoSecundario(Button b)
        {
            Botao(b, Color.White, Vermelho, Vermelho,
                Color.FromArgb(255, 240, 238), Color.FromArgb(255, 224, 220), false);
        }

        public static void BotaoNeutro(Button b)
        {
            Botao(b, Color.White, TextoEscuro, Color.FromArgb(200, 200, 200),
                Color.FromArgb(245, 245, 245), Color.FromArgb(230, 230, 230), false);
        }

        private static void Botao(Button b, Color fundo, Color texto, Color borda,
            Color sobre, Color pressionado, bool negrito)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.UseVisualStyleBackColor = false;
            b.ForeColor = texto;
            b.FlatAppearance.BorderSize = 1;
            b.FlatAppearance.MouseOverBackColor = sobre;
            b.FlatAppearance.MouseDownBackColor = pressionado;
            if (negrito) b.Font = new Font(b.Font, FontStyle.Bold);

            System.EventHandler aplicar = (s, e) =>
            {
                b.BackColor = b.Enabled ? fundo : Color.FromArgb(235, 235, 235);
                b.FlatAppearance.BorderColor = b.Enabled ? borda : Borda;
                b.Cursor = b.Enabled ? Cursors.Hand : Cursors.Default;
            };
            b.EnabledChanged += aplicar;
            aplicar(b, System.EventArgs.Empty);
        }

        public static void ColunaMoeda(DataGridViewColumn c)
        {
            c.DefaultCellStyle.Format = "C2";
            c.DefaultCellStyle.FormatProvider = new System.Globalization.CultureInfo("pt-BR", false);
            c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern System.IntPtr SendMessage(System.IntPtr h, int msg, int wp, string lp);
        private const int EM_SETCUEBANNER = 0x1501;

        public static void Dica(TextBox caixa, string texto)
        {
            SendMessage(caixa.Handle, EM_SETCUEBANNER, 1, texto);
        }
    }
}
