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
                // Verifica se o nome da taxa não foi informado
                if (string.IsNullOrWhiteSpace(entradaTaxa.Taxa.Nome))
                {
                    entradaTaxa.Taxa.Nome = string.Empty;
                }

                // Verifica se a empresa não foi informada
                if (string.IsNullOrWhiteSpace(entradaTaxa.EmpresaLogada.Id.ToString()))
                {
                    entradaTaxa.EmpresaLogada.Id = Guid.Empty;
                }

                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                List<Contrato.Taxa> lstTaxas = (from t in context.T_TAXA
                                                where
                                                    (t.BOL_ATIVO == entradaTaxa.Taxa.Ativo)
                                                 && (entradaTaxa.EmpresaLogada.Id == Guid.Empty || t.ID_EMPRESA == entradaTaxa.EmpresaLogada.Id)
                                                 && (entradaTaxa.Taxa.Nome == string.Empty || t.NOME_TAXA.Contains(entradaTaxa.Taxa.Nome))
                                                 && (!entradaTaxa.Taxa.Fornecedor.HasValue || t.BOL_FORNECEDOR != null && t.BOL_FORNECEDOR == entradaTaxa.Taxa.Fornecedor.Value)
                                                 && (!entradaTaxa.Taxa.Produto.HasValue || t.BOL_PRODUTO != null && t.BOL_PRODUTO == entradaTaxa.Taxa.Produto.Value)
                                                select new Contrato.Taxa
                                                {
                                                    Id = t.ID_TAXA,
                                                    Nome = t.NOME_TAXA,
                                                    Fornecedor = t.BOL_FORNECEDOR != null ? (bool)t.BOL_FORNECEDOR : false,
                                                    Produto = t.BOL_PRODUTO != null ? (bool)t.BOL_PRODUTO : false,
                                                    Desconto = t.BOL_DESCONTO != null ? (bool)t.BOL_DESCONTO : false,
                                                    Ativo = t.BOL_ATIVO
                                                }).ToList();

                // Verifica se foi encontrado algum registro
                if (lstTaxas.Count > 0)
                {
                    // Preenche o objeto de retorno
                    retTaxa.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    retTaxa.Taxas = lstTaxas;
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
        internal static List<Contrato.Taxa> ListarProdutoTaxa(System.Data.Objects.DataClasses.EntityCollection<Dados.PRODUTO_TAXA> lstProdutoTaxa, IEnumerable<Dados.TAXA> lstTaxa)
        {
            List<Contrato.Taxa> taxas = null;

            if (lstProdutoTaxa != null)
            {
                taxas = new List<Contrato.Taxa>();

                foreach (Dados.PRODUTO_TAXA taxa in lstProdutoTaxa)
                {
                    Dados.TAXA tx = lstTaxa.Where(t => t.ID_TAXA == taxa.ID_TAXA).FirstOrDefault();

                    taxas.Add(new Contrato.Taxa
                    {
                        Id = tx.ID_TAXA,
                        Nome = tx.NOME_TAXA,
                        Valor = taxa.NUM_VALOR,
                        Desconto = tx.BOL_DESCONTO,
                        Prioridade = taxa.ORD_PRIORIDADE,
                        Ativo = tx.BOL_ATIVO
                    });
                }
            }

            return taxas;
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
            string strValidacao = ValidarTaxaPreenchida(entradaTaxa.Taxa);

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
                                                          && (entradaTaxa.EmpresaLogada.Id == Guid.Empty || t.ID_EMPRESA == entradaTaxa.EmpresaLogada.Id))
                                                       || (entradaTaxa.Novo == null && entradaTaxa.Taxa.Id == t.ID_TAXA)
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
                        // Se existe a taxa
                        if (lstTaxas.Count > 0)
                        {
                            // Atualiza a taxa
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
                            // Cria a taxa
                            Dados.TAXA tTaxa = new Dados.TAXA();
                            tTaxa.ID_TAXA = Guid.NewGuid();
                            tTaxa.NOME_TAXA = entradaTaxa.Taxa.Nome;
                            tTaxa.ID_EMPRESA = entradaTaxa.EmpresaLogada.Id;
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
        internal static Contrato.RetornoTaxa SalvarTaxaProduto(Dados.PRODUTO Produto, string UsuarioLogado, Contrato.Taxa Taxa)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoTaxa retTaxa = new Contrato.RetornoTaxa();

            // Verifica se as informações do taxa foram informadas
            string strValidacao = ValidarTaxaPreenchida(Taxa);
                      
            // Se existe algum erro
            if (strValidacao.Length > 0)
            {
                retTaxa.Codigo = Contrato.Constantes.COD_FILTRO_VAZIO;
                retTaxa.Mensagem = strValidacao;
            }
            else
            {               
                // Cria a taxa para o produto
                Dados.PRODUTO_TAXA tProdutoTaxa = new Dados.PRODUTO_TAXA()
                                    {
                                        ID_PRODUTO_TAXA = Guid.NewGuid(),
                                        ID_PRODUTO = Produto.ID_PRODUTO,
                                        ID_TAXA = Taxa.Id,
                                        NUM_VALOR = Taxa.Valor,
                                        ORD_PRIORIDADE = Taxa.Prioridade,
                                        LOGIN_USUARIO = UsuarioLogado,
                                        DATA_ATUALIZACAO = DateTime.Now
                                    };

                Produto.T_PRODUTO_TAXA.Add(tProdutoTaxa);
                
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
        private static string ValidarTaxaPreenchida(Contrato.Taxa Taxa)
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
