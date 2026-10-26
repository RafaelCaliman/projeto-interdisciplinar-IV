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

            using (FrmLogin login = new FrmLogin())
            {
                if (login.ShowDialog() != DialogResult.OK)
                    return; 
            }
            Application.Run(new FrmPrincipal());
        }
    }
}
