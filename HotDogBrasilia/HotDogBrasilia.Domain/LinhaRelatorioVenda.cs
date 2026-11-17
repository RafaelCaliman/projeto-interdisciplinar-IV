using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotDogBrasilia.Domain
{
    public class LinhaRelatorioVenda
    {
        public int IdVenda { get; set; }
        public DateTime DataHora { get; set; }
        public string Cliente { get; set; }
        public decimal Total { get; set; }
    }
}
