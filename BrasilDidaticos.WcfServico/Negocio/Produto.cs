using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrasilDidaticos.WcfServico;

namespace BrasilDidaticos.WcfServico.Negocio
{
    static class Produto
    {
        /// <summary>
        /// Método para buscar o produto
        /// </summary>
        /// <param name="Fornecedor">Objeto com o identificador do produto</param>
        /// <returns>Contrato.RetornoProduto</returns>
        public static Contrato.Produto BuscarProduto(Dados.PRODUTO produto)
        {
            // Objeto que recebe o retorno do método
            Contrato.Produto retProduto = new Contrato.Produto();

            // Verifica se foi encontrado algum registro
            if (produto != null)
            {
                retProduto = new Contrato.Produto()
                {
                    Id = produto.ID_PRODUTO,
                    Nome = produto.NOME_PRODUTO,
                    Codigo = produto.COD_PRODUTO,
                    ValorBase = produto.NUM_VALOR,
                    Ncm = produto.NCM_PRODUTO,
                    Fornecedor = Negocio.Fornecedor.BuscarFornecedor(produto.T_FORNECEDOR),                    
                    Ativo = produto.BOL_ATIVO,                    
                    Taxas = Negocio.Taxa.ListarProdutoTaxa(produto.T_PRODUTO_TAXA)
                };
            }

            // retorna os dados
            return retProduto;
        }

        /// <summary>
        /// Método para listar os produtos
        /// </summary>
        /// <param name="Produto">Objeto com os dados do filtro</param>
        /// <returns>Contrato.RetornoProduto</returns>
        public static Contrato.RetornoProduto ListarProduto(Contrato.EntradaProduto entradaProduto)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoProduto retProduto = new Contrato.RetornoProduto();
            
            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaProduto.UsuarioLogado, Chave = entradaProduto.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Verifica se o código não foi informado
                if (string.IsNullOrWhiteSpace(entradaProduto.Produto.Codigo))
                {
                    entradaProduto.Produto.Codigo = string.Empty;
                }

                // Verifica se o nome não foi informado
                if (string.IsNullOrWhiteSpace(entradaProduto.Produto.Nome))
                {
                    entradaProduto.Produto.Nome = string.Empty;
                }

                // Verifica se o fornecedor não foi informado
                if (entradaProduto.Produto.Fornecedor == null)
                {
                    entradaProduto.Produto.Fornecedor = new Contrato.Fornecedor();
                }

                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();
                                                
                // Busca o produto no banco
                List<Dados.PRODUTO> lstProdutos = null;
                    
                if (entradaProduto.Paginar)
                {
                    lstProdutos = (from p in context.T_PRODUTO
                                    where 
                                    (p.BOL_ATIVO == entradaProduto.Produto.Ativo)
                                    && (entradaProduto.Produto.Codigo == string.Empty || p.COD_PRODUTO.StartsWith(entradaProduto.Produto.Codigo))
                                    && (entradaProduto.Produto.Nome == string.Empty || p.NOME_PRODUTO.ToLower().Contains(entradaProduto.Produto.Nome.ToLower()))
                                    && (entradaProduto.Produto.Fornecedor.Id == Guid.Empty || p.ID_FORNECEDOR == entradaProduto.Produto.Fornecedor.Id)
                                    select p
                                    ).OrderBy(o => o.NOME_PRODUTO).ThenBy(o => o.COD_PRODUTO).ThenBy(o => o.T_FORNECEDOR.NOME_FORNECEDOR).Skip(entradaProduto.PosicaoUltimoItem).Take(entradaProduto.CantidadeItens).ToList();
                }
                else
                {
                    lstProdutos = (from p in context.T_PRODUTO
                                   where
                                   (p.BOL_ATIVO == entradaProduto.Produto.Ativo)
                                   && (entradaProduto.Produto.Codigo == string.Empty || p.COD_PRODUTO.StartsWith(entradaProduto.Produto.Codigo))
                                   && (entradaProduto.Produto.Nome == string.Empty || p.NOME_PRODUTO.ToLower().Contains(entradaProduto.Produto.Nome.ToLower()))
                                   && (entradaProduto.Produto.Fornecedor.Id == Guid.Empty || p.ID_FORNECEDOR == entradaProduto.Produto.Fornecedor.Id)
                                   select p
                                    ).ToList();
                }
                                
