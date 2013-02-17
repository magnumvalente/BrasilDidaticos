using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrasilDidaticos.WcfServico;

namespace BrasilDidaticos.WcfServico.Negocio
{
    internal static class Sessao
    {
        /// <summary>
        /// Método para logar na aplicação
        /// </summary>
        /// <param name="Sessao">Objeto com os dados da sessão a validar</param>
        /// <returns>Contrato.RetornoSessao</returns>
        internal static Contrato.RetornoSessao ValidarSessao(Contrato.Sessao Sessao)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoSessao retSessao = new Contrato.RetornoSessao();

            // Verifica se as informações do usuário forma informadas
            string strValidacao = ValidarSessaoPreenchido(Sessao);

            if (strValidacao.Length > 0)
            {
                retSessao.Codigo = Contrato.Constantes.COD_FILTRO_VAZIO;
                retSessao.Mensagem = strValidacao;
            }
            else
            {
                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                // Busca o usuario no banco
                Dados.SESSAO objSessao = (from s in context.T_SESSAO
                                            where s.LOGIN_USUARIO == Sessao.Login
                                            && s.DES_CHAVE == Sessao.Chave
                                            select s).FirstOrDefault();

                // Verifica se foi encontrado algum registro
                if (objSessao != null)
                {
                    // Preenche o objeto de retorno
                    retSessao.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;                   
                }
                else
                {
                    // Preenche o objeto de retorno
                    retSessao.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;
                    retSessao.Mensagem = "Não existe dados para o filtro informado.";
                }
            }

            // retorna os dados
            return retSessao;
        }

        /// <summary>
        /// Método para listar as sessões do usuário
        /// </summary>
        /// <param name="Sessao">Objeto com os dados da sessão a listar</param>
        /// <returns>Contrato.RetornoSessao</returns>
        internal static Contrato.RetornoSessao ListarSessao(Contrato.Sessao Sessao)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoSessao retSessao = new Contrato.RetornoSessao();

            // Loga no banco de dados
            Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

            // Busca o usuario no banco
            List<Dados.SESSAO> objSessoes = (from s in context.T_SESSAO
                                        where s.LOGIN_USUARIO == Sessao.Login || Sessao.Login == string.Empty
                                        select s).ToList();

            // Verifica se foi encontrado algum registro
            if (objSessoes != null && objSessoes.Count > 0)
            {
                // Preenche o objeto de retorno
                retSessao.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                retSessao.Sessoes = new List<Contrato.Sessao>();

                // Para cada sessão existente
                foreach (Dados.SESSAO sessao in objSessoes)
                {
                    retSessao.Sessoes.Add(new Contrato.Sessao
                    {
                        Id = sessao.ID_SESSAO,
                        Login = sessao.LOGIN_USUARIO,
                        Chave = sessao.DES_CHAVE
                    });
                }
            }
            else
            {
                // Preenche o objeto de retorno
                retSessao.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;
                retSessao.Mensagem = "Não existe dados para o filtro informado.";
            }

            // retorna os dados
            return retSessao;
        }

        /// <summary>
        /// Método para salvar a sessão
        /// </summary>
        /// <param name="Sessao">Objeto com os dados da sessão</param>
        /// <returns>Contrato.RetornoSessao</returns>
        internal static Contrato.RetornoSessao SalvarSessao(Contrato.Sessao Sessao)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoSessao retSessao = new Contrato.RetornoSessao();

            // Verifica se as informações do usuário foram informadas
            string strValidacao = ValidarSessaoPreenchido(Sessao);

            // Se existe algum erro
            if (strValidacao.Length > 0)
            {
                retSessao.Codigo = Contrato.Constantes.COD_FILTRO_VAZIO;
                retSessao.Mensagem = strValidacao;
            }
            else
            {
                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                // Busca o usuário no banco
                List<Dados.SESSAO> lstSessoes = (from s in context.T_SESSAO
                                                          where s.LOGIN_USUARIO == Sessao.Login                                                          
                                                          select s).ToList();

                // Verifica se foi encontrado algum registro
                if (lstSessoes.Count > 0 )
                {
                    if (lstSessoes.First().DES_CHAVE != Sessao.Chave)
                    {
                        // Preenche o objeto de retorno
                        retSessao.Codigo = Contrato.Constantes.COD_REGISTRO_DUPLICADO;
                        retSessao.Mensagem = string.Format("O usuário de Login '{0}' já está logado!", Sessao.Login);
                    }
                }
                else
                {
                    // Cria o usuário
                    Dados.SESSAO tSessao = new Dados.SESSAO();
                    tSessao.ID_SESSAO = Guid.NewGuid();
                    tSessao.LOGIN_USUARIO = Sessao.Login;                        
                    tSessao.DATA_LOGIN = DateTime.Now;
                    tSessao.DES_CHAVE = Sessao.Chave;
                    context.AddToT_SESSAO(tSessao);

                    // Salva as alterações
                    context.SaveChanges();

                    // Preenche o objeto de retorno
                    retSessao.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                }
            }

            // retorna dos dados 
            return retSessao;
        }

        /// <summary>
        /// Método para salvar o usuário
        /// </summary>
        /// <param name="Sessao">Objeto com os dados do usuário a Logar</param>
        /// <returns>Contrato.RetornoFornecedor</returns>
        internal static Contrato.RetornoSessao ExcluirSessao(Contrato.Sessao Sessao)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoSessao retSessao = new Contrato.RetornoSessao();

            // Verifica se as informações do usuário foram informadas
            string strValidacao = ValidarSessaoPreenchido(Sessao);

            // Se existe algum erro
            if (strValidacao.Length > 0)
            {
                retSessao.Codigo = Contrato.Constantes.COD_FILTRO_VAZIO;
                retSessao.Mensagem = strValidacao;
            }
            else
            {
                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                // Busca o usuário no banco
                List<Dados.SESSAO> lstSessoes = (from s in context.T_SESSAO
                                                 where s.LOGIN_USUARIO == Sessao.Login
                                                 && s.DES_CHAVE == Sessao.Chave
                                                 select s).ToList();

                // Verifica se foi encontrado algum registro
                if (lstSessoes.Count > 0)
                {
                    // Exclui a sessão do usuário
                    context.T_SESSAO.DeleteObject(lstSessoes.First());

                    // Salva as alterações
                    context.SaveChanges();

                    // Preenche o objeto de retorno
                    retSessao.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                }
                else
                {
                    // Preenche o objeto de retorno
                    retSessao.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;                    
                }
            }

            // retorna dos dados 
            return retSessao;
        }

        /// <summary>
        /// Método para verificar se as informações da sessão foram preenchidas
        /// </summary>
        /// <param name="Sessao">Objeto com o dados da sessão</param>
        /// <returns>string</returns>
        private static string ValidarSessaoPreenchido(Contrato.Sessao Sessao)
        {
            // Cria a variável de retorno
            string strRetorno = string.Empty;

            // Verifica se o Login foi preenchido
            if (string.IsNullOrWhiteSpace(Sessao.Login))
                strRetorno = "O campo 'Login' não foi informado!\n";

            // Verifica se o Nome foi preenchido
            if (string.IsNullOrWhiteSpace(Sessao.Chave))
                strRetorno = "O campo 'Chave' não foi informado!\n";

            // retorna a variável de retorno
            return strRetorno;

        }       
    }
}
