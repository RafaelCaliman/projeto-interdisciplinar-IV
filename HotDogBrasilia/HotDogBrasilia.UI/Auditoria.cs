using System;
using System.Diagnostics;
using HotDogBrasilia.Data.Repositories;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.UI
{
    // Grava no log quem fez o quê (a data/hora o banco preenche).
    // Usado pelas telas de cadastro depois que a ação já deu certo.
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
                // O cadastro já foi salvo; uma falha no log não pode desfazer
                // nem assustar o usuário. Fica anotado na janela de Saída do VS.
                Debug.WriteLine("Falha ao gravar log: " + ex.Message);
            }
        }
    }
}