                // Verifica se foi encontrado algum registro
                if (lstProdutos.Count > 0)
                {
                    // Preenche o objeto de retorno
                    retProduto.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    retProduto.Produtos = new List<Contrato.Produto>();
                    foreach (Dados.PRODUTO produto in lstProdutos)
                    {          
                        retProduto.Produtos.Add( new Contrato.Produto()
                        {
                            Id = produto.ID_PRODUTO,
                            Nome = produto.NOME_PRODUTO,
                            Codigo = produto.COD_PRODUTO,
                            ValorBase = produto.NUM_VALOR,
                            Ncm = produto.NCM_PRODUTO,
                            Ativo = produto.BOL_ATIVO,
                            Fornecedor = Negocio.Fornecedor.BuscarFornecedor(produto.T_FORNECEDOR),
                            Taxas = Negocio.Taxa.ListarProdutoTaxa(produto.T_PRODUTO_TAXA)
                        });
                    };
                }
                else
                {
                    // Preenche o objeto de retorno
                    retProduto.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;
                    retProduto.Mensagem = "Não existe dados para o filtro informado.";
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retProduto.Codigo = retSessao.Codigo;
                retProduto.Mensagem = retSessao.Mensagem;
            }
            
            // retorna os dados
            return retProduto;
        }

        /// <summary>
        /// Método para salvar o produto
        /// </summary>
        /// <param name="entradaProduto">Objeto com os dados do produto</param>
        /// <returns>Contrato.RetornoProduto</returns>
        public static Contrato.RetornoProduto SalvarProduto(Contrato.EntradaProduto entradaProduto)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoProduto retProduto = new Contrato.RetornoProduto();

            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaProduto.UsuarioLogado, Chave = entradaProduto.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Verifica se as informações do produto foram informadas
                string strValidacao = ValidarProdutoPreenchido(entradaProduto.Produto);
            
