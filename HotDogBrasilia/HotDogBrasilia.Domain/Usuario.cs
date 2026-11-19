using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotDogBrasilia.Domain
{
    public class Usuario
    {
        public int IdUser { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public int IdPerfil { get; set; }
        public string Perfil { get; set; }
        public bool Ativo { get; set; }
        public DateTime? UltimoAcesso { get; set;}

    }
}
