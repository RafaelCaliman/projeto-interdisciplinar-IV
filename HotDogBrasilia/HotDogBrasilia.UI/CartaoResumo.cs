using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HotDogBrasilia.UI
{
    // Cartão de indicador usado no resumo dos relatórios:
    // faixa colorida à esquerda, título pequeno e valor grande.
    public class CartaoResumo : Control
    {
        private string _titulo = "Indicador";
        private string _valor = "0";
        private Color _corDestaque = Tema.Vermelho;
        private Color _corValor = Tema.TextoEscuro;

        public CartaoResumo()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            BackColor = Tema.FundoClaro;
            Size = new Size(240, 72);
            TabStop = false;
        }

        [Category("Aparência")]
        public string Titulo
        {
            get { return _titulo; }
            set { _titulo = value; AccessibleName = value; Invalidate(); }
        }

        [Category("Aparência")]
        public string Valor
        {
            get { return _valor; }
            set { _valor = value; AccessibleDescription = value; Invalidate(); }
        }

        [Category("Aparência")]
        public Color CorDestaque
        {
            get { return _corDestaque; }
            set { _corDestaque = value; Invalidate(); }
        }

        [Category("Aparência")]
        public Color CorValor
        {
            get { return _corValor; }
            set { _corValor = value; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(BackColor);

            using (var faixa = new SolidBrush(_corDestaque))
                g.FillRectangle(faixa, 0, 0, 5, Height);
            using (var borda = new Pen(Tema.Borda))
                g.DrawRectangle(borda, 0, 0, Width - 1, Height - 1);

            using (var fTitulo = new Font("Segoe UI", 9F))
            using (var fValor = new Font("Segoe UI", 15F, FontStyle.Bold))
            {
                TextRenderer.DrawText(g, _titulo, fTitulo, new Rectangle(16, 8, Width - 22, 20),
                    Color.DimGray, TextFormatFlags.EndEllipsis | TextFormatFlags.Left);
                TextRenderer.DrawText(g, _valor, fValor, new Rectangle(14, 28, Width - 20, Height - 32),
                    _corValor, TextFormatFlags.EndEllipsis | TextFormatFlags.Left);
            }
        }
    }
}
