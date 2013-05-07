using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrasilDidaticos.WcfServico;

namespace BrasilDidaticos.WcfServico.Negocio
{
    static class UnidadeMedida
    {
        /// <summary>
        /// Método para listar as unidade de medidas
        /// </summary>
        /// <param name="entradaUnidadeMedida.UnidadeMedidas">Objeto com os dados do filtro</param>
        /// <returns>Contrato.RetornoUnidadeMedida</returns>
        internal static Contrato.RetornoUnidadeMedida ListarUnidadeMedida(Contrato.EntradaUnidadeMedida entradaUnidadeMedida)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoUnidadeMedida retUnidadeMedida = new Contrato.RetornoUnidadeMedida();
             
            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaUnidadeMedida.UsuarioLogado, Chave = entradaUnidadeMedida.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Verifica se a empresa não foi informada
                if (string.IsNullOrWhiteSpace(entradaUnidadeMedida.EmpresaLogada.Id.ToString()))
                {
                    entradaUnidadeMedida.EmpresaLogada.Id = Guid.Empty;
                }

                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                // Busca o unidademedida no banco
                List<Dados.UNIDADE_MEDIDA> lstUnidadeMedidas = (from um in context.T_UNIDADE_MEDIDA
                                                where 
                                                    (um.BOL_ATIVO == entradaUnidadeMedida.UnidadeMedida.Ativo)
                                                 && (entradaUnidadeMedida.UnidadeMedida.Codigo == null || entradaUnidadeMedida.UnidadeMedida.Codigo == string.Empty || um.COD_UNIDADE_MEDIDA.Contains(entradaUnidadeMedida.UnidadeMedida.Codigo))
                                                 && (entradaUnidadeMedida.UnidadeMedida.Nome == null || entradaUnidadeMedida.UnidadeMedida.Nome == string.Empty || um.NOME_UNIDADE_MEDIDA.Contains(entradaUnidadeMedida.UnidadeMedida.Nome))
                                                 && (entradaUnidadeMedida.EmpresaLogada.Id == Guid.Empty || um.ID_EMPRESA == entradaUnidadeMedida.EmpresaLogada.Id)
                                                 select um).ToList();

                // Verifica se foi encontrado algum registro
                if (lstUnidadeMedidas.Count > 0)
                {
                    // Preenche o objeto de retorno
                    retUnidadeMedida.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    retUnidadeMedida.UnidadeMedidas = new List<Contrato.UnidadeMedida>();
                    foreach (Dados.UNIDADE_MEDIDA unidademedida in lstUnidadeMedidas)
                    {
                        retUnidadeMedida.UnidadeMedidas.Add(new Contrato.UnidadeMedida()
                        {
                            Id = unidademedida.ID_UNIDADE_MEDIDA,
                            Codigo = unidademedida.COD_UNIDADE_MEDIDA,
                            Nome = unidademedida.NOME_UNIDADE_MEDIDA,
                            Descricao = unidademedida.DES_UNIDADE_MEDIDA,                            
                            Ativo = unidademedida.BOL_ATIVO
                        });
                    };

                }
                else
                {
                    // Preenche o objeto de retorno
                    retUnidadeMedida.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;
                    retUnidadeMedida.Mensagem = "Não existe dados para o filtro informado.";
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retUnidadeMedida.Codigo = retSessao.Codigo;
                retUnidadeMedida.Mensagem = retSessao.Mensagem;
            }
            
            // retorna os dados
            return retUnidadeMedida;
        }

        /// <summary>
        /// Retorna uma lista de unidade de medidas
        /// </summary>
        /// <param name="lstUsuarioUnidadeMedida">Recebe as unidade de medidas do produto recuperado do banco</param>
        /// <returns>List<Contrato.UnidadeMedida></returns>
        internal static List<Contrato.UnidadeMedida> ListarProdutoUnidadeMedida(System.Data.Objects.DataClasses.EntityCollection<Dados.PRODUTO_UNIDADE_MEDIDA> lstProdutoUnidadeMedida)
        {
            List<Contrato.UnidadeMedida> lstUnidadeMedida = null;

            if (lstProdutoUnidadeMedida != null)
            {
                lstUnidadeMedida = new List<Contrato.UnidadeMedida>();

                foreach (Dados.PRODUTO_UNIDADE_MEDIDA unidademedida in lstProdutoUnidadeMedida)
                {
                    lstUnidadeMedida.Add(new Contrato.UnidadeMedida
                    {
                        Id = unidademedida.T_UNIDADE_MEDIDA.ID_UNIDADE_MEDIDA,
                        Nome = unidademedida.T_UNIDADE_MEDIDA.NOME_UNIDADE_MEDIDA,
                        Quantidade = unidademedida.NUM_QUANTIDADE
                    });
                }
            }

            return lstUnidadeMedida;
        }

