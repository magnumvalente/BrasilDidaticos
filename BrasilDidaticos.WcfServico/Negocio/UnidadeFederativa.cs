using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrasilDidaticos.WcfServico;

namespace BrasilDidaticos.WcfServico.Negocio
{
    public static class UnidadeFederativa
    {
        /// <summary>
        /// Método para buscar a unidade federativa
        /// </summary>
        /// <param name="codigoUnidade">Código da unidade federativa do cliente</param>
        /// <returns>Contrato.UnidadeFederativa</returns>
        public static Contrato.UnidadeFederativa BuscarUnidadeFederativa(string codigoUnidade)
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
        /// Método para retornar as unidades federativas
        /// </summary>        
        /// <returns>List<Contrato.UnidadeFederativa></returns>
        public static Contrato.RetornoUnidadeFederativa ListarUnidadeFederativa()
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoUnidadeFederativa retUnidadeFederativa = new Contrato.RetornoUnidadeFederativa();

            // Loga no banco de dados
            Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

            // Busca o usuario no banco
            List<Dados.UNIDADE_FEDERATIVA> lstUnidadesFederativas = context.T_UNIDADE_FEDERATIVA.ToList();

            // Verifica se foi encontrado algum registro
            if (lstUnidadesFederativas.Count > 0)
            {
                // Preenche o objeto de retorno
                retUnidadeFederativa.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                retUnidadeFederativa.UnidadesFederativas = new List<Contrato.UnidadeFederativa>();
                
                // Preenche o objeto de retorno
                foreach (Dados.UNIDADE_FEDERATIVA unidadeFederativa in lstUnidadesFederativas)
                {
                    retUnidadeFederativa.UnidadesFederativas.Add(new Contrato.UnidadeFederativa()
                    {
                        Id = unidadeFederativa.ID_UNIDADE_FEDERATIVA,
                        Codigo = unidadeFederativa.COD_UNIDADE_FEDERATIVA,
                        Nome = unidadeFederativa.NOME_UNIDADE_FEDERATIVA
                    });
                };
            }

            // retorna os dados
            return retUnidadeFederativa;
        }
    }
}
