using System;
using System.Diagnostics;
using HotDogBrasilia.Data.Repositories;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.UI
{
    public static class Auditoria
    {
        public static void Registrar(string acao, string entidade, int? idRegistro, string detalhe)
        {
            if (Sessao.Usuario == null) return;

            try
            {
                LogRepository.registrar(Sessao.Usuario.IdUser, acao, entidade, idRegistro, detalhe);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Falha ao gravar log: " + ex.Message);
            }
        }
    }
}
