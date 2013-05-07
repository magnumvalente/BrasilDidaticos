using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrasilDidaticos.WcfServico;

namespace BrasilDidaticos.WcfServico.Negocio
{
    static class Empresa
    {
        /// <summary>
        /// Método para listar as empresas
        /// </summary>
        /// <param name="entradaEmpresa.Empresas">Objeto com os dados do filtro</param>
        /// <returns>Contrato.RetornoEmpresa</returns>
        internal static Contrato.RetornoEmpresa ListarEmpresa(Contrato.EntradaEmpresa entradaEmpresa)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoEmpresa retEmpresa = new Contrato.RetornoEmpresa();
             
            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaEmpresa.UsuarioLogado, Chave = entradaEmpresa.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {

                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                // Busca o empresa no banco
                List<Dados.EMPRESA> lstEmpresas = (
                                                    from f in context.T_EMPRESA
                                                    where (f.BOL_ATIVO == entradaEmpresa.Empresa.Ativo)
                                                    select f
                                                   ).ToList();

                // Verifica se foi encontrado algum registro
                if (lstEmpresas.Count > 0)
                {
                    // Preenche o objeto de retorno
                    retEmpresa.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    retEmpresa.Empresas = new List<Contrato.Empresa>();
                    foreach (Dados.EMPRESA empresa in lstEmpresas)
                    {
                        retEmpresa.Empresas.Add(new Contrato.Empresa()
                        {
                            Id = empresa.ID_EMPRESA,
                            Codigo = empresa.COD_EMPRESA,
                            Nome = empresa.NOME_EMPRESA,
                            Ativo = empresa.BOL_ATIVO
                        });
                    };

                }
                else
                {
                    // Preenche o objeto de retorno
                    retEmpresa.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;
                    retEmpresa.Mensagem = "Não existe dados para o filtro informado.";
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retEmpresa.Codigo = retSessao.Codigo;
                retEmpresa.Mensagem = retSessao.Mensagem;
            }
            
            // retorna os dados
            return retEmpresa;
        }

        /// <summary>
        /// Retorna a empresa
        /// </summary>
        /// <param name="lstUsuarioEmpresa">Recebe a empresa do usuário recuperado do banco</param>
        /// <returns>Contrato.Empresa</returns>
        internal static Contrato.Empresa BuscarUsuarioEmpresa(Dados.EMPRESA empresa)
        {
            Contrato.Empresa retEmpresa = new Contrato.Empresa();

            if (empresa != null)
            {
                retEmpresa = new Contrato.Empresa 
                {
                    Id = empresa.ID_EMPRESA,
                    Codigo = empresa.COD_EMPRESA,
                    Nome = empresa.NOME_EMPRESA,
                    Ativo = empresa.BOL_ATIVO
                };
            }

            return retEmpresa;
        }

        /// <summary>
        /// Método para verificar se as informações do empresa foram preenchidas
        /// </summary>
        /// <param name="Usuario">Objeto com o dados do empresa</param>
        /// <returns></returns>
        private static string ValidarEmpresaPreenchido(Contrato.Empresa Empresa)
        {
            // Cria a variável de retorno
            string strRetorno = string.Empty;

            // Verifica se a Nome foi preenchida
            if (string.IsNullOrWhiteSpace(Empresa.Nome))
                strRetorno += "O campo 'Nome' não foi informado!\n";

            // retorna a variável de retorno
            return strRetorno;

        }       
    }
}
