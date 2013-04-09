using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrasilDidaticos.WcfServico;

namespace BrasilDidaticos.WcfServico.Negocio
{
    static class Taxa
    {
        /// <summary>
        /// Método para listar as taxas
        /// </summary>
        /// <param name="entradaTaxa.Taxas">Objeto com os dados do filtro</param>
        /// <returns>Contrato.RetornoTaxa</returns>
        internal static Contrato.RetornoTaxa ListarTaxa(Contrato.EntradaTaxa entradaTaxa)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoTaxa retTaxa = new Contrato.RetornoTaxa();
             
            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaTaxa.UsuarioLogado, Chave = entradaTaxa.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {

                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                // Busca o taxa no banco
                List<Dados.TAXA> lstTaxas = (from t in context.T_TAXA
                                                where 
                                                    (t.BOL_ATIVO == entradaTaxa.Taxa.Ativo)
                                                 && (!entradaTaxa.Taxa.Fornecedor.HasValue || t.BOL_FORNECEDOR != null && t.BOL_FORNECEDOR == entradaTaxa.Taxa.Fornecedor.Value)
                                                 && (!entradaTaxa.Taxa.Produto.HasValue || t.BOL_PRODUTO != null && t.BOL_PRODUTO == entradaTaxa.Taxa.Produto.Value)
                                                select t).ToList();

                // Verifica se foi encontrado algum registro
                if (lstTaxas.Count > 0)
                {
                    // Preenche o objeto de retorno
                    retTaxa.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    retTaxa.Taxas = new List<Contrato.Taxa>();
                    foreach (Dados.TAXA taxa in lstTaxas)
                    {
                        retTaxa.Taxas.Add(new Contrato.Taxa()
                        {
                            Id = taxa.ID_TAXA,
                            Nome = taxa.NOME_TAXA,
                            Fornecedor = taxa.BOL_FORNECEDOR!=null? (bool)taxa.BOL_FORNECEDOR:false,
                            Produto = taxa.BOL_PRODUTO != null ? (bool)taxa.BOL_PRODUTO : false,
                            Desconto = taxa.BOL_DESCONTO != null ? (bool)taxa.BOL_DESCONTO : false,
                            Ativo = taxa.BOL_ATIVO
                        });
                    };

                }
                else
                {
                    // Preenche o objeto de retorno
                    retTaxa.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;
                    retTaxa.Mensagem = "Não existe dados para o filtro informado.";
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retTaxa.Codigo = retSessao.Codigo;
                retTaxa.Mensagem = retSessao.Mensagem;
            }
            
            // retorna os dados
            return retTaxa;
        }

        /// <summary>
        /// Retorna uma lista de taxas 
        /// </summary>
        /// <param name="lstUsuarioTaxa">Recebe os taxas do fornecedor recuperado do banco</param>
        /// <returns>List<Contrato.Taxa></returns>
        internal static List<Contrato.Taxa> ListarFornecedorTaxa(System.Data.Objects.DataClasses.EntityCollection<Dados.FORNECEDOR_TAXA> lstFornecedorTaxa)
        {
            List<Contrato.Taxa> lstTaxa = null;

            if (lstFornecedorTaxa != null)
            {
                lstTaxa = new List<Contrato.Taxa>();

                foreach (Dados.FORNECEDOR_TAXA taxa in lstFornecedorTaxa)
                {
                    lstTaxa.Add(new Contrato.Taxa
                    {
                        Id = taxa.T_TAXA.ID_TAXA,
                        Nome = taxa.T_TAXA.NOME_TAXA,
                        Valor = taxa.NUM_VALOR,
                        Desconto = taxa.T_TAXA.BOL_DESCONTO,
                        Prioridade = taxa.ORD_PRIORIDADE,
                        Ativo = taxa.T_TAXA.BOL_ATIVO
                    });
                }
            }

            return lstTaxa;
        }

        /// <summary>
        /// Retorna uma lista de taxas
        /// </summary>
        /// <param name="lstUsuarioTaxa">Recebe os taxas do produto recuperado do banco</param>
        /// <returns>List<Contrato.Taxa></returns>
        internal static List<Contrato.Taxa> ListarProdutoTaxa(System.Data.Objects.DataClasses.EntityCollection<Dados.PRODUTO_TAXA> lstProdutoTaxa)
        {
            List<Contrato.Taxa> lstTaxa = null;

            if (lstProdutoTaxa != null)
            {
                lstTaxa = new List<Contrato.Taxa>();

                foreach (Dados.PRODUTO_TAXA taxa in lstProdutoTaxa)
                {
                    lstTaxa.Add(new Contrato.Taxa
                    {
                        Id = taxa.T_TAXA.ID_TAXA,
                        Nome = taxa.T_TAXA.NOME_TAXA,
                        Valor = taxa.NUM_VALOR,
                        Desconto = taxa.T_TAXA.BOL_DESCONTO,
                        Prioridade = taxa.ORD_PRIORIDADE,
                        Ativo = taxa.T_TAXA.BOL_ATIVO
                    });
                }
            }

            return lstTaxa;
        }