                // Se existe algum erro
                if (strValidacao.Length > 0)
                {
                    retProduto.Codigo = Contrato.Constantes.COD_FILTRO_VAZIO;
                    retProduto.Mensagem = strValidacao;
                }
                else
                {
                    // Loga no banco de dados
                    Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                    // Busca o produto no banco
                    List<Dados.PRODUTO> lstProdutos = (from p in context.T_PRODUTO
                                                       where (p.COD_PRODUTO == entradaProduto.Produto.Codigo
                                                          || (entradaProduto.Novo == null && entradaProduto.Produto.Id == p.ID_PRODUTO))
                                                       select p).ToList();

                    // Verifica se foi encontrado algum registro
                    if (lstProdutos.Count > 0 && entradaProduto.Novo != null && (bool)entradaProduto.Novo)
                    {
                        // Preenche o objeto de retorno
                        retProduto.Codigo = Contrato.Constantes.COD_REGISTRO_DUPLICADO;
                        retProduto.Mensagem = string.Format("O produto de código '{0}' já existe!", lstProdutos.First().COD_PRODUTO);
                    }
                    else
                    {
                        // Se existe o produto
                        if (lstProdutos.Count > 0)
                        {
                            // Atualiza o produto                            
                            lstProdutos.First().NOME_PRODUTO = entradaProduto.Produto.Nome;
                            lstProdutos.First().NUM_VALOR = entradaProduto.Produto.ValorBase;
                            lstProdutos.First().ID_FORNECEDOR = entradaProduto.Produto.Fornecedor.Id;
                            lstProdutos.First().NCM_PRODUTO = entradaProduto.Produto.Ncm;
                            lstProdutos.First().BOL_ATIVO = entradaProduto.Produto.Ativo;
                            lstProdutos.First().DATA_ATUALIZACAO = DateTime.Now;
                            lstProdutos.First().LOGIN_USUARIO = entradaProduto.UsuarioLogado;

                            // Apaga todos as taxas que estão relacionados
                            while (lstProdutos.First().T_PRODUTO_TAXA.Count > 0)
                            {
                                context.T_PRODUTO_TAXA.DeleteObject(lstProdutos.First().T_PRODUTO_TAXA.First());
                            }

                            // Verifica se existe alguma taxa  associado ao usuário
                            if (entradaProduto.Produto.Taxas != null)
                            {
                                // Para cada perfil associado
                                foreach (Contrato.Taxa taxa in entradaProduto.Produto.Taxas)
                                {
                                    // Associa a taxa ao fornecedor
                                    lstProdutos.First().T_PRODUTO_TAXA.Add(new Dados.PRODUTO_TAXA()
                                    {
                                        ID_PRODUTO_TAXA = Guid.NewGuid(),
                                        ID_PRODUTO = entradaProduto.Produto.Id,
                                        ID_TAXA = taxa.Id,
                                        NUM_VALOR = taxa.Valor,
                                        ORD_PRIORIDADE = taxa.Prioridade,
                                        LOGIN_USUARIO = entradaProduto.UsuarioLogado,
                                        DATA_ATUALIZACAO = DateTime.Now
                                    });
                                }
                            }
                        }
                        else
                        {
                            // Cria o produto
                            Dados.PRODUTO tProduto = new Dados.PRODUTO();
                            tProduto.ID_PRODUTO = Guid.NewGuid();
                            tProduto.COD_PRODUTO = entradaProduto.Produto.Codigo;
                            tProduto.NOME_PRODUTO = entradaProduto.Produto.Nome;
                            tProduto.ID_FORNECEDOR = entradaProduto.Produto.Fornecedor.Id;
                            tProduto.NCM_PRODUTO = entradaProduto.Produto.Ncm;
                            tProduto.NUM_VALOR = entradaProduto.Produto.ValorBase;
                            tProduto.BOL_ATIVO = entradaProduto.Produto.Ativo;
                            tProduto.DATA_ATUALIZACAO = DateTime.Now;
                            tProduto.LOGIN_USUARIO = entradaProduto.UsuarioLogado;

                            // Verifica se existe algum perfil associado ao usuário
                            if (entradaProduto.Produto.Taxas != null)
                            {
                                // Para cada perfil associado
                                foreach (Contrato.Taxa taxa in entradaProduto.Produto.Taxas)
                                {
                                    // Associa a taxa ao produto
                                    tProduto.T_PRODUTO_TAXA.Add(new Dados.PRODUTO_TAXA()
                                    {
                                        ID_PRODUTO_TAXA = Guid.NewGuid(),
                                        ID_PRODUTO = entradaProduto.Produto.Id,
                                        ID_TAXA = taxa.Id,
                                        NUM_VALOR = taxa.Valor,
                                        ORD_PRIORIDADE = taxa.Prioridade,
                                        LOGIN_USUARIO = entradaProduto.UsuarioLogado,
                                        DATA_ATUALIZACAO = DateTime.Now
                                    });
                                }
                            }    

                            context.AddToT_PRODUTO(tProduto);
                        }

                        // Salva as alterações
                        context.SaveChanges();

                        // Preenche o objeto de retorno
                        retProduto.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    }
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retProduto.Codigo = retSessao.Codigo;
                retProduto.Mensagem = retSessao.Mensagem;
            }

