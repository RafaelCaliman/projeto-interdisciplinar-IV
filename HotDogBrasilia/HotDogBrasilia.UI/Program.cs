using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
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

            var ptBr = new CultureInfo("pt-BR", false);
            CultureInfo.DefaultThreadCurrentCulture = ptBr;
            CultureInfo.DefaultThreadCurrentUICulture = ptBr;
            Thread.CurrentThread.CurrentCulture = ptBr;
            Thread.CurrentThread.CurrentUICulture = ptBr;

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => Erros.Mostrar(e.Exception);

            AppDomain.CurrentDomain.UnhandledException +=
                (s, e) => Erros.Mostrar(e.ExceptionObject as Exception);

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
