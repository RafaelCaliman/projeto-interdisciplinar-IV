using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HotDogBrasilia.Domain
{
    public static class Sessao
    {
        public static Usuario Usuario { get; private set; }
        private static HashSet<string> permissao = new HashSet<string>(StringComparer.OrdinalIgnoreCase);


        public static void Iniciar(Usuario usuario, IEnumerable<string> codigos)
        {
            
            permissao.Clear();
            Usuario = usuario;
            permissao = new HashSet<string>(codigos , StringComparer.OrdinalIgnoreCase);

        }
        public static bool TemPermissao(string codigo)
        {
            return permissao.Contains(codigo);
        }
        public static void Encerrar()
        {
            Usuario = null;
            permissao.Clear();
        }
    }
}
