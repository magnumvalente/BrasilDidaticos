using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrasilDidaticos.WcfServico;

namespace BrasilDidaticos.WcfServico.Negocio
{
    static class Perfil
    {
        /// <summary>
        /// Método para listar os perfis
        /// </summary>
        /// <param name="entradaPerfil.Perfis">Objeto com os dados do filtro</param>
        /// <returns>Contrato.RetornoPerfil</returns>
        internal static Contrato.RetornoPerfil ListarPerfil(Contrato.EntradaPerfil entradaPerfil)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoPerfil retPerfil = new Contrato.RetornoPerfil();
             
            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaPerfil.UsuarioLogado, Chave = entradaPerfil.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Verifica se a empresa não foi informada
                if (string.IsNullOrWhiteSpace(entradaPerfil.EmpresaLogada.Id.ToString()))
                {
                    entradaPerfil.EmpresaLogada.Id = Guid.Empty;
                }

                // Verifica se o nome foi informado
                if (string.IsNullOrWhiteSpace(entradaPerfil.Perfil.Codigo))
                {
                    entradaPerfil.Perfil.Codigo = string.Empty;
                }

                // Verifica se o Login foi informado
                if (string.IsNullOrWhiteSpace(entradaPerfil.Perfil.Nome))
                {
                    entradaPerfil.Perfil.Nome = string.Empty;
                }

                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();
                context.ContextOptions.LazyLoadingEnabled = true;

                // Busca o perfil no banco
                List<Dados.PERFIL> lstPerfis = (from p in context.T_PERFIL
                                                where p.BOL_ATIVO == entradaPerfil.Perfil.Ativo                                                   
                                                   && (entradaPerfil.EmpresaLogada.Id == Guid.Empty || p.ID_EMPRESA == entradaPerfil.EmpresaLogada.Id)
                                                   && (entradaPerfil.Perfil.Codigo == string.Empty || p.COD_PERFIL.Contains(entradaPerfil.Perfil.Codigo))
                                                   && (entradaPerfil.Perfil.Nome == string.Empty || p.NOME_PERFIL.Contains(entradaPerfil.Perfil.Nome))
                                                select p).ToList();

                // Verifica se foi encontrado algum registro
                if (lstPerfis.Count > 0)
                {
                    // Preenche o objeto de retorno
                    retPerfil.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    retPerfil.Perfis = new List<Contrato.Perfil>();
                    foreach (Dados.PERFIL perfil in lstPerfis)
                    {
                        retPerfil.Perfis.Add(new Contrato.Perfil()
                        {
                            Id = perfil.ID_PERFIL,
                            Codigo = perfil.COD_PERFIL,
                            Nome = perfil.NOME_PERFIL,
                            Descricao = perfil.DES_PERFIL,
                            Ativo = perfil.BOL_ATIVO,
                            Permissoes = Negocio.Permissao.ListarPerfilPermissao(perfil.T_PERFIL_PERMISSAO)
                        });
                    };

                }
                else
                {
                    // Preenche o objeto de retorno
                    retPerfil.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;
                    retPerfil.Mensagem = "Não existe dados para o filtro informado.";
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retPerfil.Codigo = retSessao.Codigo;
                retPerfil.Mensagem = retSessao.Mensagem;
            }
            
            // retorna os dados
            return retPerfil;
        }

        /// <summary>
        /// Retorna uma lista de perfis
        /// </summary>
        /// <param name="lstUsuarioPerfil">Recebe os perfils do usuário recuperado do banco</param>
        /// <returns>List<Contrato.Perfil></returns>
        internal static List<Contrato.Perfil> ListarUsuarioPerfil(System.Data.Objects.DataClasses.EntityCollection<Dados.USUARIO_PERFIL> lstUsuarioPerfil)
        {
            List<Contrato.Perfil> lstPerfil = null;

            if (lstUsuarioPerfil != null)
            {
                lstPerfil = new List<Contrato.Perfil>();

                foreach (Dados.USUARIO_PERFIL perfil in lstUsuarioPerfil)
                {
                    lstPerfil.Add(new Contrato.Perfil 
                    {
                        Id = perfil.T_PERFIL.ID_PERFIL,
                        Codigo = perfil.T_PERFIL.COD_PERFIL,
                        Nome = perfil.T_PERFIL.NOME_PERFIL,
                        Descricao = perfil.T_PERFIL.DES_PERFIL,
                        Ativo = perfil.T_PERFIL.BOL_ATIVO,
                        Permissoes = Negocio.Permissao.ListarPerfilPermissao(perfil.T_PERFIL.T_PERFIL_PERMISSAO)
                    });
                }
            }

            return lstPerfil;
        }

        /// <summary>
        /// Método para salvar o perfil
        /// </summary>
        /// <param name="Perfis">Objeto com os dados do perfil</param>
        /// <returns>Contrato.RetornoPerfil</returns>
        internal static Contrato.RetornoPerfil SalvarPerfil(Contrato.EntradaPerfil entradaPerfil)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoPerfil retPerfil = new Contrato.RetornoPerfil();

