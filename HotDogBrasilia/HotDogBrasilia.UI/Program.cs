using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotDogBrasilia.UI
{
    internal static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Rede de segurança: qualquer erro não tratado nas telas vira uma
            // mensagem, em vez de fechar o programa.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => Erros.Mostrar(e.Exception);

            // Erros fora das telas (outras threads): ao menos avisa o usuário.
            AppDomain.CurrentDomain.UnhandledException +=
                (s, e) => Erros.Mostrar(e.ExceptionObject as Exception);

            // Repete enquanto o usuário escolher "Trocar usuário".
            while (true)
            {
                using (FrmLogin login = new FrmLogin())
                {
                    if (login.ShowDialog() != DialogResult.OK)
                        return;
                }

                using (FrmPrincipal principal = new FrmPrincipal())
                {
                    Application.Run(principal);
                    if (!principal.TrocarUsuario)
                        return;
                }
            }
        }
    }
}
