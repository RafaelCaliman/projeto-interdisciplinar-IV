using System.Windows.Forms;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.UI
{
    public static class ControleAcesso
    {
        public static bool Verificar(string codigo)
        {
            if (Sessao.TemPermissao(codigo))
                return true;

            MessageBox.Show("Você não tem permissão para esta ação.", "Acesso negado",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
    }
}
