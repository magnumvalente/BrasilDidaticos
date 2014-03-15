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
        /// Método para buscar o código do produto
        /// </summary>        
        /// <returns>string</returns>
        internal static string BuscarCodigoProduto(Guid IdEmpresa)
        {
            // Objeto que recebe o retorno do método
            string retCodigoProduto = string.Empty;

            // Loga no banco de dados
            Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();
            System.Data.Objects.ObjectParameter objCodigoProduto = new System.Data.Objects.ObjectParameter("P_CODIGO", typeof(global::System.Int32));
            context.RETORNAR_CODIGO(Contrato.Constantes.TIPO_COD_PRODUTO, IdEmpresa, objCodigoProduto);

            // Recupera o código do produto
            retCodigoProduto = Util.RecuperaCodigo((int)objCodigoProduto.Value, Contrato.Constantes.TIPO_COD_PRODUTO);


            // retorna os dados
            return retCodigoProduto;
        }

        /// <summary>
        /// Método para buscar o produto
        /// </summary>
        /// <param name="Fornecedor">Objeto com o identificador do produto</param>
        /// <returns>Contrato.RetornoProduto</returns>
        internal static Contrato.Produto BuscarProduto(Dados.PRODUTO produto)
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
                    CodigoBarras = produto.COD_BARRAS,
                    CodigoFornecedor = produto.COD_PRODUTO_FORNECEDOR,
                    ValorBase = produto.NUM_VALOR,
                    ValorPercentagemAtacado = produto.NUM_VALOR_ATACADO,
                    ValorPercentagemVarejo = produto.NUM_VALOR_VAREJO,
                    Quantidade = produto.NUM_QUANTIDADE,
                    Ncm = produto.NCM_PRODUTO,
                    Fornecedor = Negocio.Fornecedor.BuscarFornecedor(produto.T_FORNECEDOR),                    
                    Ativo = produto.BOL_ATIVO,                    
                    Taxas = Negocio.Taxa.ListarProdutoTaxa(produto.T_PRODUTO_TAXA),
                    UnidadeMedidas = Negocio.UnidadeMedida.ListarProdutoUnidadeMedida(produto.T_PRODUTO_UNIDADE_MEDIDA)
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
        internal static Contrato.RetornoProduto ListarProduto(Contrato.EntradaProduto entradaProduto)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoProduto retProduto = new Contrato.RetornoProduto();
            
            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaProduto.UsuarioLogado, Chave = entradaProduto.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Verifica se a empresa não foi informada
                if (string.IsNullOrWhiteSpace(entradaProduto.EmpresaLogada.Id.ToString()))
                {
                    entradaProduto.EmpresaLogada.Id = Guid.Empty;
                }

                // Verifica se o código não foi informado
                if (string.IsNullOrWhiteSpace(entradaProduto.Produto.Codigo))
                {
                    entradaProduto.Produto.Codigo = string.Empty;
                }

                // Verifica se o código não foi informado
                if (string.IsNullOrWhiteSpace(entradaProduto.Produto.CodigoFornecedor))
                {
                    entradaProduto.Produto.CodigoFornecedor = string.Empty;
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
                context.ContextOptions.LazyLoadingEnabled = true;

                if (entradaProduto.Paginar)
                {
                    // Busca o produto no banco
                    var lstProdutos = (from p in context.T_PRODUTO
                                       where
                                        (p.BOL_ATIVO == entradaProduto.Produto.Ativo)
                                        && (entradaProduto.EmpresaLogada.Id == Guid.Empty || p.T_FORNECEDOR.ID_EMPRESA == entradaProduto.EmpresaLogada.Id)
                                        && (entradaProduto.Produto.Codigo == string.Empty || p.COD_PRODUTO.Contains(entradaProduto.Produto.Codigo))
                                        && (entradaProduto.Produto.Nome == string.Empty || p.NOME_PRODUTO.Contains(entradaProduto.Produto.Nome))
                                        && (entradaProduto.Produto.CodigoFornecedor == string.Empty || p.COD_PRODUTO_FORNECEDOR.Contains(entradaProduto.Produto.CodigoFornecedor))
                                        && (entradaProduto.Produto.Fornecedor.Id == Guid.Empty || p.ID_FORNECEDOR == entradaProduto.Produto.Fornecedor.Id)
                                       select p
                                        ).OrderBy(o => o.NOME_PRODUTO).ThenBy(o => o.COD_PRODUTO).ThenBy(o => o.T_FORNECEDOR.NOME_FORNECEDOR).Skip(entradaProduto.PosicaoUltimoItem).Take(entradaProduto.CantidadeItens)
                                        .Select(p => new
                                        {
                                            p,
                                            t = p.T_PRODUTO_TAXA,
                                            um = p.T_PRODUTO_UNIDADE_MEDIDA
                                        }).ToList();

                    // Verifica se foi encontrado algum registro
                    if (lstProdutos.Count > 0)
                    {
                        // Preenche o objeto de retorno
                        retProduto.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                        retProduto.Produtos = new List<Contrato.Produto>();
                        foreach (var item in lstProdutos)
                        {
                            retProduto.Produtos.Add(new Contrato.Produto()
                            {
                                Id = item.p.ID_PRODUTO,
                                Nome = item.p.NOME_PRODUTO,
                                Codigo = item.p.COD_PRODUTO,
                                CodigoBarras = item.p.COD_BARRAS,
                                CodigoFornecedor = item.p.COD_PRODUTO_FORNECEDOR,
                                Quantidade = item.p.NUM_QUANTIDADE,
                                ValorBase = item.p.NUM_VALOR,
                                ValorPercentagemAtacado = item.p.NUM_VALOR_ATACADO,
                                ValorPercentagemVarejo = item.p.NUM_VALOR_VAREJO,
                                Ncm = item.p.NCM_PRODUTO,
                                Ativo = item.p.BOL_ATIVO,
                                Fornecedor = Negocio.Fornecedor.BuscarFornecedor(item.p.T_FORNECEDOR),
                                Taxas = Negocio.Taxa.ListarProdutoTaxa(item.t),
                                UnidadeMedidas = Negocio.UnidadeMedida.ListarProdutoUnidadeMedida(item.um)
                            });
                        }
                    }
                }
                else
                {
                    // Busca o produto no banco
                    var lstProdutos = (from p in context.T_PRODUTO
                                       where
                                        (p.BOL_ATIVO == entradaProduto.Produto.Ativo)
                                        && (entradaProduto.EmpresaLogada.Id == Guid.Empty || p.T_FORNECEDOR.ID_EMPRESA == entradaProduto.EmpresaLogada.Id)
                                        && (entradaProduto.Produto.Codigo == string.Empty || p.COD_PRODUTO.Contains(entradaProduto.Produto.Codigo))
                                        && (entradaProduto.Produto.CodigoFornecedor == string.Empty || p.COD_PRODUTO_FORNECEDOR.Contains(entradaProduto.Produto.CodigoFornecedor))
                                        && (entradaProduto.Produto.Nome == string.Empty || p.NOME_PRODUTO.Contains(entradaProduto.Produto.Nome))
                                        && (entradaProduto.Produto.Fornecedor.Id == Guid.Empty || p.ID_FORNECEDOR == entradaProduto.Produto.Fornecedor.Id)
                                       select new
                                       {
                                           p,
                                           t = p.T_PRODUTO_TAXA,
                                           um = p.T_PRODUTO_UNIDADE_MEDIDA
                                       }
                                        ).ToList();

                    // Verifica se foi encontrado algum registro
                    if (lstProdutos.Count > 0)
                    {
                        // Preenche o objeto de retorno
                        retProduto.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                        retProduto.Produtos = new List<Contrato.Produto>();
                        foreach (var item in lstProdutos)
                        {
                            retProduto.Produtos.Add(new Contrato.Produto()
                            {
                                Id = item.p.ID_PRODUTO,
                                Nome = item.p.NOME_PRODUTO,
                                Codigo = item.p.COD_PRODUTO,
                                CodigoBarras = item.p.COD_BARRAS,
                                CodigoFornecedor = item.p.COD_PRODUTO_FORNECEDOR,
                                Quantidade = item.p.NUM_QUANTIDADE,
                                ValorBase = item.p.NUM_VALOR,
                                ValorPercentagemAtacado = item.p.NUM_VALOR_ATACADO,
                                ValorPercentagemVarejo = item.p.NUM_VALOR_VAREJO,
                                Ncm = item.p.NCM_PRODUTO,
                                Ativo = item.p.BOL_ATIVO,
                                Fornecedor = Negocio.Fornecedor.BuscarFornecedor(item.p.T_FORNECEDOR),
                                Taxas = Negocio.Taxa.ListarProdutoTaxa(item.t),
                                UnidadeMedidas = Negocio.UnidadeMedida.ListarProdutoUnidadeMedida(item.um)
                            });
                        }
                    }
                }

                // Se não econtrou nenhum produto
                if (retProduto.Produtos == null || retProduto.Produtos.Count == 0)
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

            // define o tempo de duração
            retProduto.Duracao = DateTime.Now.Ticks - retProduto.Duracao;
            
            // retorna os dados
            return retProduto;
        }
 
        /// <summary>
        /// Método para listar os produtos para relatórios
        /// </summary>
        /// <param name="Produto">Objeto com os dados do filtro</param>
        /// <returns>Contrato.RetornoProduto</returns>
        internal static Contrato.RetornoProduto ListarProdutoRelatorio(Contrato.EntradaProduto entradaProduto)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoProduto retProduto = new Contrato.RetornoProduto();

            // define o tempo de duração
            retProduto.Duracao = DateTime.Now.Ticks;

            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaProduto.UsuarioLogado, Chave = entradaProduto.Chave });

            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Verifica se a empresa não foi informada
                if (string.IsNullOrWhiteSpace(entradaProduto.EmpresaLogada.Id.ToString()))
                {
                    entradaProduto.EmpresaLogada.Id = Guid.Empty;
                }

                // Verifica se o código não foi informado
                if (string.IsNullOrWhiteSpace(entradaProduto.Produto.Codigo))
                {
                    entradaProduto.Produto.Codigo = string.Empty;
                }

                // Verifica se o código não foi informado
                if (string.IsNullOrWhiteSpace(entradaProduto.Produto.CodigoFornecedor))
                {
                    entradaProduto.Produto.CodigoFornecedor = string.Empty;
                }

                // Verifica se o nome não foi informado
                if (string.IsNullOrWhiteSpace(entradaProduto.Produto.Nome))
                {
                    entradaProduto.Produto.Nome = string.Empty;
                }

                List<Guid> IdsFornecedores = new List<Guid>();
                // Verifica se existe fornecedores
                if (entradaProduto.Produto.Fornecedores != null && entradaProduto.Produto.Fornecedores.Count > 0)
                {
                    IdsFornecedores = entradaProduto.Produto.Fornecedores.Select(f => f.Id).ToList();
                }
                
                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();
                context.ContextOptions.LazyLoadingEnabled = false;

                // Busca o produto no banco
                var lstProdutos = (from p in context.T_PRODUTO
                                   where
                                   (p.BOL_ATIVO == entradaProduto.Produto.Ativo)
                                   && (entradaProduto.EmpresaLogada.Id == Guid.Empty || p.T_FORNECEDOR.ID_EMPRESA == entradaProduto.EmpresaLogada.Id)
                                   && (entradaProduto.Produto.Codigo == string.Empty || p.COD_PRODUTO.Contains(entradaProduto.Produto.Codigo))
                                   && (entradaProduto.Produto.Nome == string.Empty || p.NOME_PRODUTO.Contains(entradaProduto.Produto.Nome))
                                   && (entradaProduto.Produto.CodigoFornecedor == string.Empty || p.COD_PRODUTO_FORNECEDOR.Contains(entradaProduto.Produto.CodigoFornecedor))
                                   && (IdsFornecedores.Count == 0 || IdsFornecedores.Contains(p.ID_FORNECEDOR))
                                   select new Contrato.Produto
                                    {
                                        Id = p.ID_PRODUTO,
                                        Nome = p.NOME_PRODUTO,
                                        Codigo = p.COD_PRODUTO,
                                        CodigoBarras = p.COD_BARRAS,
                                        CodigoFornecedor = p.COD_PRODUTO_FORNECEDOR,
                                        Quantidade = p.NUM_QUANTIDADE,
                                        ValorBase = p.NUM_VALOR,
                                        ValorPercentagemAtacado = p.NUM_VALOR_ATACADO,
                                        ValorPercentagemVarejo = p.NUM_VALOR_VAREJO,
                                        Ncm = p.NCM_PRODUTO,
                                        Ativo = p.BOL_ATIVO,
                                        Fornecedor = new Contrato.Fornecedor
                                        {
                                            Id = p.T_FORNECEDOR.ID_FORNECEDOR,
                                            Nome = p.T_FORNECEDOR.NOME_FORNECEDOR,
                                            Codigo = p.T_FORNECEDOR.COD_FORNECEDOR,
                                            Cpf_Cnpj = p.T_FORNECEDOR.CPF_CNJP_FORNECEDOR,
                                            ValorPercentagemAtacado = p.T_FORNECEDOR.NUM_VALOR_ATACADO,
                                            ValorPercentagemVarejo = p.T_FORNECEDOR.NUM_VALOR_VAREJO,
                                            Ativo = p.T_FORNECEDOR.BOL_ATIVO,
                                            Tipo = p.T_FORNECEDOR.BOL_PESSOA_FISICA ? Contrato.Enumeradores.Pessoa.Fisica : Contrato.Enumeradores.Pessoa.Juridica,
                                        },
                                        eTaxas = (from pt in p.T_PRODUTO_TAXA join t in context.T_TAXA on pt.ID_TAXA equals t.ID_TAXA select new Contrato.Taxa
                                        {
                                            Id = t.ID_TAXA,
                                            Nome = t.NOME_TAXA,
                                            Valor = pt.NUM_VALOR,
                                            Desconto = t.BOL_DESCONTO,
                                            Prioridade = pt.ORD_PRIORIDADE,
                                            Ativo = t.BOL_ATIVO

                                        }).AsEnumerable(),
                                   }
                                ).ToList();

                if (lstProdutos.Count > 0)
                {
                    retProduto.Produtos = lstProdutos.ToList(); 
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

            // define o tempo de duração
            retProduto.Duracao = DateTime.Now.Ticks - retProduto.Duracao;

            // retorna os dados
            return retProduto;
        }

        /// <summary>
        /// Método para listar os produtos para relatórios
        /// </summary>
        /// <param name="Produto">Objeto com os dados do filtro</param>
        /// <returns>Contrato.RetornoProduto</returns>
        internal static Contrato.RetornoProduto ListarProdutoRelatorio2(Contrato.EntradaProduto entradaProduto)
        {            
            // Objeto que recebe o retorno do método
            Contrato.RetornoProduto retProduto = new Contrato.RetornoProduto();

            // define o tempo de duração
            retProduto.Duracao = DateTime.Now.Ticks;

            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaProduto.UsuarioLogado, Chave = entradaProduto.Chave });

            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Verifica se a empresa não foi informada
                if (string.IsNullOrWhiteSpace(entradaProduto.EmpresaLogada.Id.ToString()))
                {
                    entradaProduto.EmpresaLogada.Id = Guid.Empty;
                }

                // Verifica se o código não foi informado
                if (string.IsNullOrWhiteSpace(entradaProduto.Produto.Codigo))
                {
                    entradaProduto.Produto.Codigo = string.Empty;
                }

                // Verifica se o código não foi informado
                if (string.IsNullOrWhiteSpace(entradaProduto.Produto.CodigoFornecedor))
                {
                    entradaProduto.Produto.CodigoFornecedor = string.Empty;
                }

                // Verifica se o nome não foi informado
                if (string.IsNullOrWhiteSpace(entradaProduto.Produto.Nome))
                {
                    entradaProduto.Produto.Nome = string.Empty;
                }

                List<Guid> IdsFornecedores = new List<Guid>();
                // Verifica se existe fornecedores
                if (entradaProduto.Produto.Fornecedores != null && entradaProduto.Produto.Fornecedores.Count > 0)
                {
                    IdsFornecedores = entradaProduto.Produto.Fornecedores.Select(f => f.Id).ToList();
                }

                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();
                context.ContextOptions.LazyLoadingEnabled = false;

                // Busca o produto no banco
                var lstProdutos = (from p in context.T_PRODUTO
                                   where
                                   (p.BOL_ATIVO == entradaProduto.Produto.Ativo)
                                   && (entradaProduto.EmpresaLogada.Id == Guid.Empty || p.T_FORNECEDOR.ID_EMPRESA == entradaProduto.EmpresaLogada.Id)
                                   && (entradaProduto.Produto.Codigo == string.Empty || p.COD_PRODUTO.Contains(entradaProduto.Produto.Codigo))
                                   && (entradaProduto.Produto.Nome == string.Empty || p.NOME_PRODUTO.Contains(entradaProduto.Produto.Nome))
                                   && (entradaProduto.Produto.CodigoFornecedor == string.Empty || p.COD_PRODUTO_FORNECEDOR.Contains(entradaProduto.Produto.CodigoFornecedor))
                                   && (IdsFornecedores.Count == 0 || IdsFornecedores.Contains(p.ID_FORNECEDOR))
                                   select  new
                                   {
                                       p,
                                       f = p.T_FORNECEDOR,
                                       tp = p.T_PRODUTO_TAXA,
                                       t = (from pt in p.T_PRODUTO_TAXA join t in context.T_TAXA on pt.ID_TAXA equals t.ID_TAXA select t),
                                   }
                                ).ToList();

                // Verifica se foi encontrado algum registro
                if (lstProdutos.Count > 0)
                {
                    // Preenche o objeto de retorno
                    retProduto.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    retProduto.Produtos = new List<Contrato.Produto>();
                    foreach (var item in lstProdutos)
                    {
                        retProduto.Produtos.Add(new Contrato.Produto()
                        {
                            Id = item.p.ID_PRODUTO,
                            Nome = item.p.NOME_PRODUTO,
                            Codigo = item.p.COD_PRODUTO,
                            CodigoFornecedor = item.p.COD_PRODUTO_FORNECEDOR,
                            Quantidade = item.p.NUM_QUANTIDADE,
                            ValorBase = item.p.NUM_VALOR,
                            Ncm = item.p.NCM_PRODUTO,
                            Ativo = item.p.BOL_ATIVO,
                            Fornecedor = Negocio.Fornecedor.BuscarFornecedor(item.p.T_FORNECEDOR),
                            Taxas = Negocio.Taxa.ListarProdutoTaxa(item.tp, item.t)
                        });
                    }
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

            // define o tempo de duração
            retProduto.Duracao = DateTime.Now.Ticks - retProduto.Duracao;

            // retorna os dados
            return retProduto;
        }

        /// <summary>
        /// Método para salvar o produto
        /// </summary>
        /// <param name="entradaProduto">Objeto com os dados do produto</param>
        /// <returns>Contrato.RetornoProduto</returns>
        internal static Contrato.RetornoProduto SalvarProduto(Contrato.EntradaProduto entradaProduto)
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
                    context.ContextOptions.LazyLoadingEnabled = true;

                    // Busca o produto no banco
                    List<Dados.PRODUTO> lstProdutos = (from p in context.T_PRODUTO
                                                       where (p.COD_PRODUTO == entradaProduto.Produto.Codigo
                                                          || (entradaProduto.Novo == null && entradaProduto.Produto.Id == p.ID_PRODUTO) 
                                                          || (entradaProduto.Novo.Value == true && p.COD_PRODUTO_FORNECEDOR == entradaProduto.Produto.CodigoFornecedor && p.COD_PRODUTO_FORNECEDOR != string.Empty && p.ID_FORNECEDOR == entradaProduto.Produto.Fornecedor.Id))
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
                            lstProdutos.First().COD_BARRAS = entradaProduto.Produto.CodigoBarras;
                            lstProdutos.First().NUM_QUANTIDADE = entradaProduto.Produto.Quantidade;
                            lstProdutos.First().NUM_VALOR = entradaProduto.Produto.ValorBase;
                            lstProdutos.First().NUM_VALOR_ATACADO = entradaProduto.Produto.ValorPercentagemAtacado;
                            lstProdutos.First().NUM_VALOR_VAREJO = entradaProduto.Produto.ValorPercentagemVarejo;
                            lstProdutos.First().COD_PRODUTO_FORNECEDOR = entradaProduto.Produto.CodigoFornecedor;
                            lstProdutos.First().ID_FORNECEDOR = entradaProduto.Produto.Fornecedor.Id;
                            lstProdutos.First().NCM_PRODUTO = entradaProduto.Produto.Ncm;
                            lstProdutos.First().BOL_ATIVO = entradaProduto.Produto.Ativo;
                            lstProdutos.First().DATA_ATUALIZACAO = DateTime.Now;
                            lstProdutos.First().LOGIN_USUARIO = entradaProduto.UsuarioLogado;

                            // Apaga todas as taxas que estão relacionados
                            while (lstProdutos.First().T_PRODUTO_TAXA.Count > 0)
                            {
                                context.T_PRODUTO_TAXA.DeleteObject(lstProdutos.First().T_PRODUTO_TAXA.First());
                            }

                            // Verifica se existe alguma taxa associada ao produto
                            if (entradaProduto.Produto.Taxas != null)
                            {
                                // Para cada taxa associada
                                foreach (Contrato.Taxa taxa in entradaProduto.Produto.Taxas)
                                {
                                    Negocio.Taxa.SalvarTaxaProduto(lstProdutos.First(), entradaProduto.UsuarioLogado, taxa);
                                }
                            }

                            // Apaga todas as unidades de medidas que estão relacionados
                            while (lstProdutos.First().T_PRODUTO_UNIDADE_MEDIDA.Count > 0)
                            {
                                context.T_PRODUTO_UNIDADE_MEDIDA.DeleteObject(lstProdutos.First().T_PRODUTO_UNIDADE_MEDIDA.First());
                            }

                            // Verifica se existe alguma unidade de medida associado ao produto
                            if (entradaProduto.Produto.UnidadeMedidas != null)
                            {
                                // Para cada taxa associada
                                foreach (Contrato.UnidadeMedida unidadeMedida in entradaProduto.Produto.UnidadeMedidas)
                                {
                                    Negocio.UnidadeMedida.SalvarUnidadeMedidaProduto(lstProdutos.First(), entradaProduto.UsuarioLogado, unidadeMedida);
                                }
                            }
                        }
                        else
                        {
                            // Recupera o código do produto
                            string codigoProduto = string.Empty;
                            if (entradaProduto.Produto.Codigo != string.Empty)
                                codigoProduto = entradaProduto.Produto.Codigo;
                            else
                            {
                                System.Data.Objects.ObjectParameter objCodigoProduto = new System.Data.Objects.ObjectParameter("P_CODIGO", typeof(global::System.Int32));
                                context.RETORNAR_CODIGO(Contrato.Constantes.TIPO_COD_PRODUTO, entradaProduto.EmpresaLogada.Id, objCodigoProduto);
                                codigoProduto = Util.RecuperaCodigo((int)objCodigoProduto.Value, Contrato.Constantes.TIPO_COD_PRODUTO);
                            }

                            // Cria o produto
                            Dados.PRODUTO tProduto = new Dados.PRODUTO();
                            tProduto.ID_PRODUTO = Guid.NewGuid();
                            tProduto.COD_PRODUTO = codigoProduto;
                            tProduto.COD_BARRAS = entradaProduto.Produto.CodigoBarras;
                            tProduto.NOME_PRODUTO = entradaProduto.Produto.Nome;
                            tProduto.COD_PRODUTO_FORNECEDOR = entradaProduto.Produto.CodigoFornecedor;
                            tProduto.ID_FORNECEDOR = entradaProduto.Produto.Fornecedor.Id;
                            tProduto.NCM_PRODUTO = entradaProduto.Produto.Ncm;
                            tProduto.NUM_VALOR = entradaProduto.Produto.ValorBase;
                            tProduto.NUM_VALOR_ATACADO = entradaProduto.Produto.ValorPercentagemAtacado;
                            tProduto.NUM_VALOR_VAREJO = entradaProduto.Produto.ValorPercentagemVarejo;
                            tProduto.NUM_QUANTIDADE = entradaProduto.Produto.Quantidade;
                            tProduto.BOL_ATIVO = entradaProduto.Produto.Ativo;
                            tProduto.DATA_ATUALIZACAO = DateTime.Now;
                            tProduto.LOGIN_USUARIO = entradaProduto.UsuarioLogado;

                            // Verifica se existe alguma taxa associada ao produto
                            if (entradaProduto.Produto.Taxas != null)
                            {
                                // Para cada taxa associada
                                foreach (Contrato.Taxa taxa in entradaProduto.Produto.Taxas)
                                {
                                    Negocio.Taxa.SalvarTaxaProduto(tProduto, entradaProduto.UsuarioLogado, taxa);
                                }
                            }

                            // Verifica se existe alguma unidade de medida associado ao produto
                            if (entradaProduto.Produto.UnidadeMedidas != null)
                            {
                                // Para cada taxa associada
                                foreach (Contrato.UnidadeMedida unidadeMedida in entradaProduto.Produto.UnidadeMedidas)
                                {
                                    Negocio.UnidadeMedida.SalvarUnidadeMedidaProduto(lstProdutos.First(), entradaProduto.UsuarioLogado, unidadeMedida);
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
        internal static Contrato.RetornoProduto SalvarProdutos(Contrato.EntradaProdutos entradaProdutos)
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
                    if (entradaProdutos.Fornecedor != null && entradaProdutos.Fornecedor.Id == Guid.Empty)
                        // Busca os dados do fornecedor
                        entradaProdutos.Fornecedor = Fornecedor.BuscarFornecedor((from f in context.T_FORNECEDOR where f.COD_FORNECEDOR == entradaProdutos.Fornecedor.Codigo select f).FirstOrDefault());

                    // Para cada produto existente na lista
                    foreach (Contrato.Produto produto in entradaProdutos.Produtos)
                    {
                        // Se encontrou o fornecedor
                        if (entradaProdutos.Fornecedor != null)
                            // Define o fornecedor do produto
                            produto.Fornecedor = entradaProdutos.Fornecedor;

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
                            context.ContextOptions.LazyLoadingEnabled = true;
                            // Busca o produto no banco
                            List<Dados.PRODUTO> lstProdutos = (from p in context.T_PRODUTO
                                                                where                                                                    
                                                                     p.ID_PRODUTO == produto.Id && produto.Id != Guid.Empty  &&
                                                                     p.ID_FORNECEDOR == produto.Fornecedor.Id
                                                                select p).ToList();

                            // Se existe o produto
                            if (lstProdutos.Count > 0)
                            {
                                // Atualiza o produto
                                lstProdutos.First().COD_BARRAS = produto.CodigoBarras;
                                lstProdutos.First().NUM_QUANTIDADE = produto.Quantidade;
                                lstProdutos.First().NUM_VALOR = produto.ValorBase;
                                lstProdutos.First().NUM_VALOR_ATACADO = produto.ValorPercentagemAtacado;
                                lstProdutos.First().NUM_VALOR_VAREJO = produto.ValorPercentagemVarejo;
                                lstProdutos.First().ID_FORNECEDOR = produto.Fornecedor.Id;
                                lstProdutos.First().NCM_PRODUTO = produto.Ncm;
                                lstProdutos.First().BOL_ATIVO = (bool)produto.Fornecedor.Ativo;
                                lstProdutos.First().DATA_ATUALIZACAO = DateTime.Now;
                                lstProdutos.First().LOGIN_USUARIO = entradaProdutos.UsuarioLogado;

                                // Apaga todas as taxas que estão relacionados
                                while (lstProdutos.First().T_PRODUTO_TAXA.Count > 0)
                                {
                                    context.T_PRODUTO_TAXA.DeleteObject(lstProdutos.First().T_PRODUTO_TAXA.First());
                                }

                                // Se existe o forneedor
                                if (entradaProdutos.Fornecedor != null)
                                {
                                    // Verifica se existe alguma taxa associada ao fornecedor
                                    if (produto.Fornecedor.Taxas != null)
                                    {
                                        // Para cada taxa associada
                                        foreach (Contrato.Taxa taxa in produto.Fornecedor.Taxas)
                                        {
                                            Negocio.Taxa.SalvarTaxaProduto(lstProdutos.First(), entradaProdutos.UsuarioLogado, taxa);
                                        }
                                    }
                                }

                                // Verifica se existe alguma taxa associada ao produto
                                if (produto.Taxas != null)
                                {
                                    // Para cada taxa associada
                                    foreach (Contrato.Taxa taxa in produto.Taxas)
                                    {
                                        Negocio.Taxa.SalvarTaxaProduto(lstProdutos.First(), entradaProdutos.UsuarioLogado, taxa);
                                    }
                                }
                            }
                            else
                            {
                                // Cria o produto
                                Dados.PRODUTO tProduto = new Dados.PRODUTO();
                                tProduto.ID_PRODUTO = Guid.NewGuid();
                                tProduto.COD_PRODUTO = BuscarCodigoProduto(entradaProdutos.EmpresaLogada.Id);
                                tProduto.COD_BARRAS = produto.CodigoBarras;
                                tProduto.NOME_PRODUTO = produto.Nome;
                                tProduto.COD_PRODUTO_FORNECEDOR = produto.CodigoFornecedor;
                                tProduto.ID_FORNECEDOR = produto.Fornecedor.Id;
                                tProduto.NUM_QUANTIDADE = produto.Quantidade;
                                tProduto.NUM_VALOR = produto.ValorBase;
                                tProduto.NUM_VALOR_ATACADO = produto.ValorPercentagemAtacado;
                                tProduto.NUM_VALOR_VAREJO = produto.ValorPercentagemVarejo;
                                tProduto.NCM_PRODUTO = produto.Ncm;
                                tProduto.BOL_ATIVO = produto.Ativo;
                                tProduto.DATA_ATUALIZACAO = DateTime.Now;
                                tProduto.LOGIN_USUARIO = entradaProdutos.UsuarioLogado;

                                // Verifica se existe alguma taxa associada ao produto
                                if (produto.Fornecedor.Taxas != null)
                                {
                                    // Para cada taxa associada
                                    foreach (Contrato.Taxa taxa in produto.Fornecedor.Taxas)
                                    {
                                        Negocio.Taxa.SalvarTaxaProduto(tProduto, entradaProdutos.UsuarioLogado, taxa);
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

            // Verifica se a Nome foi preenchida
            if (string.IsNullOrWhiteSpace(Produto.Nome))
                strRetorno += "O campo 'Nome' não foi informado!\n";

            // Verifica se a Nome foi preenchida
            if (Produto.Fornecedor == null)
                strRetorno += "O campo 'Fornecedor' não foi informado!\n";

            // retorna a variável de retorno
            return strRetorno;
        }       
    }
}