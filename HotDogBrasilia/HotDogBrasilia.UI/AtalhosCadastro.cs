using System;
using System.Linq;
using System.Windows.Forms;

namespace HotDogBrasilia.UI
{
    public static class AtalhosCadastro
    {
        public static bool Processar(Form tela, Keys tecla, Button novo, Button salvar,
            Button excluir, Action atualizar, params Control[] camposDoFormulario)
        {
            switch (tecla)
            {
                case Keys.Control | Keys.N:
                    return Clicar(novo);
                case Keys.Control | Keys.S:
                    return Clicar(salvar);
                case Keys.Enter:
                    return camposDoFormulario.Contains(tela.ActiveControl) && Clicar(salvar);
                case Keys.F5:
                    if (atualizar == null) return false;
                    atualizar();
                    return true;
                case Keys.Delete:
                    if (EmCampoDeTexto(tela.ActiveControl)) return false;
                    return Clicar(excluir);
                case Keys.Escape:
                    tela.Close();
                    return true;
            }
            return false;
        }

        private static bool EmCampoDeTexto(Control c)
        {
            return c is TextBoxBase || c is NumericUpDown || c is ComboBox || c is UpDownBase;
        }

        private static bool Clicar(Button b)
        {
            if (b == null || !b.Enabled || !b.Visible) return false;
            b.PerformClick();
            return true;
        }
    }
}