            // Verifica se as informações do perfil foram informadas
            string strValidacao = ValidarPerfilPreenchido(entradaPerfil.Perfil);

            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaPerfil.UsuarioLogado, Chave = entradaPerfil.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Se existe algum erro
                if (strValidacao.Length > 0)
                {
                    retPerfil.Codigo = Contrato.Constantes.COD_FILTRO_VAZIO;
                    retPerfil.Mensagem = strValidacao;
                }
                else
                {
                    // Loga no banco de dados
                    Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                    // Busca o perfil no banco
                    List<Dados.PERFIL> lstPerfis = (from p in context.T_PERFIL
                                                    where (p.COD_PERFIL == entradaPerfil.Perfil.Codigo
                                                          && (entradaPerfil.EmpresaLogada.Id == Guid.Empty || p.ID_EMPRESA == entradaPerfil.EmpresaLogada.Id))
                                                       || (entradaPerfil.Novo == null && entradaPerfil.Perfil.Id == p.ID_PERFIL)
                                                    select p).ToList();

                     // Verifica se foi encontrado algum registro
                    if (lstPerfis.Count > 0 && entradaPerfil.Novo != null && (bool)entradaPerfil.Novo)
                    {
                        // Preenche o objeto de retorno
                        retPerfil.Codigo = Contrato.Constantes.COD_REGISTRO_DUPLICADO;
                        retPerfil.Mensagem = string.Format("O perfil de código '{0}' já existe!", lstPerfis.First().COD_PERFIL);
                    }
                    else
                    {
                        // Se existe o perfil
                        if (lstPerfis.Count > 0)
                        {
                            // Atualiza o perfil
                            lstPerfis.First().NOME_PERFIL = entradaPerfil.Perfil.Nome;
                            lstPerfis.First().DES_PERFIL = entradaPerfil.Perfil.Descricao;
                            lstPerfis.First().BOL_ATIVO = entradaPerfil.Perfil.Ativo;
                            lstPerfis.First().DATA_ATUALIZACAO = DateTime.Now;
                            lstPerfis.First().LOGIN_USUARIO = entradaPerfil.UsuarioLogado;

                            // Apaga todos os perfis que estão relacionados
                            while (lstPerfis.First().T_PERFIL_PERMISSAO.Count > 0)
                            {
                                context.T_PERFIL_PERMISSAO.DeleteObject(lstPerfis.First().T_PERFIL_PERMISSAO.First());
                            }

                            // Verifica se existe algum perfil associado ao usuário
                            if (entradaPerfil.Perfil.Permissoes != null)
                            {
                                // Para cada perfil associado
                                foreach (Contrato.Permissao permissao in entradaPerfil.Perfil.Permissoes)
                                {
                                    // Associa o perfil ao usuário
                                    lstPerfis.First().T_PERFIL_PERMISSAO.Add(new Dados.PERFIL_PERMISSAO()
                                    {
                                        ID_PERFIL_PERMISSAO = Guid.NewGuid(),
                                        ID_PERFIL = lstPerfis.First().ID_PERFIL,
                                        ID_PERMISSAO = permissao.Id,
                                        LOGIN_USUARIO = entradaPerfil.UsuarioLogado,
                                        DATA_ATUALIZACAO = DateTime.Now
                                    });
                                }
                            }      
                        }
                        else
                        {
                            // Cria o perfil
                            Dados.PERFIL tPerfil = new Dados.PERFIL();
                            tPerfil.ID_PERFIL = Guid.NewGuid();
                            tPerfil.ID_EMPRESA = entradaPerfil.EmpresaLogada.Id;
                            tPerfil.COD_PERFIL = entradaPerfil.Perfil.Codigo;
                            tPerfil.NOME_PERFIL = entradaPerfil.Perfil.Nome;
                            tPerfil.DES_PERFIL = entradaPerfil.Perfil.Descricao;
                            tPerfil.BOL_ATIVO = entradaPerfil.Perfil.Ativo;
                            tPerfil.DATA_ATUALIZACAO = DateTime.Now;
                            tPerfil.LOGIN_USUARIO = entradaPerfil.UsuarioLogado;

                            // Verifica se existe algum perfil associado ao usuário
                            if (entradaPerfil.Perfil.Permissoes != null)
                            {
                                // Para cada perfil associado
                                foreach (Contrato.Permissao permissao in entradaPerfil.Perfil.Permissoes)
                                {
                                    // Associa o perfil ao usuário
                                    tPerfil.T_PERFIL_PERMISSAO.Add(new Dados.PERFIL_PERMISSAO()
                                    {
                                        ID_PERFIL_PERMISSAO = Guid.NewGuid(),
                                        ID_PERFIL = entradaPerfil.Perfil.Id,
                                        ID_PERMISSAO = permissao.Id,
                                        LOGIN_USUARIO = entradaPerfil.UsuarioLogado,
                                        DATA_ATUALIZACAO = DateTime.Now
                                    });
                                }
                            }   

                            context.AddToT_PERFIL(tPerfil);
                        }

                        // Salva as alterações
                        context.SaveChanges();

                        // Preenche o objeto de retorno
                        retPerfil.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    }
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retPerfil.Codigo = retSessao.Codigo;
                retPerfil.Mensagem = retSessao.Mensagem;
            }

            // retorna dos dados 
            return retPerfil;
        }

        /// <summary>
        /// Método para verificar se as informações do perfil foram preenchidas
        /// </summary>
        /// <param name="Usuario">Objeto com o dados do perfil</param>
        /// <returns></returns>
        private static string ValidarPerfilPreenchido(Contrato.Perfil Perfil)
        {
            // Cria a variável de retorno
            string strRetorno = string.Empty;

            // Verifica se o Codigo foi preenchido
            if (string.IsNullOrWhiteSpace(Perfil.Codigo))
                strRetorno = "O campo 'Codigo' não foi informado!\n";

            // Verifica se o Nome foi preenchida
            if (string.IsNullOrWhiteSpace(Perfil.Nome))
                strRetorno += "O campo 'Nome' não foi informado!\n";

            // retorna a variável de retorno
            return strRetorno;

        }       
    }
}