        /// <summary>
        /// Retorna uma lista de taxas
        /// </summary>
        /// <param name="lstUsuarioTaxa">Recebe os taxas do produto recuperado do banco</param>
        /// <returns>List<Contrato.Taxa></returns>
        internal static Contrato.Taxa BuscarProdutoTaxa(Dados.PRODUTO_TAXA produtoTaxa)
        {
            Contrato.Taxa taxa = null;

            if (produtoTaxa != null)
            {
                taxa = new Contrato.Taxa
                {
                    Id = produtoTaxa.T_TAXA.ID_TAXA,
                    Nome = produtoTaxa.T_TAXA.NOME_TAXA,
                    Valor = produtoTaxa.NUM_VALOR,
                    Desconto = produtoTaxa.T_TAXA.BOL_DESCONTO,
                    Prioridade = produtoTaxa.ORD_PRIORIDADE,
                    Ativo = produtoTaxa.T_TAXA.BOL_ATIVO
                };
            }

            return taxa;
        }

        /// <summary>
        /// Retorna uma lista de taxas
        /// </summary>
        /// <param name="lstUsuarioTaxa">Recebe os taxas do fornecedor recuperado do banco</param>
        /// <returns>List<Contrato.Taxa></returns>
        internal static Contrato.Taxa BuscarFornecedorTaxa(Dados.FORNECEDOR_TAXA fornecedorTaxa)
        {
            Contrato.Taxa taxa = null;

            if (fornecedorTaxa != null)
            {
                taxa = new Contrato.Taxa
                {
                    Id = fornecedorTaxa.T_TAXA.ID_TAXA,
                    Nome = fornecedorTaxa.T_TAXA.NOME_TAXA,
                    Valor = fornecedorTaxa.NUM_VALOR,
                    Desconto = fornecedorTaxa.T_TAXA.BOL_DESCONTO,
                    Prioridade = fornecedorTaxa.ORD_PRIORIDADE,
                    Ativo = fornecedorTaxa.T_TAXA.BOL_ATIVO
                };
            }

            return taxa;
        }

        /// <summary>
        /// Método para salvar o taxa
        /// </summary>
        /// <param name="Taxas">Objeto com os dados do taxa</param>
        /// <returns>Contrato.RetornoTaxa</returns>
        internal static Contrato.RetornoTaxa SalvarTaxa(Contrato.EntradaTaxa entradaTaxa)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoTaxa retTaxa = new Contrato.RetornoTaxa();

            // Verifica se as informações do taxa foram informadas
            string strValidacao = ValidarTaxaPreenchido(entradaTaxa.Taxa);

            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaTaxa.UsuarioLogado, Chave = entradaTaxa.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Se existe algum erro
                if (strValidacao.Length > 0)
                {
                    retTaxa.Codigo = Contrato.Constantes.COD_FILTRO_VAZIO;
                    retTaxa.Mensagem = strValidacao;
                }
                else
                {
                    // Loga no banco de dados
                    Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                    // Busca o taxa no banco
                    List<Dados.TAXA> lstTaxas = (from t in context.T_TAXA
                                                    where (t.NOME_TAXA == entradaTaxa.Taxa.Nome
                                                       || (entradaTaxa.Novo == null && entradaTaxa.Taxa.Id == t.ID_TAXA))
                                                    select t).ToList();

                     // Verifica se foi encontrado algum registro
                    if (lstTaxas.Count > 0 && entradaTaxa.Novo != null && (bool)entradaTaxa.Novo)
                    {
                        // Preenche o objeto de retorno
                        retTaxa.Codigo = Contrato.Constantes.COD_REGISTRO_DUPLICADO;
                        retTaxa.Mensagem = string.Format("A taxa de nome '{0}' já existe!", lstTaxas.First().NOME_TAXA);
                    }
                    else
                    {
                        // Se existe o taxa
                        if (lstTaxas.Count > 0)
                        {
                            // Atualiza o taxa
                            lstTaxas.First().NOME_TAXA = entradaTaxa.Taxa.Nome;
                            lstTaxas.First().BOL_DESCONTO = entradaTaxa.Taxa.Desconto;
                            lstTaxas.First().BOL_FORNECEDOR = entradaTaxa.Taxa.Fornecedor;
                            lstTaxas.First().BOL_PRODUTO = entradaTaxa.Taxa.Produto;
                            lstTaxas.First().BOL_ATIVO = entradaTaxa.Taxa.Ativo;
                            lstTaxas.First().DATA_ATUALIZACAO = DateTime.Now;
                            lstTaxas.First().LOGIN_USUARIO = entradaTaxa.UsuarioLogado;
                        }
                        else
                        {
                            // Cria o taxa
                            Dados.TAXA tTaxa = new Dados.TAXA();
                            tTaxa.ID_TAXA = Guid.NewGuid();
                            tTaxa.NOME_TAXA = entradaTaxa.Taxa.Nome;                            
                            tTaxa.BOL_DESCONTO = entradaTaxa.Taxa.Desconto;
                            tTaxa.BOL_FORNECEDOR = entradaTaxa.Taxa.Fornecedor;
                            tTaxa.BOL_PRODUTO = entradaTaxa.Taxa.Produto;
                            tTaxa.BOL_ATIVO = entradaTaxa.Taxa.Ativo;
                            tTaxa.DATA_ATUALIZACAO = DateTime.Now;
                            tTaxa.LOGIN_USUARIO = entradaTaxa.UsuarioLogado;

                            context.AddToT_TAXA(tTaxa);
                        }

                        // Salva as alterações
                        context.SaveChanges();

                        // Preenche o objeto de retorno
                        retTaxa.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    }
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retTaxa.Codigo = retSessao.Codigo;
                retTaxa.Mensagem = retSessao.Mensagem;
            }

