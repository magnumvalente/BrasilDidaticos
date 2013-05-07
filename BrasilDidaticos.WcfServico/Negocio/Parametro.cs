using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrasilDidaticos.WcfServico;

namespace BrasilDidaticos.WcfServico.Negocio
{
    static class Parametro
    {
        /// <summary>
        /// Método para listar os parâmetros
        /// </summary>
        /// <param name="entradaParametro.Parametros">Objeto com os dados do filtro</param>
        /// <returns>Contrato.RetornoParametro</returns>
        internal static Contrato.RetornoParametro ListarParametro(Contrato.EntradaParametro entradaParametro)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoParametro retParametro = new Contrato.RetornoParametro();
             
            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaParametro.UsuarioLogado, Chave = entradaParametro.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Verifica se a empresa não foi informada
                if (string.IsNullOrWhiteSpace(entradaParametro.EmpresaLogada.Id.ToString()))
                {
                    entradaParametro.EmpresaLogada.Id = Guid.Empty;
                }

                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                // Busca o parametro no banco
                List<Dados.PARAMETRO> lstParametros = (from p in context.T_PARAMETRO
                                                       where (entradaParametro.EmpresaLogada.Id == Guid.Empty || p.ID_EMPRESA == entradaParametro.EmpresaLogada.Id)
                                                       select p).ToList();

                // Verifica se foi encontrado algum registro
                if (lstParametros.Count > 0)
                {
                    // Preenche o objeto de retorno
                    retParametro.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    retParametro.Parametros = new List<Contrato.Parametro>();
                    foreach (Dados.PARAMETRO parametro in lstParametros)
                    {
                        retParametro.Parametros.Add(new Contrato.Parametro()
                        {
                            Id = parametro.ID_PARAMETRO,
                            Codigo = parametro.COD_PARAMETRO,
                            Nome = parametro.NOME_PARAMETRO,
                            Valor = parametro.VALOR_PARAMETRO,
                            TipoParametro = RecuperarTipoParametro(parametro.TIPO_PARAMETRO),
                            Ordem = parametro.NUM_ORDEM
                        });
                    };

                }
                else
                {
                    // Preenche o objeto de retorno
                    retParametro.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;
                    retParametro.Mensagem = "Não existe dados para o filtro informado.";
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retParametro.Codigo = retSessao.Codigo;
                retParametro.Mensagem = retSessao.Mensagem;
            }
            
            // retorna os dados
            return retParametro;
        }

        /// <summary>
        /// Método para salvar o parametro
        /// </summary>
        /// <param name="Parametros">Objeto com os dados do parametro</param>
        /// <returns>Contrato.RetornoParametro</returns>
        internal static Contrato.RetornoParametro SalvarParametros(Contrato.EntradaParametros entradaParametros)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoParametro retParametro = new Contrato.RetornoParametro();
             
            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaParametros.UsuarioLogado, Chave = entradaParametros.Chave });
                
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Verifica se existe Parametros
                if (entradaParametros.Parametros != null)
                {
                    // Para cada parâmetro existente
                    foreach (Contrato.Parametro param in entradaParametros.Parametros)
                    {
                        // Salva o parâmetro
                        Contrato.RetornoParametro retParam = SalvarParametro(param, entradaParametros.UsuarioLogado, entradaParametros.EmpresaLogada.Id);
                        
                        // Verifica se o parâmetro foi salvo com sucesso
                        if (retParam.Codigo != Contrato.Constantes.COD_RETORNO_SUCESSO ) 
                            retParametro.Mensagem += string.Format("O parâmetro {0} não foi cadastrado! \n", param.Nome);                        
                    }
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retParametro.Codigo = retSessao.Codigo;
                retParametro.Mensagem = retSessao.Mensagem;
            }           

            // retorna dos dados 
            return retParametro;
        }

        /// <summary>
        /// Método para salvar o parametro
        /// </summary>
        /// <param name="Parametro">Objeto com os dados do parametro</param>
        /// <param name="usuarioLogado">Nome do usuário Logado</param>
        /// <returns>Contrato.RetornoParametro</returns>
        internal static Contrato.RetornoParametro SalvarParametro(Contrato.Parametro Parametro, string usuarioLogado, Guid idEmpresa)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoParametro retParametro = new Contrato.RetornoParametro();

            // Verifica se as informações do parametro foram informadas
            string strValidacao = ValidarParametroPreenchido(Parametro);                                 

            // Se existe algum erro
            if (strValidacao.Length > 0)
            {
                retParametro.Codigo = Contrato.Constantes.COD_FILTRO_VAZIO;
                retParametro.Mensagem = strValidacao;
            }
            else
            {
                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                // Busca o parametro no banco
                List<Dados.PARAMETRO> lstParametros = (from p in context.T_PARAMETRO
                                                       where 
                                                            (p.COD_PARAMETRO == Parametro.Codigo)
                                                         && (idEmpresa == Guid.Empty || p.ID_EMPRESA == idEmpresa)
                                                       select p).ToList();

                // Se existe o parametro
                if (lstParametros.Count > 0)
                {
                    // Atualiza o parametro                    
                    lstParametros.First().VALOR_PARAMETRO = Parametro.Valor;
                    lstParametros.First().DATA_ATUALIZACAO = DateTime.Now;
                    lstParametros.First().LOGIN_USUARIO = usuarioLogado;
                }

                // Salva as alterações
                context.SaveChanges();

                // Preenche o objeto de retorno
                retParametro.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;                
            }

            return retParametro;
        }

        /// <summary>
        /// Método para verificar se as informações do parametro foram preenchidas
        /// </summary>
        /// <param name="Usuario">Objeto com o dados do parametro</param>
        /// <returns></returns>
        private static string ValidarParametroPreenchido(Contrato.Parametro Parametro)
        {
            // Cria a variável de retorno
            string strRetorno = string.Empty;

            // Verifica se o Codigo foi preenchido
            if (string.IsNullOrWhiteSpace(Parametro.Codigo))
                strRetorno = "O campo 'Codigo' não foi informado!\n";

            // Verifica se o Nome foi preenchido
            if (string.IsNullOrWhiteSpace(Parametro.Nome))
                strRetorno += "O campo 'Nome' não foi informado!\n";

            // Verifica se o Valor foi preenchido
            if (string.IsNullOrWhiteSpace(Parametro.Valor))
                strRetorno += "O campo 'Valor' não foi informado!\n";

            // retorna a variável de retorno
            return strRetorno;
        }

        private static Contrato.Enumeradores.TipoParametro RecuperarTipoParametro(string tipoParametro)
        {
            switch (tipoParametro)
            {
                case "Texto":
                    return Contrato.Enumeradores.TipoParametro.Texto;
                case "Decimal":
                    return Contrato.Enumeradores.TipoParametro.Decimal;
                case "DataHora":
                    return Contrato.Enumeradores.TipoParametro.DataHora;
                case "Inteiro":
                    return Contrato.Enumeradores.TipoParametro.Inteiro;
                case "Percentagem":
                    return Contrato.Enumeradores.TipoParametro.Percentagem;
                case "Cor":
                    return Contrato.Enumeradores.TipoParametro.Cor;
                default:
                    return Contrato.Enumeradores.TipoParametro.Binario;
            }
        }
    }
}