        /// <summary>
        /// Retorna uma lista de unidadeMedidas
        /// </summary>
        /// <param name="lstUsuarioUnidadeMedida">Recebe os unidadeMedidas do produto recuperado do banco</param>
        /// <returns>List<Contrato.UnidadeMedida></returns>
        //internal static Contrato.UnidadeMedida BuscarProdutoUnidadeMedida(Dados.PRODUTO_UNIDADE_MEDIDA produtoUnidadeMedida)
        //{
        //    Contrato.UnidadeMedida unidademedida = null;

        //    if (produtoUnidadeMedida != null)
        //    {
        //        unidademedida = new Contrato.UnidadeMedida
        //        {
        //            Id = produtoUnidadeMedida.T_UNIDADE_MEDIDA.ID_UNIDADE_MEDIDA,
        //            Nome = produtoUnidadeMedida.T_UNIDADE_MEDIDA.NOME_UNIDADE_MEDIDA,
        //            Descricao = produtoUnidadeMedida.T_UNIDADE_MEDIDA.DES_UNIDADE_MEDIDA,
        //            Ativo = produtoUnidadeMedida.T_UNIDADE_MEDIDA.BOL_ATIVO
        //        };
        //    }

        //    return unidademedida;
        //}

        /// <summary>
        /// Método para salvar a unidade de medida
        /// </summary>
        /// <param name="UnidadeMedidas">Objeto com os dados da unidade de medida</param>
        /// <returns>Contrato.RetornoUnidadeMedida</returns>
        internal static Contrato.RetornoUnidadeMedida SalvarUnidadeMedida(Contrato.EntradaUnidadeMedida entradaUnidadeMedida)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoUnidadeMedida retUnidadeMedida = new Contrato.RetornoUnidadeMedida();

            // Verifica se as informações do unidademedida foram informadas
            string strValidacao = ValidarUnidadeMedidaPreenchido(entradaUnidadeMedida.UnidadeMedida);

            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaUnidadeMedida.UsuarioLogado, Chave = entradaUnidadeMedida.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Se existe algum erro
                if (strValidacao.Length > 0)
                {
                    retUnidadeMedida.Codigo = Contrato.Constantes.COD_FILTRO_VAZIO;
                    retUnidadeMedida.Mensagem = strValidacao;
                }
                else
                {
                    // Loga no banco de dados
                    Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                    // Busca o unidademedida no banco
                    List<Dados.UNIDADE_MEDIDA> lstUnidadeMedidas = (from um in context.T_UNIDADE_MEDIDA
                                                    where (um.COD_UNIDADE_MEDIDA == entradaUnidadeMedida.UnidadeMedida.Codigo
                                                       && (entradaUnidadeMedida.EmpresaLogada.Id == Guid.Empty || um.ID_EMPRESA == entradaUnidadeMedida.EmpresaLogada.Id))
                                                       || (entradaUnidadeMedida.Novo == null && entradaUnidadeMedida.UnidadeMedida.Id == um.ID_UNIDADE_MEDIDA)
                                                    select um).ToList();

                     // Verifica se foi encontrado algum registro
                    if (lstUnidadeMedidas.Count > 0 && entradaUnidadeMedida.Novo != null && (bool)entradaUnidadeMedida.Novo)
                    {
                        // Preenche o objeto de retorno
                        retUnidadeMedida.Codigo = Contrato.Constantes.COD_REGISTRO_DUPLICADO;
                        retUnidadeMedida.Mensagem = string.Format("A unidade de medida de código '{0}' já existe!", lstUnidadeMedidas.First().COD_UNIDADE_MEDIDA);
                    }
                    else
                    {
                        // Se existe o unidademedida
                        if (lstUnidadeMedidas.Count > 0)
                        {
                            // Atualiza o unidademedida
                            lstUnidadeMedidas.First().NOME_UNIDADE_MEDIDA = entradaUnidadeMedida.UnidadeMedida.Nome;
                            lstUnidadeMedidas.First().DES_UNIDADE_MEDIDA = entradaUnidadeMedida.UnidadeMedida.Descricao;
                            lstUnidadeMedidas.First().BOL_ATIVO = entradaUnidadeMedida.UnidadeMedida.Ativo;
                            lstUnidadeMedidas.First().DATA_ATUALIZACAO = DateTime.Now;
                            lstUnidadeMedidas.First().LOGIN_USUARIO = entradaUnidadeMedida.UsuarioLogado;
                        }
                        else
                        {
                            // Cria o unidademedida
                            Dados.UNIDADE_MEDIDA tUnidadeMedida = new Dados.UNIDADE_MEDIDA();
                            tUnidadeMedida.ID_UNIDADE_MEDIDA = Guid.NewGuid();
                            tUnidadeMedida.COD_UNIDADE_MEDIDA = entradaUnidadeMedida.UnidadeMedida.Codigo;
                            tUnidadeMedida.NOME_UNIDADE_MEDIDA = entradaUnidadeMedida.UnidadeMedida.Nome;
                            tUnidadeMedida.DES_UNIDADE_MEDIDA = entradaUnidadeMedida.UnidadeMedida.Descricao;
                            tUnidadeMedida.ID_EMPRESA = entradaUnidadeMedida.EmpresaLogada.Id;
                            tUnidadeMedida.BOL_ATIVO = entradaUnidadeMedida.UnidadeMedida.Ativo;
                            tUnidadeMedida.DATA_ATUALIZACAO = DateTime.Now;
                            tUnidadeMedida.LOGIN_USUARIO = entradaUnidadeMedida.UsuarioLogado;

                            context.AddToT_UNIDADE_MEDIDA(tUnidadeMedida);
                        }

                        // Salva as alterações
                        context.SaveChanges();

                        // Preenche o objeto de retorno
                        retUnidadeMedida.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    }
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retUnidadeMedida.Codigo = retSessao.Codigo;
                retUnidadeMedida.Mensagem = retSessao.Mensagem;
            }

