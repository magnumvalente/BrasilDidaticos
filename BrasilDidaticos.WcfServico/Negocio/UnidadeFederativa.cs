using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrasilDidaticos.WcfServico;

namespace BrasilDidaticos.WcfServico.Negocio
{
    internal static class UnidadeFederativa
    {
        /// <summary>
        /// Método para buscar a unidade federativa
        /// </summary>
        /// <param name="codigoUnidade">Código da unidade federativa do cliente</param>
        /// <returns>Contrato.UnidadeFederativa</returns>
        internal static Contrato.UnidadeFederativa BuscarUnidadeFederativa(string codigoUnidade)
        {
            // Objeto que recebe o retorno do método
            Contrato.UnidadeFederativa retUf = new Contrato.UnidadeFederativa();

             // Loga no banco de dados
            Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

            // Recupera a unidade federativa
            Dados.UNIDADE_FEDERATIVA uf = context.T_UNIDADE_FEDERATIVA.FirstOrDefault( ufd => ufd.COD_UNIDADE_FEDERATIVA == codigoUnidade );

            // Verifica se foi encontrado algum registro
            if (uf != null)
            {
                retUf = new Contrato.UnidadeFederativa()
                {
                    Id = uf.ID_UNIDADE_FEDERATIVA,
                    Nome = uf.NOME_UNIDADE_FEDERATIVA,
                    Codigo = uf.COD_UNIDADE_FEDERATIVA
                };
            }

            // retorna os dados
            return retUf;
        }

        /// <summary>
        /// Método para buscar a unidade federativa
        /// </summary>
        /// <param name="codigoUnidade">Código da unidade federativa do cliente</param>
        /// <param name="lstUfs">Lista de unidades federativas</param>
        /// <returns>Contrato.UnidadeFederativa</returns>
        internal static Contrato.UnidadeFederativa BuscarUnidadeFederativa(string codigoUnidade, List<Contrato.UnidadeFederativa> lstUfs)
        {
            // Objeto que recebe o retorno do método
            Contrato.UnidadeFederativa retUf = null;

            // Verifica se existe dados na lista de unidades federativas
            if (lstUfs != null)
            {
                // Recupera a unidade federativa
                retUf = lstUfs.FirstOrDefault(ufd => ufd.Codigo == codigoUnidade);
            }

            // retorna os dados
            return retUf;
        }

        /// <summary>
        /// Método para retornar as unidades federativas
        /// </summary>        
        /// <returns>List<Contrato.UnidadeFederativa></returns>
        internal static Contrato.RetornoUnidadeFederativa ListarUnidadeFederativa()
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoUnidadeFederativa retUnidadeFederativa = new Contrato.RetornoUnidadeFederativa();

            // Loga no banco de dados
            Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

            // Busca o usuario no banco
            List<Contrato.UnidadeFederativa> lstUnidadesFederativas = (from uf in context.T_UNIDADE_FEDERATIVA select new Contrato.UnidadeFederativa
                                                                                                                {
                                                                                                                    Id = uf.ID_UNIDADE_FEDERATIVA,
                                                                                                                    Codigo = uf.COD_UNIDADE_FEDERATIVA,
                                                                                                                    Nome = uf.NOME_UNIDADE_FEDERATIVA
                                                                                                                }).ToList();

            // Verifica se foi encontrado algum registro
            if (lstUnidadesFederativas.Count > 0)
            {
                // Preenche o objeto de retorno
                retUnidadeFederativa.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                retUnidadeFederativa.UnidadesFederativas = lstUnidadesFederativas;
            }
            else
            {
                // Preenche o objeto de retorno
                retUnidadeFederativa.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;
                retUnidadeFederativa.Mensagem = "Não existem unidades federativas cadastradas.";
            }

            // retorna os dados
            return retUnidadeFederativa;
        }
    }
}
