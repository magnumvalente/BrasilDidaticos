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
            Juridica,
        }

        public enum TipoOrcamento
        {
            Varejo,
            Atacado,
        }

        public enum TipoParametro
        { 
            Binario,
            Texto,
            Inteiro,
            Decimal,
            Percentagem,
            DataHora
        }
    }
}
