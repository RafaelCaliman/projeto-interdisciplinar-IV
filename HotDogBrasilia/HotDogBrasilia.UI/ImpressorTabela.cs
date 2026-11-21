using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Windows.Forms;

namespace HotDogBrasilia.UI
{
    // Imprime qualquer DataTable com título, subtítulo e número de página.
    // Abre a pré-visualização; de lá o usuário imprime ou escolhe
    // "Microsoft Print to PDF" para gerar o PDF.
    public class ImpressorTabela
    {
        private static readonly CultureInfo Br = new CultureInfo("pt-BR", false);

        private readonly DataTable _t;
        private readonly string _titulo, _sub;
        private int _linha, _pagina;

        public ImpressorTabela(DataTable t, string titulo, string sub)
        {
            _t = t; _titulo = titulo; _sub = sub;
        }

        public void Mostrar(IWin32Window dono)
        {
            _linha = 0; _pagina = 0;
            using (var doc = new PrintDocument())
            {
                doc.DocumentName = _titulo;
                doc.DefaultPageSettings.Landscape = _t.Columns.Count > 5;
                doc.BeginPrint += (s, e) => { _linha = 0; _pagina = 0; };
                doc.PrintPage += Pagina;
                using (var prev = new PrintPreviewDialog { Document = doc })
                {
                    prev.Text = "Hot-dog Brasília — Pré-visualização";
                    prev.Width = 1000; prev.Height = 700;
                    prev.StartPosition = FormStartPosition.CenterParent;
                    prev.UseAntiAlias = true;
                    prev.ShowDialog(dono);
                }
            }
        }

        private void Pagina(object sender, PrintPageEventArgs e)
        {
            var g = e.Graphics; var a = e.MarginBounds;
            var fmt = new StringFormat
            {
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap,
                LineAlignment = StringAlignment.Center
            };
            var fmtDir = (StringFormat)fmt.Clone();
            fmtDir.Alignment = StringAlignment.Far;

            using (var fT = new Font("Segoe UI", 14, FontStyle.Bold))
            using (var fN = new Font("Segoe UI", 9))
            using (var fB = new Font("Segoe UI", 9, FontStyle.Bold))
            using (var faixa = new SolidBrush(Tema.Vermelho))
            using (var cinza = new SolidBrush(Tema.FundoClaro))
            {
                float y = a.Top;
                g.FillRectangle(faixa, a.Left, y, a.Width, 4);
                y += 8;
                g.DrawString(_titulo, fT, Brushes.Black, a.Left, y);
                y += fT.GetHeight(g) + 2;
                g.DrawString(_sub, fN, Brushes.DimGray, a.Left, y);
                y += fN.GetHeight(g) + 12;

                float w = a.Width / (float)_t.Columns.Count, h = fN.GetHeight(g) + 8;
                g.FillRectangle(cinza, a.Left, y, a.Width, h);
                for (int c = 0; c < _t.Columns.Count; c++)
                    g.DrawString(_t.Columns[c].ColumnName, fB, Brushes.Black,
                        new RectangleF(a.Left + c * w + 3, y, w - 6, h),
                        EhNumero(_t.Columns[c]) ? fmtDir : fmt);
                y += h;
                g.DrawLine(Pens.Black, a.Left, y, a.Right, y);

                if (_t.Rows.Count == 0)
                {
                    g.DrawString("Nenhum registro encontrado.", fN, Brushes.DimGray, a.Left + 3, y + 6);
                }

                while (_linha < _t.Rows.Count && y + h < a.Bottom - 20)
                {
                    for (int c = 0; c < _t.Columns.Count; c++)
                        g.DrawString(Texto(_t.Rows[_linha][c]), fN, Brushes.Black,
                            new RectangleF(a.Left + c * w + 3, y, w - 6, h),
                            EhNumero(_t.Columns[c]) ? fmtDir : fmt);
                    _linha++; y += h;
                    g.DrawLine(Pens.Gainsboro, a.Left, y, a.Right, y);
                }

                _pagina++;
                g.DrawString("Página " + _pagina, fN, Brushes.Gray,
                    new RectangleF(a.Left, a.Bottom, a.Width, fN.GetHeight(g) + 4), fmtDir);
            }
            e.HasMorePages = _linha < _t.Rows.Count;
        }

        private static bool EhNumero(DataColumn c)
        {
            return c.DataType == typeof(decimal) || c.DataType == typeof(int);
        }

        // Mesma formatação da tela e do CSV (moeda e data em pt-BR).
        private static string Texto(object v)
        {
            if (v == null || v == DBNull.Value) return "";
            if (v is decimal) return ((decimal)v).ToString("C2", Br);
            if (v is DateTime) return ((DateTime)v).ToString("dd/MM/yyyy HH:mm", Br);
            return Convert.ToString(v, Br);
        }
    }
}
