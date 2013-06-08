using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrasilDidaticos.Contrato
{
    public static class Enumeradores
    {
        public enum Pessoa
        {
            Fisica,
            Juridica
        }

        public enum TipoOrcamento
        {
            Varejo,
            Atacado
        }

        public enum TipoParametro
        { 
            Binario,
            Texto,
            Inteiro,
            Decimal,
            Percentagem,
            DataHora,
            Cor
        }

        public enum EstadoOrcamento
        {
            Iniciado = 01,
            EmAndamento = 02,
            Concluido = 03,
            Aprovado = 04,
            Reprovado = 05,
            Reiniciado = 06
        }

        public enum EstadoPedido
        {
            EmAndamento = 01,
            Concluido = 02,
            Aprovado = 03,
            Reprovado = 04,
            Reiniciado = 05
        }
    }
}
