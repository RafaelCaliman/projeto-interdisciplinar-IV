using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotDogBrasilia.Domain
{
    public class ResumoFinanceiro
    {
        public decimal Recebido { get; set; }
        public decimal AReceber { get; set; }
        public decimal Pago { get; set; }
        public decimal APagar { get; set; }
        public decimal SaldoRealizado { get; set; }
        public decimal SaldoPrevisto { get; set; }
    }
}
