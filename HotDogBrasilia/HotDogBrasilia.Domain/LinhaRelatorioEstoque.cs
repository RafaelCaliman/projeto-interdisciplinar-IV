using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotDogBrasilia.Domain
{
    public class LinhaRelatorioEstoque
    {
        public string Produto { get; set; }
        public int Estoque { get; set; }
        public string Fornecedor { get; set; }
        public string Situacao { get; set; }

    }
}
