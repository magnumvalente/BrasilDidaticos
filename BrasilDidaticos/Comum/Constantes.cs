﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrasilDidaticos.Comum
{
    static class Constantes
    {
        public const string PERMISSAO_CRIAR = "CRIAR";
        public const string PERMISSAO_MODIFICAR = "MODIFICAR";
        public const string PERMISSAO_CONSULTAR = "CONSULTAR";
        public const string PERMISSAO_IMPORTAR = "IMPORTAR";
        public const string VER_CUSTO = "VER_CUSTO";
        public const string PERMISSAO_DESBLOQUEAR_USUARIO = "DESBLOQUEAR_USUARIO";

        public const string TELA_PRINCIPAL = "PRINCIPAL";
        public const string TELA_USUARIO = "USUARIO";
        public const string TELA_PERFIL = "PERFIL";
        public const string TELA_FORNECEDOR = "FORNECEDOR";
        public const string TELA_ORCAMENTO = "ORCAMENTO";
        public const string TELA_PEDIDO = "PEDIDO";
        public const string TELA_PRODUTO = "PRODUTO";
        public const string TELA_TAXA = "TAXA";
        public const string TELA_UNIDADE_MEDIDA = "UNIDADE_MEDIDA";
        public const string TELA_ESTOQUE = "ESTOQUE";
        public const string TELA_PARAMETRO = "PARAMETRO";
        public const string TELA_CLIENTE = "CLIENTE";
        public const string TELA_SESSAO = "SESSAO";

        public const string RELATORIO_VAREJO = "RELATORIO_VAREJO";
        public const string RELATORIO_ATACADO = "RELATORIO_ATACADO";

        public const string PARAMETRO_DEC_VAREJO = "DEC_VAREJO";
        public const string PARAMETRO_DEC_ATACADO = "DEC_ATACADO";
        public const string PARAMETRO_QTD_ITENS_PAGINA = "QTD_ITENS_PAGINA";
        public const string PARAMETRO_COD_PERFIL_VENDEDOR = "COD_PERFIL_VENDEDOR";
        public const string PARAMETRO_COD_PERFIL_ORCAMENTISTA = "COD_PERFIL_ORCAMENTISTA";
        public const string PARAMETRO_COD_EMPRESA_PRODUTO = "COD_EMPRESA_PRODUTO";
        public const string PARAMETRO_NUM_VALIDADE_ORCAMENTO = "NUM_VALIDADE_ORCAMENTO";
        public const string PARAMETRO_NUM_PRAZO_ENTREGA = "NUM_PRAZO_ENTREGA";
        public const string PARAMETRO_COR_PRIMARIA_FUNDO = "COR_PRIMARIA_FUNDO";
        public const string PARAMETRO_COR_SECUNDARIA_FUNDO = "COR_SECUNDARIA_FUNDO";

        public const int QTD_ITENS_PAGINA = 50;
        public const int NUM_VALIDADE_ORCAMENTO = 60;
        public const int NUM_PRAZO_ENTREGA = 30;
        public const string COR_PRIMARIA_FUNDO = "#EB0047E4";
        public const string COR_SECUNDARIA_FUNDO = "#FFF8FFFF";

        public const string CEP_CODIGO_FILIACAO = "A1C70368-E6DC-4D1A-B562-46004AA53408";

        public const string STRING_FORMAT_MOEDA = "C2";
        public const string STRING_FORMAT_DECIMAL = "F2";
        public const string STRING_FORMAT_PORCENTAGEM = "P2";

        public const string AMBIENTE_DESENVOLVIMENTO = "D_";
        public const string AMBIENTE_HOMOLOGACAO = "H_";
        public const string AMBIENTE_PRODUCAO = "P_";

        public const string NOME_END_POINT = "{0}BasicHttpBinding_IBrasilDidaticos";
    }
}
