using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrasilDidaticos.WcfServico;

namespace BrasilDidaticos.WcfServico.Negocio
{
    static class Permissao
    {
        /// <summary>
        /// Método para listar os permissões
        /// </summary>
        /// <param name="entradaPermissao.Permissoes">Objeto com os dados do filtro</param>
        /// <returns>Contrato.RetornoPermissao</returns>
        internal static Contrato.RetornoPermissao ListarPermissao(Contrato.EntradaPermissao entradaPermissao)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoPermissao retPermissao = new Contrato.RetornoPermissao();
             
            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaPermissao.UsuarioLogado, Chave = entradaPermissao.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {

                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                // Busca o permissao no banco
                List<Dados.PERMISSAO> lstPermissoes = (from f in context.T_PERMISSAO
                                                where (f.BOL_ATIVO == entradaPermissao.Permissao.Ativo)
                                                select f).ToList();

                // Verifica se foi encontrado algum registro
                if (lstPermissoes.Count > 0)
                {
                    // Preenche o objeto de retorno
                    retPermissao.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    retPermissao.Permissoes = new List<Contrato.Permissao>();
                    foreach (Dados.PERMISSAO permissao in lstPermissoes)
                    {
                        retPermissao.Permissoes.Add(new Contrato.Permissao()
                        {
                            Id = permissao.ID_PERMISSAO,
                            Nome = permissao.NOME_PERMISSAO,
                            Ativo = permissao.BOL_ATIVO
                        });
                    };

                }
                else
                {
                    // Preenche o objeto de retorno
                    retPermissao.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;
                    retPermissao.Mensagem = "Não existe dados para o filtro informado.";
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retPermissao.Codigo = retSessao.Codigo;
                retPermissao.Mensagem = retSessao.Mensagem;
            }
            
            // retorna os dados
            return retPermissao;
        }

        /// <summary>
        /// Retorna uma lista de permissoes
        /// </summary>
        /// <param name="lstUsuarioPermissao">Recebe os permissaos do usuário recuperado do banco</param>
        /// <returns>List<Contrato.Permissao></returns>
        internal static List<Contrato.Permissao> ListarPerfilPermissao(System.Data.Objects.DataClasses.EntityCollection<Dados.PERFIL_PERMISSAO> lstPerfilPermissao)
        {
            List<Contrato.Permissao> lstPermissao = null;

            if (lstPerfilPermissao != null)
            {
                lstPermissao = new List<Contrato.Permissao>();

                foreach (Dados.PERFIL_PERMISSAO permissao in lstPerfilPermissao)
                {
                    lstPermissao.Add(new Contrato.Permissao 
                    {
                        Id = permissao.T_PERMISSAO.ID_PERMISSAO,
                        Nome = permissao.T_PERMISSAO.NOME_PERMISSAO,
                        Ativo = permissao.T_PERMISSAO.BOL_ATIVO
                    });
                }
            }

            return lstPermissao;
        }

        /// <summary>
        /// Método para verificar se as informações do permissao foram preenchidas
        /// </summary>
        /// <param name="Usuario">Objeto com o dados do permissao</param>
        /// <returns></returns>
        private static string ValidarPermissaoPreenchido(Contrato.Permissao Permissao)
        {
            // Cria a variável de retorno
            string strRetorno = string.Empty;

            // Verifica se a Nome foi preenchida
            if (string.IsNullOrWhiteSpace(Permissao.Nome))
                strRetorno += "O campo 'Nome' não foi informado!\n";

            // retorna a variável de retorno
            return strRetorno;

        }       
    }
}
