using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrasilDidaticos.Comum
{
    public static class Parametros
    {
        public static decimal PercentagemVarejo
        {
            get;
            set;
        }

        public static decimal PercentagemAtacado
        {
            get;
            set;
        }

        public static int QuantidadeItensPagina
        {
            get;
            set;
        }

        public static string CodigoPerfilVendedor
        {
            get;
            set;
        }

        public static string CodigoPerfilOrcamentista
        {
            get;
            set;
        }

        public static void CarregarParametros()
        {
            Contrato.EntradaParametro entradaParametro = new Contrato.EntradaParametro();
            entradaParametro.Chave = Comum.Util.Chave;
            entradaParametro.UsuarioLogado = Comum.Util.UsuarioLogado.Login;            

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
            Contrato.RetornoParametro retParametro = servBrasilDidaticos.ParametroListar(entradaParametro);
            servBrasilDidaticos.Close();

            if (retParametro.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                foreach (Contrato.Parametro parametro in retParametro.Parametros)
                {
                    if (!string.IsNullOrWhiteSpace(parametro.Valor))
                    {
                        switch (parametro.Codigo)
                        {
                            case Constantes.PARAMETRO_DEC_ATACADO:
                                PercentagemAtacado = decimal.Parse(parametro.Valor) / 100;
                                break;
                            case Constantes.PARAMETRO_DEC_VAREJO:
                                PercentagemVarejo = decimal.Parse(parametro.Valor) / 100;
                                break;
                            case Constantes.PARAMETRO_QTD_ITENS_PAGINA:
                                QuantidadeItensPagina = int.Parse(parametro.Valor);
                                break;
                            case Constantes.PARAMETRO_COD_PERFIL_VENDEDOR:
                                CodigoPerfilVendedor = parametro.Valor;
                                break;
                            case Constantes.PARAMETRO_COD_PERFIL_ORCAMENTISTA:
                                CodigoPerfilOrcamentista = parametro.Valor;
                                break;
                        }
                    }
                }
            }
        }
    }
}
