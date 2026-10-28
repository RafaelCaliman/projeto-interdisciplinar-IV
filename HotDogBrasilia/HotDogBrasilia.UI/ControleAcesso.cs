using System.Windows.Forms;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.UI
{
    // Ajuda as telas a conferir a permissão antes de executar uma ação.
    public static class ControleAcesso
    {
        // Retorna true se o usuário logado pode; senão avisa e retorna false.
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
