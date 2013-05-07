using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrasilDidaticos.WcfServico;

namespace BrasilDidaticos.WcfServico.Negocio
{
    static class Usuario
    {
        /// <summary>
        /// Método para logar na aplicação
        /// </summary>
        /// <param name="entradaUsuario">Objeto com os dados do usuário a Logar</param>
        /// <returns>Contrato.RetornoUsuario</returns>
        internal static Contrato.RetornoUsuario Logar(Contrato.EntradaUsuario entradaUsuario)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoUsuario retUsuario = new Contrato.RetornoUsuario();

            // Verifica se as informações do usuário forma informadas
            string strValidacao = ValidarLoginUsuarioPreenchido(entradaUsuario.Usuario);

            if (strValidacao.Length > 0)
            {
                retUsuario.Codigo = Contrato.Constantes.COD_FILTRO_VAZIO;
                retUsuario.Mensagem = strValidacao;
            }
            else
            {
                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();
                string con = context.Connection.ConnectionString;

                // Busca o usuario no banco
                Dados.USUARIO objUsuario = (from u in context.T_USUARIO
                                            where u.LOGIN_USUARIO == entradaUsuario.Usuario.Login
                                            && u.SENHA_USUARIO == entradaUsuario.Usuario.Senha
                                            && u.BOL_ATIVO == true
                                            select u).FirstOrDefault();

                // Verifica se foi encontrado algum registro
                if (objUsuario != null)
                {                    
                    retUsuario.Usuarios = new List<Contrato.Usuario>();
                    retUsuario.Usuarios.Add(new Contrato.Usuario()
                    {
                        Id = objUsuario.ID_USUARIO,
                        Nome = objUsuario.NOME_USUARIO
                    });
                    
                    if (!entradaUsuario.PreencherListaSelecao)
                    {
                        retUsuario.Usuarios.Last().Login = objUsuario.LOGIN_USUARIO;
                        retUsuario.Usuarios.Last().Senha = objUsuario.SENHA_USUARIO;
                        retUsuario.Usuarios.Last().Ativo = objUsuario.BOL_ATIVO;
                        retUsuario.Usuarios.Last().Empresa = Negocio.Empresa.BuscarUsuarioEmpresa(objUsuario.T_EMPRESA);
                        retUsuario.Usuarios.Last().Perfis = Negocio.Perfil.ListarUsuarioPerfil(objUsuario.T_USUARIO_PERFIL);
                    }

                    Contrato.RetornoSessao retSessao = Negocio.Sessao.SalvarSessao(new Contrato.Sessao() 
                    {
                        Login = entradaUsuario.Usuario.Login,
                        Chave = entradaUsuario.Chave
                    });

                    if (retUsuario.Usuarios.First().Empresa == null) 
                    {
                        // Preenche o objeto de retorno
                        retUsuario.Codigo = Contrato.Constantes.COD_EMPRESA_INEXISTENTE;
                        retUsuario.Mensagem = string.Format("Não foi encontrado uma empresa para o usuário '{0}' informado.", retUsuario.Usuarios.First().Nome);
                    }
                    else if (!retUsuario.Usuarios.First().Empresa.Ativo)
                    {
                        // Preenche o objeto de retorno
                        retUsuario.Codigo = Contrato.Constantes.COD_EMPRESA_DESATIVADA;
                        retUsuario.Mensagem = string.Format("Infelizmente a empresa '{0}' não está ativa para o sistema.", retUsuario.Usuarios.First().Empresa.Nome);
     
                    }
                    else
                    {
                        // Preenche o objeto de retorno
                        retUsuario.Codigo = retSessao.Codigo;
                        retUsuario.Mensagem = retSessao.Mensagem;
                    }
                }
                else
                {
                    // Preenche o objeto de retorno
                    retUsuario.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;
                    retUsuario.Mensagem = "Não existe dados para o filtro informado.";
                }
            }

            // retorna os dados
            return retUsuario;
        }