            // retorna dos dados 
            return retUnidadeMedida;
        }

        /// <summary>
        /// Método para salvar o unidade de medida do produto
        /// </summary>
        /// <param name="UnidadeMedidas">Objeto com os dados do unidade de medida</param>
        /// <returns>Contrato.RetornoUnidadeMedida</returns>
        internal static Contrato.RetornoUnidadeMedida SalvarUnidadeMedidaProduto(Guid IdProduto, string UsuarioLogado, Contrato.UnidadeMedida UnidadeMedida)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoUnidadeMedida retUnidadeMedida = new Contrato.RetornoUnidadeMedida();

            // Verifica se as informações do unidademedida foram informadas
            string strValidacao = ValidarUnidadeMedidaPreenchido(UnidadeMedida);

            // Se existe algum erro
            if (strValidacao.Length > 0)
            {
                retUnidadeMedida.Codigo = Contrato.Constantes.COD_FILTRO_VAZIO;
                retUnidadeMedida.Mensagem = strValidacao;
            }
            else
            {
                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                // Busca o unidademedida do produto no banco
                List<Dados.PRODUTO_UNIDADE_MEDIDA> lstUnidadeMedidas = (from t in context.T_PRODUTO_UNIDADE_MEDIDA
                                                                where (t.ID_UNIDADE_MEDIDA == UnidadeMedida.Id && t.ID_PRODUTO == IdProduto)
                                                                select t).ToList();
                // Se existe a unidade de medida
                if (lstUnidadeMedidas.Count > 0)
                {
                    // Atualiza a unidade de medida
                    lstUnidadeMedidas.First().NUM_QUANTIDADE = UnidadeMedida.Quantidade;                    
                    lstUnidadeMedidas.First().DATA_ATUALIZACAO = DateTime.Now;
                    lstUnidadeMedidas.First().LOGIN_USUARIO = UsuarioLogado;
                }
                else
                {
                    // Cria a unidade de medida
                    Dados.PRODUTO_UNIDADE_MEDIDA tProdutoUnidadeMedida = new Dados.PRODUTO_UNIDADE_MEDIDA()
                                        {
                                            ID_PRODUTO_UNIDADE_MEDIDA = Guid.NewGuid(),
                                            ID_PRODUTO = IdProduto,
                                            ID_UNIDADE_MEDIDA = UnidadeMedida.Id,
                                            NUM_QUANTIDADE = UnidadeMedida.Quantidade,
                                            LOGIN_USUARIO = UsuarioLogado,
                                            DATA_ATUALIZACAO = DateTime.Now
                                        };

                    context.AddToT_PRODUTO_UNIDADE_MEDIDA(tProdutoUnidadeMedida);
                }

                // Salva as alterações
                context.SaveChanges();

                // Preenche o objeto de retorno
                retUnidadeMedida.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
            }

            // retorna dos dados 
            return retUnidadeMedida;
        }

        /// <summary>
        /// Método para verificar se as informações do unidademedida foram preenchidas
        /// </summary>
        /// <param name="Usuario">Objeto com o dados do unidademedida</param>
        /// <returns></returns>
        private static string ValidarUnidadeMedidaPreenchido(Contrato.UnidadeMedida UnidadeMedida)
        {
            // Cria a variável de retorno
            string strRetorno = string.Empty;

            // Verifica se a Nome foi preenchida
            if (string.IsNullOrWhiteSpace(UnidadeMedida.Nome))
                strRetorno += "O campo 'Nome' não foi informado!\n";

            // retorna a variável de retorno
            return strRetorno;

        }       
    }
}
