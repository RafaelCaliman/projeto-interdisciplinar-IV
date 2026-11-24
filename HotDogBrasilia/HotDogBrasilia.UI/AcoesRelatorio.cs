using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.UI
{
    public static class AcoesRelatorio
    {
        public static string Subtitulo(string filtros)
        {
            string usuario = Sessao.Usuario != null ? Sessao.Usuario.Login : "";
            return filtros + " · Gerado em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                + " por " + usuario;
        }

        public static string TextoPeriodo(DateTimePicker ini, DateTimePicker fim)
        {
            if (!ini.Checked && !fim.Checked) return "Período: todos";
            string de = ini.Checked ? ini.Value.ToString("dd/MM/yyyy") : "início";
            string ate = fim.Checked ? fim.Value.ToString("dd/MM/yyyy") : "hoje";
            return "Período: " + de + " a " + ate;
        }

        public static string ExportarCsv(IWin32Window dono, DataTable t, string nomeBase,
                                         string tipo, string filtros)
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Title = "Exportar relatório em CSV";
                dlg.Filter = "Arquivo CSV (*.csv)|*.csv";
                dlg.FileName = nomeBase + "_" + DateTime.Today.ToString("yyyy-MM-dd") + ".csv";
                dlg.OverwritePrompt = true;
                if (dlg.ShowDialog(dono) != DialogResult.OK) return null;

                try
                {
                    ExportadorCsv.Salvar(dlg.FileName, t);
                }
                catch (IOException)
                {
                    MessageBox.Show(dono, "Não foi possível gravar o arquivo.\n"
                        + "Se ele estiver aberto no Excel, feche-o e tente de novo.",
                        "Exportar CSV", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show(dono, "Sem permissão para gravar nessa pasta.\n"
                        + "Escolha outra pasta (ex.: Documentos).",
                        "Exportar CSV", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }

                Auditoria.Registrar("RELATORIO_EXPORTAR", "relatorio", null, tipo + " | " + filtros);
                return dlg.FileName;
            }
        }

        public static void Imprimir(IWin32Window dono, DataTable t, string titulo,
                                    string tipo, string filtros)
        {
            new ImpressorTabela(t, titulo, Subtitulo(filtros)).Mostrar(dono);
            Auditoria.Registrar("RELATORIO_IMPRIMIR", "relatorio", null, tipo + " | " + filtros);
        }
    }
}
