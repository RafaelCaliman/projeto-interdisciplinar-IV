using System;

namespace HotDogBrasilia.Domain
{
    public class RegistroLog
    {
        public int IdLog { get; set; }
        public DateTime DataHora { get; set; }
        public string Usuario { get; set; }
        public string Acao { get; set; }
        public string Entidade { get; set; }
        public int? IdRegistro { get; set; }
        public string Detalhe { get; set; }
    }
}