        /// <summary>
        /// Método para buscar o usuário
        /// </summary>
        /// <param name="Usuario">Objeto com o identificador do usuário</param>
        /// <returns>Contrato.RetornoUsuario</returns>
        internal static Contrato.Usuario BuscarUsuario(Dados.USUARIO usuario)
        {
            // Objeto que recebe o retorno do método
            Contrato.Usuario retUsuario = new Contrato.Usuario();

            // Verifica se foi encontrado algum registro
            if (usuario != null)
            {
                retUsuario = new Contrato.Usuario()
                {
                    Id = usuario.ID_USUARIO,                 
                    Nome = usuario.NOME_USUARIO,
                    Login = usuario.LOGIN_USUARIO,
                    Ativo = usuario.BOL_ATIVO,
                    Perfis = Negocio.Perfil.ListarUsuarioPerfil(usuario.T_USUARIO_PERFIL)
                };
            }

            // retorna os dados
            return retUsuario;
        }
        
        /// <summary>
        /// Método para listar os usuários
        /// </summary>
        /// <param name="entradaUsuario">Objeto com os dados do usuário a listar</param>
        /// <returns>Contrato.RetornoUsuario</returns>
        internal static Contrato.RetornoUsuario ListarUsuario(Contrato.EntradaUsuario entradaUsuario)
        {

            // Objeto que recebe o retorno do método
            Contrato.RetornoUsuario retUsuario = new Contrato.RetornoUsuario();

            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaUsuario.UsuarioLogado, Chave = entradaUsuario.Chave});
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Verifica se a empresa não foi informada
                if (string.IsNullOrWhiteSpace(entradaUsuario.EmpresaLogada.Id.ToString()))
                {
                    entradaUsuario.EmpresaLogada.Id = Guid.Empty;
                }

                // Verifica se o nome foi informado
                if (string.IsNullOrWhiteSpace(entradaUsuario.Usuario.Nome))
                {
                    entradaUsuario.Usuario.Nome = string.Empty;
                }

                // Verifica se o Login foi informado
                if (string.IsNullOrWhiteSpace(entradaUsuario.Usuario.Login))
                {
                    entradaUsuario.Usuario.Login = string.Empty;
                }

                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                // Busca o usuario no banco
                List<Dados.USUARIO> lstUsuarios = (from u in context.T_USUARIO                                                   
                                                   where
                                                        (entradaUsuario.Usuario.Ativo == null || u.BOL_ATIVO == entradaUsuario.Usuario.Ativo)
                                                     && (entradaUsuario.EmpresaLogada.Id == Guid.Empty || u.ID_EMPRESA == entradaUsuario.EmpresaLogada.Id)
                                                     && (entradaUsuario.Usuario.Nome == string.Empty || u.NOME_USUARIO.StartsWith(entradaUsuario.Usuario.Nome))
                                                     && (entradaUsuario.Usuario.Login == string.Empty || u.LOGIN_USUARIO.Contains(entradaUsuario.Usuario.Login))
                                                   select u).ToList();


                 // Verifica se o código do Perfil foi informado
                string codigoPerfil = string.Empty;
                if (entradaUsuario.Usuario.Perfis != null && entradaUsuario.Usuario.Perfis.Count > 0)
                {
                    codigoPerfil = entradaUsuario.Usuario.Perfis.First().Codigo;
                    lstUsuarios = (from u in lstUsuarios                                   
                                   join up in context.T_USUARIO_PERFIL on u.ID_USUARIO equals up.ID_USUARIO
                                   join p in context.T_PERFIL on up.ID_PERFIL equals p.ID_PERFIL
                                   where (p.COD_PERFIL == codigoPerfil)
                                   select u).ToList();
                }

                // Verifica se foi encontrado algum registro
                if (lstUsuarios != null && lstUsuarios.Count > 0)
                {

                    // Preenche o objeto de retorno
                    retUsuario.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    retUsuario.Usuarios = new List<Contrato.Usuario>();

                    foreach (Dados.USUARIO usuario in lstUsuarios)
                    {
                        retUsuario.Usuarios.Add(new Contrato.Usuario()
                        {
                            Id = usuario.ID_USUARIO,
                            Nome = usuario.NOME_USUARIO,
                            Login = usuario.LOGIN_USUARIO,
                            Senha = usuario.SENHA_USUARIO,
                            Ativo = usuario.BOL_ATIVO,
                            Perfis = Negocio.Perfil.ListarUsuarioPerfil(usuario.T_USUARIO_PERFIL)
                        });
                    }
                }
                else
                {
                    // Preenche o objeto de retorno
                    retUsuario.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;
                    retUsuario.Mensagem = "Não existe dados para o filtro informado.";
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retUsuario.Codigo = retSessao.Codigo;
                retUsuario.Mensagem = retSessao.Mensagem;
            }

            // retorna os dados
            return retUsuario;
        }

        /// <summary>
        /// Método para salvar o usuário
        /// </summary>
        /// <param name="entradaUsuario">Objeto com os dados do usuário a Logar</param>
        /// <returns>Contrato.RetornoUsuario</returns>
        internal static Contrato.RetornoUsuario SalvarUsuario(Contrato.EntradaUsuario entradaUsuario)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoUsuario retUsuario = new Contrato.RetornoUsuario();

            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaUsuario.UsuarioLogado, Chave = entradaUsuario.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {

                // Verifica se as informações do usuário foram informadas
                string strValidacao = ValidarUsuarioPreenchido(entradaUsuario.Usuario);

                // Se existe algum erro
                if (strValidacao.Length > 0)
                {
                    retUsuario.Codigo = Contrato.Constantes.COD_FILTRO_VAZIO;
                    retUsuario.Mensagem = strValidacao;
                }
                else
                {
                    // Loga no banco de dados
                    Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                    // Busca o usuário no banco
                    List<Dados.USUARIO> lstUsuarios = (from u in context.T_USUARIO
                                                              where (u.LOGIN_USUARIO == entradaUsuario.Usuario.Login
                                                                 && (entradaUsuario.EmpresaLogada.Id == Guid.Empty || u.ID_EMPRESA == entradaUsuario.EmpresaLogada.Id)
                                                                ||  (entradaUsuario.Novo == null && entradaUsuario.Usuario.Id == u.ID_USUARIO))
                                                              select u).ToList();

                    // Verifica se foi encontrado algum registro
                    if (lstUsuarios.Count > 0 && entradaUsuario.Novo != null && (bool)entradaUsuario.Novo)
                    {
                        // Preenche o objeto de retorno
                        retUsuario.Codigo = Contrato.Constantes.COD_REGISTRO_DUPLICADO;
                        retUsuario.Mensagem = string.Format("O usuário de Login '{0}' já existe!", lstUsuarios.First().LOGIN_USUARIO);
                    }
                    else
                    {
                        // Se existe o usuário
                        if (lstUsuarios.Count > 0 )
                        {
                            // Atualiza o fornecedor
                            lstUsuarios.First().NOME_USUARIO = entradaUsuario.Usuario.Nome;
                            lstUsuarios.First().LOGIN_USUARIO = entradaUsuario.Usuario.Login;
                            lstUsuarios.First().SENHA_USUARIO = entradaUsuario.Usuario.Senha;
                            lstUsuarios.First().BOL_ATIVO = (bool)entradaUsuario.Usuario.Ativo;                        
                            lstUsuarios.First().DATA_ATUALIZACAO = DateTime.Now;
                            lstUsuarios.First().USUARIO_LOGADO = entradaUsuario.UsuarioLogado;

                            // Apaga todos os perfis que estão relacionados
                            while (lstUsuarios.First().T_USUARIO_PERFIL.Count > 0)
                            {
                                context.T_USUARIO_PERFIL.DeleteObject(lstUsuarios.First().T_USUARIO_PERFIL.First());
                            }                            

                            // Verifica se existe algum perfil associado ao usuário
                            if (entradaUsuario.Usuario.Perfis != null)
                            {
                                // Para cada perfil associado
                                foreach (Contrato.Perfil perfil in entradaUsuario.Usuario.Perfis)
                                {
                                    // Associa o perfil ao usuário
                                    lstUsuarios.First().T_USUARIO_PERFIL.Add(new Dados.USUARIO_PERFIL()
                                    {
                                        ID_USUARIO_PERFIL = Guid.NewGuid(),
                                        ID_USUARIO = lstUsuarios.First().ID_USUARIO,
                                        ID_PERFIL = perfil.Id,
                                        LOGIN_USUARIO = entradaUsuario.UsuarioLogado,
                                        DATA_ATUALIZACAO = DateTime.Now
                                    });
                                }
                            }
                        }
                        else
                        {
                            // Cria o usuário
                            Dados.USUARIO tUsuario = new Dados.USUARIO();
                            tUsuario.ID_USUARIO = Guid.NewGuid();
                            tUsuario.NOME_USUARIO = entradaUsuario.Usuario.Nome;
                            tUsuario.LOGIN_USUARIO = entradaUsuario.Usuario.Login;
                            tUsuario.SENHA_USUARIO = entradaUsuario.Usuario.Senha;
                            tUsuario.ID_EMPRESA = entradaUsuario.EmpresaLogada.Id;
                            tUsuario.BOL_ATIVO = (bool)entradaUsuario.Usuario.Ativo;                        
                            tUsuario.DATA_ATUALIZACAO = DateTime.Now;
                            tUsuario.USUARIO_LOGADO = entradaUsuario.UsuarioLogado;
                            
                            if (entradaUsuario.Usuario.Perfis != null)
                            {
                                foreach (Contrato.Perfil perfil in entradaUsuario.Usuario.Perfis)
                                {
                                    tUsuario.T_USUARIO_PERFIL.Add(new Dados.USUARIO_PERFIL()
                                    {
                                        ID_USUARIO_PERFIL = Guid.NewGuid(),
                                        ID_USUARIO = entradaUsuario.Usuario.Id,
                                        ID_PERFIL = perfil.Id,
                                        LOGIN_USUARIO = entradaUsuario.UsuarioLogado,
                                        DATA_ATUALIZACAO = DateTime.Now
                                    });
                                }
                            }

                            context.AddToT_USUARIO(tUsuario);
                        }

                        // Salva as alterações
                        context.SaveChanges();

                        // Preenche o objeto de retorno
                        retUsuario.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    }
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retUsuario.Codigo = retSessao.Codigo;
                retUsuario.Mensagem = retSessao.Mensagem;
            }

            // retorna dos dados 
            return retUsuario;
        }

        /// <summary>
        /// Método para verificar se as informações do usuário logado foram preenchidas
        /// </summary>
        /// <param name="Usuario">Objeto com o dados do usuário</param>
        /// <returns>string</returns>
        private static string ValidarLoginUsuarioPreenchido(Contrato.Usuario Usuario)
        {
            // Cria a variável de retorno
            string strRetorno = string.Empty;

            // Verifica se o Login foi preenchido
            if (string.IsNullOrWhiteSpace(Usuario.Login))
                strRetorno = "O campo 'Login' não foi informado!\n";

            // Verifica se a Senha foi preenchida
            if (string.IsNullOrWhiteSpace(Usuario.Senha))
                strRetorno += "O campo 'Senha' não foi informado!\n";

            // retorna a variável de retorno
            return strRetorno;

        }

        /// <summary>
        /// Método para verificar se as informações do usuário foram preenchidas
        /// </summary>
        /// <param name="Usuario">Objeto com o dados do usuário</param>
        /// <returns>string</returns>
        private static string ValidarUsuarioPreenchido(Contrato.Usuario Usuario)
        {
            // Cria a variável de retorno
            string strRetorno = string.Empty;

            // Verifica se o Login foi preenchido
            if (string.IsNullOrWhiteSpace(Usuario.Login))
                strRetorno = "O campo 'Login' não foi informado!\n";

            // Verifica se o Nome foi preenchido
            if (string.IsNullOrWhiteSpace(Usuario.Nome))
                strRetorno = "O campo 'Nome' não foi informado!\n";

            // Verifica se a Senha foi preenchida
            if (string.IsNullOrWhiteSpace(Usuario.Senha))
                strRetorno += "O campo 'Senha' não foi informado!\n";

            // retorna a variável de retorno
            return strRetorno;

        }       
    }
}
