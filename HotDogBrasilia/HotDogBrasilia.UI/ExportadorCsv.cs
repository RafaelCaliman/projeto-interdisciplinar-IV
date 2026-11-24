using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace HotDogBrasilia.UI
{
    public static class ExportadorCsv
    {
        private static readonly CultureInfo Br = new CultureInfo("pt-BR", false);

        public static void Salvar(string caminho, DataTable t)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(";",
                t.Columns.Cast<DataColumn>().Select(c => Esc(c.ColumnName))));
            foreach (DataRow r in t.Rows)
                sb.AppendLine(string.Join(";", r.ItemArray.Select(Fmt)));
            File.WriteAllText(caminho, sb.ToString(), new UTF8Encoding(true));
        }

        private static string Fmt(object v)
        {
            if (v == null || v == DBNull.Value) return "";
            if (v is decimal) return ((decimal)v).ToString("F2", Br);
            if (v is DateTime) return ((DateTime)v).ToString("dd/MM/yyyy HH:mm", Br);
            return Esc(Convert.ToString(v, Br));
        }

        private static string Esc(string s)
        {
            bool precisa = s.IndexOfAny(new[] { ';', '"', '\n', '\r' }) >= 0;
            return precisa ? "\"" + s.Replace("\"", "\"\"") + "\"" : s;
        }
    }
}