            // retorna dos dados 
            return retProduto;
        }

        /// <summary>
        /// Método para salvar o produto
        /// </summary>
        /// <param name="entradaProduto">Objeto com os dados do produto</param>
        /// <returns>Contrato.RetornoProduto</returns>
        public static Contrato.RetornoProduto SalvarProdutos(Contrato.EntradaProdutos entradaProdutos)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoProduto retProduto = new Contrato.RetornoProduto();

            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaProdutos.UsuarioLogado, Chave = entradaProdutos.Chave });

            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {

                if (entradaProdutos.Produtos != null)
                {
                    // Loga no banco de dados
                    Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                    // Se o identificador do fornecedor está vazio
                    if (entradaProdutos.Fornecedor.Id == Guid.Empty)
                        // Busca os dados do fornecedor
                        entradaProdutos.Fornecedor = Fornecedor.BuscarFornecedor((from f in context.T_FORNECEDOR where f.COD_FORNECEDOR == entradaProdutos.Fornecedor.Codigo select f).FirstOrDefault());

                    // Para cada produto existente na lista
                    foreach (Contrato.Produto produto in entradaProdutos.Produtos)
                    {

                        // Verifica se as informações do produto foram informadas
                        string strValidacao = ValidarProdutoPreenchido(produto);

                        // Se existe algum erro
                        if (strValidacao.Length > 0)
                        {
                            retProduto.Codigo = Contrato.Constantes.COD_FILTRO_VAZIO;
                            retProduto.Mensagem = strValidacao;
                        }
                        else
                        {                        
                            // Busca o produto no banco
                            List<Dados.PRODUTO> lstProdutos = (from p in context.T_PRODUTO
                                                               where 
                                                                    p.COD_PRODUTO == produto.Codigo
                                                               select p).ToList();

                            // Se existe o produto
                            if (lstProdutos.Count > 0)
                            {
                                // Atualiza o produto
                                lstProdutos.First().NUM_VALOR = produto.ValorBase;
                                lstProdutos.First().ID_FORNECEDOR = entradaProdutos.Fornecedor.Id;
                                lstProdutos.First().NCM_PRODUTO = produto.Ncm;
                                lstProdutos.First().BOL_ATIVO = (bool)entradaProdutos.Fornecedor.Ativo;
                                lstProdutos.First().DATA_ATUALIZACAO = DateTime.Now;
                                lstProdutos.First().LOGIN_USUARIO = entradaProdutos.UsuarioLogado;

                                // Verifica se existe alguma taxa associada ao produto
                                if (entradaProdutos.Fornecedor.Taxas != null)
                                {
                                    // Para cada perfil associado
                                    foreach (Contrato.Taxa taxa in entradaProdutos.Fornecedor.Taxas)
                                    {
                                        Negocio.Taxa.SalvarTaxaProduto(lstProdutos.First().ID_PRODUTO, entradaProdutos.UsuarioLogado, taxa);                                        
                                    }
                                }
                            }
                            else
                            {
                                // Cria o produto
                                Dados.PRODUTO tProduto = new Dados.PRODUTO();
                                tProduto.ID_PRODUTO = Guid.NewGuid();
                                tProduto.COD_PRODUTO = produto.Codigo;
                                tProduto.NOME_PRODUTO = produto.Nome;
                                tProduto.ID_FORNECEDOR = entradaProdutos.Fornecedor.Id;
                                tProduto.NUM_VALOR = produto.ValorBase;
                                tProduto.NCM_PRODUTO = produto.Ncm;
                                tProduto.BOL_ATIVO = produto.Ativo;
                                tProduto.DATA_ATUALIZACAO = DateTime.Now;
                                tProduto.LOGIN_USUARIO = entradaProdutos.UsuarioLogado;

                                // Verifica se existe alguma taxa associada ao produto
                                if (entradaProdutos.Fornecedor.Taxas != null)
                                {
                                    // Para cada perfil associado
                                    foreach (Contrato.Taxa taxa in entradaProdutos.Fornecedor.Taxas)
                                    {
                                        // Associa a taxa ao produto
                                        tProduto.T_PRODUTO_TAXA.Add(new Dados.PRODUTO_TAXA()
                                        {
                                            ID_PRODUTO_TAXA = Guid.NewGuid(),
                                            ID_PRODUTO = produto.Id,
                                            ID_TAXA = taxa.Id,
                                            NUM_VALOR = taxa.Valor,
                                            ORD_PRIORIDADE = taxa.Prioridade,
                                            LOGIN_USUARIO = entradaProdutos.UsuarioLogado,
                                            DATA_ATUALIZACAO = DateTime.Now
                                        });
                                    }
                                }

                                context.AddToT_PRODUTO(tProduto);
                            }
                        }                                          
                    }

                    // Salva as alterações
                    context.SaveChanges();

                    // Preenche o objeto de retorno
                    retProduto.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO; 
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retProduto.Codigo = retSessao.Codigo;
                retProduto.Mensagem = retSessao.Mensagem;
            }

            // retorna dos dados 
            return retProduto;
        }

        /// <summary>
        /// Método para verificar se as informações do produto foram preenchidas
        /// </summary>
        /// <param name="Usuario">Objeto com o dados do produto</param>
        /// <returns></returns>
        private static string ValidarProdutoPreenchido(Contrato.Produto Produto)
        {
            // Cria a variável de retorno
            string strRetorno = string.Empty;

            // Verifica se o Codigo foi preenchido
            if (string.IsNullOrWhiteSpace(Produto.Codigo))
                strRetorno = "O campo 'Codigo' não foi informado!\n";

            // Verifica se a Nome foi preenchida
            if (string.IsNullOrWhiteSpace(Produto.Nome))
                strRetorno += "O campo 'Nome' não foi informado!\n";

            // retorna a variável de retorno
            return strRetorno;
        }       
    }
}
