using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrasilDidaticos.Contrato
{
    public static class Constantes
    {
        public const string NOME_EMPRESA = "Brasil Didáticos";

        public const int COD_RETORNO_SUCESSO = 0;
        public const int COD_FILTRO_VAZIO = 100;
        public const int COD_FILTRO_INVALIDO = 101;
        public const int COD_RETORNO_VAZIO = 200;
        public const int COD_REGISTRO_DUPLICADO = 300;
        public const int COD_EMPRESA_INEXISTENTE = 400;
        public const int COD_EMPRESA_DESATIVADA = 401;

        public const string TIPO_COD_PRODUTO = "PRDT";
        public const string TIPO_COD_FORNECEDOR = "FRNC";
        public const string TIPO_COD_CLIENTE = "CLNT";
        public const string TIPO_COD_ORCAMENTO = "ORCM";
    }
}