            // retorna dos dados 
            return retTaxa;
        }

        /// <summary>
        /// Método para salvar o taxa do produto
        /// </summary>
        /// <param name="Taxas">Objeto com os dados do taxa</param>
        /// <returns>Contrato.RetornoTaxa</returns>
        internal static Contrato.RetornoTaxa SalvarTaxaProduto(Guid IdProduto, string UsuarioLogado, Contrato.Taxa Taxa)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoTaxa retTaxa = new Contrato.RetornoTaxa();

            // Verifica se as informações do taxa foram informadas
            string strValidacao = ValidarTaxaPreenchido(Taxa);
                      
            // Se existe algum erro
            if (strValidacao.Length > 0)
            {
                retTaxa.Codigo = Contrato.Constantes.COD_FILTRO_VAZIO;
                retTaxa.Mensagem = strValidacao;
            }
            else
            {
                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                // Busca o taxa do produto no banco
                List<Dados.PRODUTO_TAXA> lstTaxas = (from t in context.T_PRODUTO_TAXA
                                                     where (t.ID_TAXA == Taxa.Id && t.ID_PRODUTO == IdProduto)
                                                     select t).ToList();
                // Se existe a taxa
                if (lstTaxas.Count > 0)
                {
                    // Atualiza a taxa
                    lstTaxas.First().NUM_VALOR = Taxa.Valor;
                    lstTaxas.First().ORD_PRIORIDADE = Taxa.Prioridade;
                    lstTaxas.First().DATA_ATUALIZACAO = DateTime.Now;
                    lstTaxas.First().LOGIN_USUARIO = UsuarioLogado;
                }
                else
                {
                    // Cria o taxa
                    Dados.PRODUTO_TAXA tProdutoTaxa = new Dados.PRODUTO_TAXA()
                                        {
                                            ID_PRODUTO_TAXA = Guid.NewGuid(),
                                            ID_PRODUTO = IdProduto,
                                            ID_TAXA = Taxa.Id,
                                            NUM_VALOR = Taxa.Valor,
                                            ORD_PRIORIDADE = Taxa.Prioridade,
                                            LOGIN_USUARIO = UsuarioLogado,
                                            DATA_ATUALIZACAO = DateTime.Now
                                        };

                    context.AddToT_PRODUTO_TAXA(tProdutoTaxa);
                }

                // Salva as alterações
                context.SaveChanges();

                // Preenche o objeto de retorno
                retTaxa.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
            }

            // retorna dos dados 
            return retTaxa;
        }

        /// <summary>
        /// Método para verificar se as informações do taxa foram preenchidas
        /// </summary>
        /// <param name="Usuario">Objeto com o dados do taxa</param>
        /// <returns></returns>
        private static string ValidarTaxaPreenchido(Contrato.Taxa Taxa)
        {
            // Cria a variável de retorno
            string strRetorno = string.Empty;

            // Verifica se a Nome foi preenchida
            if (string.IsNullOrWhiteSpace(Taxa.Nome))
                strRetorno += "O campo 'Nome' não foi informado!\n";

            // retorna a variável de retorno
            return strRetorno;

        }       
    }
}
