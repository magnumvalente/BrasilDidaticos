using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrasilDidaticos.WcfServico;

namespace BrasilDidaticos.WcfServico.Negocio
{
    static class Fornecedor
    {
        /// <summary>
        /// Método para buscar o código do fornecedor
        /// </summary>        
        /// <returns>string</returns>
        internal static string BuscarCodigoFornecedor()
        {
            // Objeto que recebe o retorno do método
            string retCodigoFornecedor = string.Empty;

            // Loga no banco de dados
            Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();
            System.Data.Objects.ObjectParameter objCodigoFornecedor = new System.Data.Objects.ObjectParameter("P_CODIGO", typeof(global::System.Int32));
            context.RETORNAR_CODIGO(Contrato.Constantes.TIPO_COD_FORNECEDOR, objCodigoFornecedor);

            // Recupera o código do fornecedor
            retCodigoFornecedor = Util.RecuperaCodigo((int)objCodigoFornecedor.Value, Contrato.Constantes.TIPO_COD_FORNECEDOR);

            // retorna os dados
            return retCodigoFornecedor;
        }

        /// <summary>
        /// Método para buscar o fornecedor
        /// </summary>
        /// <param name="Fornecedor">Objeto com o identificador do fornecedor</param>
        /// <returns>Contrato.RetornoFornecedor</returns>
        internal static Contrato.Fornecedor BuscarFornecedor(Dados.FORNECEDOR fornecedor)
        {
            // Objeto que recebe o retorno do método
            Contrato.Fornecedor retFornecedor = new Contrato.Fornecedor();
            
            // Verifica se foi encontrado algum registro
            if (fornecedor != null)
            {
                retFornecedor = new Contrato.Fornecedor()
                {
                    Id = fornecedor.ID_FORNECEDOR,
                    Nome = fornecedor.NOME_FORNECEDOR,
                    Codigo = fornecedor.COD_FORNECEDOR,
                    Cpf_Cnpj = fornecedor.CPF_CNJP_FORNECEDOR,
                    ValorPercentagemAtacado = fornecedor.NUM_VALOR_ATACADO,
                    ValorPercentagemVarejo = fornecedor.NUM_VALOR_VAREJO,
                    Ativo = fornecedor.BOL_ATIVO,
                    Tipo = fornecedor.BOL_PESSOA_FISICA ? Contrato.Enumeradores.Pessoa.Fisica : Contrato.Enumeradores.Pessoa.Juridica,
                    Taxas = Negocio.Taxa.ListarFornecedorTaxa(fornecedor.T_FORNECEDOR_TAXA)
                };
            }

            // retorna os dados
            return retFornecedor;
        }

        /// <summary>
        /// Método para listar os fornecedores
        /// </summary>
        /// <param name="Fornecedor">Objeto com os dados do filtro</param>
        /// <returns>Contrato.RetornoFornecedor</returns>
        internal static Contrato.RetornoFornecedor ListarFornecedor(Contrato.EntradaFornecedor entradaFornecedor)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoFornecedor retFornecedor = new Contrato.RetornoFornecedor();
            
            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaFornecedor.UsuarioLogado, Chave = entradaFornecedor.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();
                                                
                // Busca o fornecedor no banco
                List<Dados.FORNECEDOR> lstFornecedores = (from f in context.T_FORNECEDOR
                                                          where
                                                                (entradaFornecedor.Fornecedor.Ativo == null || f.BOL_ATIVO == entradaFornecedor.Fornecedor.Ativo)
                                                             && (entradaFornecedor.Fornecedor.Codigo == null || entradaFornecedor.Fornecedor.Codigo == string.Empty || f.COD_FORNECEDOR.StartsWith(entradaFornecedor.Fornecedor.Codigo))
                                                             && (entradaFornecedor.Fornecedor.Nome == null || entradaFornecedor.Fornecedor.Nome == string.Empty || f.NOME_FORNECEDOR.ToLower().Contains(entradaFornecedor.Fornecedor.Nome.ToLower()))
                                                             && (entradaFornecedor.Fornecedor.Tipo == null || f.BOL_PESSOA_FISICA == (entradaFornecedor.Fornecedor.Tipo == Contrato.Enumeradores.Pessoa.Fisica ? true : false))
                                                             && (entradaFornecedor.Fornecedor.Cpf_Cnpj == null || entradaFornecedor.Fornecedor.Cpf_Cnpj == string.Empty || f.CPF_CNJP_FORNECEDOR != null && f.CPF_CNJP_FORNECEDOR.StartsWith(entradaFornecedor.Fornecedor.Cpf_Cnpj))
                                                          select f).ToList();
                               
                // Verifica se foi encontrado algum registro
                if (lstFornecedores.Count > 0)
                {
                    // Preenche o objeto de retorno
                    retFornecedor.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    retFornecedor.Fornecedores = new List<Contrato.Fornecedor>();
                    foreach (Dados.FORNECEDOR fornecedor in lstFornecedores)
                    {          
                        retFornecedor.Fornecedores.Add( new Contrato.Fornecedor()
                        {
                            Id = fornecedor.ID_FORNECEDOR,
                            Nome = fornecedor.NOME_FORNECEDOR,
                            Codigo = fornecedor.COD_FORNECEDOR,
                            Cpf_Cnpj = fornecedor.CPF_CNJP_FORNECEDOR,
                            ValorPercentagemAtacado = fornecedor.NUM_VALOR_ATACADO,
                            ValorPercentagemVarejo = fornecedor.NUM_VALOR_VAREJO,
                            Ativo = fornecedor.BOL_ATIVO,
                            Tipo = fornecedor.BOL_PESSOA_FISICA?Contrato.Enumeradores.Pessoa.Fisica:Contrato.Enumeradores.Pessoa.Juridica,
                            Taxas = Negocio.Taxa.ListarFornecedorTaxa(fornecedor.T_FORNECEDOR_TAXA)
                        });
                    };
                }
                else
                {
                    // Preenche o objeto de retorno
                    retFornecedor.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;
                    retFornecedor.Mensagem = "Não existe dados para o filtro informado.";
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retFornecedor.Codigo = retSessao.Codigo;
                retFornecedor.Mensagem = retSessao.Mensagem;
            }
            
            // retorna os dados
            return retFornecedor;
        }

        /// <summary>
        /// Método para salvar o fornecedor
        /// </summary>
        /// <param name="entradaFornecedor">Objeto com os dados do fornecedor</param>
        /// <returns>Contrato.RetornoFornecedor</returns>
        internal static Contrato.RetornoFornecedor SalvarFornecedor(Contrato.EntradaFornecedor entradaFornecedor)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoFornecedor retFornecedor = new Contrato.RetornoFornecedor();

            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaFornecedor.UsuarioLogado, Chave = entradaFornecedor.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Verifica se as informações do fornecedor foram informadas
                string strValidacao = ValidarFornecedorPreenchido(entradaFornecedor.Fornecedor);
            
                // Se existe algum erro
                if (strValidacao.Length > 0)
                {
                    retFornecedor.Codigo = Contrato.Constantes.COD_FILTRO_VAZIO;
                    retFornecedor.Mensagem = strValidacao;
                }
                else
                {
                    // Loga no banco de dados
                    Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                    // Busca o fornecedor no banco
                    List<Dados.FORNECEDOR> lstFornecedores = (from f in context.T_FORNECEDOR
                                                              where (f.COD_FORNECEDOR == entradaFornecedor.Fornecedor.Codigo &&
                                                                    (entradaFornecedor.Fornecedor.Cpf_Cnpj != null || f.CPF_CNJP_FORNECEDOR == entradaFornecedor.Fornecedor.Cpf_Cnpj))
                                                                 || (entradaFornecedor.Novo == null && entradaFornecedor.Fornecedor.Id == f.ID_FORNECEDOR)                                                              
                                                              select f).ToList();

                    // Verifica se foi encontrado algum registro
                    if (lstFornecedores.Count > 0 && entradaFornecedor.Novo != null && (bool)entradaFornecedor.Novo)
                    {
                        // Preenche o objeto de retorno
                        retFornecedor.Codigo = Contrato.Constantes.COD_REGISTRO_DUPLICADO;
                        retFornecedor.Mensagem = string.Format("O fornecedor de código '{0}' já existe!", lstFornecedores.First().COD_FORNECEDOR);
                    }
                    else
                    {
                        // Se existe o fornecedor
                        if (lstFornecedores.Count > 0)
                        {
                            bool atualizarProdutos = false;

                            // Atualiza o fornecedor
                            lstFornecedores.First().NOME_FORNECEDOR = entradaFornecedor.Fornecedor.Nome;
                            lstFornecedores.First().BOL_PESSOA_FISICA = entradaFornecedor.Fornecedor.Tipo == Contrato.Enumeradores.Pessoa.Fisica ? true : false;
                            lstFornecedores.First().CPF_CNJP_FORNECEDOR = entradaFornecedor.Fornecedor.Cpf_Cnpj;
                            lstFornecedores.First().NUM_VALOR_ATACADO = entradaFornecedor.Fornecedor.ValorPercentagemAtacado;
                            lstFornecedores.First().NUM_VALOR_VAREJO = entradaFornecedor.Fornecedor.ValorPercentagemVarejo;
                            lstFornecedores.First().BOL_ATIVO = (bool)entradaFornecedor.Fornecedor.Ativo;
                            lstFornecedores.First().DATA_ATUALIZACAO = DateTime.Now;
                            lstFornecedores.First().LOGIN_USUARIO = entradaFornecedor.UsuarioLogado;

                            // verifica se o fornecedor foi desativado
                            atualizarProdutos = (bool)entradaFornecedor.Fornecedor.Ativo == false;

                            // Verifica se é para atualizar os produtos
                            if (!atualizarProdutos)
                                // verifica se a quantidade de taxas foi alterada
                                atualizarProdutos = entradaFornecedor.Fornecedor.Taxas != null && lstFornecedores.First().T_FORNECEDOR_TAXA.Count != entradaFornecedor.Fornecedor.Taxas.Count;

                            // Verifica se é para atualizar os produtos
                            if (entradaFornecedor.Fornecedor.Taxas != null && !atualizarProdutos)
                                foreach (Contrato.Taxa t in entradaFornecedor.Fornecedor.Taxas)
                                {
                                    // Verifica se é para atualizar os produtos
                                    if (!atualizarProdutos)
                                        atualizarProdutos = (from ft in lstFornecedores.First().T_FORNECEDOR_TAXA where ft.ID_TAXA == t.Id && (ft.NUM_VALOR != t.Valor || ft.ORD_PRIORIDADE != t.Prioridade) select ft).Count() > 0;
                                    else
                                        break;
                                }

                            // Apaga todas as taxas que estão relacionadas ao fornecedor
                            while (lstFornecedores.First().T_FORNECEDOR_TAXA.Count > 0)
                            {
                                context.T_FORNECEDOR_TAXA.DeleteObject(lstFornecedores.First().T_FORNECEDOR_TAXA.First());
                            }

                            // Preenche as taxas do fornecedor
                            PreencherTaxaFornecedor(entradaFornecedor, lstFornecedores.First());

                            // Verifica se é para atualizar os produtos
                            if (atualizarProdutos)
                                // Atualiza o valor das taxas nos produtos
                                SalvarProdutosFornecedor(entradaFornecedor);
                        }
                        else
                        {
                            // Recupera o código do cliente
                            string codigoFornecedor = string.Empty;
                            if (entradaFornecedor.Fornecedor.Codigo != string.Empty)
                                codigoFornecedor = entradaFornecedor.Fornecedor.Codigo;
                            else
                            {
                                System.Data.Objects.ObjectParameter objCodigoFornecedor = new System.Data.Objects.ObjectParameter("P_CODIGO", typeof(global::System.Int32));
                                context.RETORNAR_CODIGO(Contrato.Constantes.TIPO_COD_FORNECEDOR, objCodigoFornecedor);
                                codigoFornecedor = Util.RecuperaCodigo((int)objCodigoFornecedor.Value, Contrato.Constantes.TIPO_COD_FORNECEDOR);
                            }

                            // Cria o fornecedor
                            Dados.FORNECEDOR tFornecedor = new Dados.FORNECEDOR();
                            tFornecedor.ID_FORNECEDOR = Guid.NewGuid();
                            tFornecedor.COD_FORNECEDOR = codigoFornecedor;
                            tFornecedor.NOME_FORNECEDOR = entradaFornecedor.Fornecedor.Nome;
                            tFornecedor.BOL_PESSOA_FISICA = entradaFornecedor.Fornecedor.Tipo == Contrato.Enumeradores.Pessoa.Fisica ? true : false;
                            tFornecedor.CPF_CNJP_FORNECEDOR = entradaFornecedor.Fornecedor.Cpf_Cnpj;
                            tFornecedor.NUM_VALOR_ATACADO = entradaFornecedor.Fornecedor.ValorPercentagemAtacado;
                            tFornecedor.NUM_VALOR_VAREJO = entradaFornecedor.Fornecedor.ValorPercentagemVarejo;
                            tFornecedor.BOL_ATIVO = (bool)entradaFornecedor.Fornecedor.Ativo;                           
                            tFornecedor.DATA_ATUALIZACAO = DateTime.Now;
                            tFornecedor.LOGIN_USUARIO = entradaFornecedor.UsuarioLogado;

                            // Preenche as taxas do fornecedor
                            PreencherTaxaFornecedor(entradaFornecedor, tFornecedor);

                            // Adiciona o fornecedor na tabela
                            context.AddToT_FORNECEDOR(tFornecedor);
                        }

                        // Salva as alterações
                        context.SaveChanges();

                        // Preenche o objeto de retorno
                        retFornecedor.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    }
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retFornecedor.Codigo = retSessao.Codigo;
                retFornecedor.Mensagem = retSessao.Mensagem;
            }

            // retorna dos dados 
            return retFornecedor;
        }

        /// <summary>
        /// Preenche a taxa do fornecedor
        /// </summary>
        /// <param name="entradaFornecedor">Contrato.EntradaFornecedor</param>
        /// <param name="tFornecedor">Dados.FORNECEDOR </param>
        private static void PreencherTaxaFornecedor(Contrato.EntradaFornecedor entradaFornecedor, Dados.FORNECEDOR tFornecedor) 
        {
            // Verifica se existe alguma taxa associada ao fornecedor
            if (entradaFornecedor.Fornecedor.Taxas != null)
            {
                // Para cada perfil associado
                foreach (Contrato.Taxa taxa in entradaFornecedor.Fornecedor.Taxas)
                {
                    // Associa a taxa ao fornecedor
                    tFornecedor.T_FORNECEDOR_TAXA.Add(new Dados.FORNECEDOR_TAXA()
                    {
                        ID_FORNECEDOR_TAXA = Guid.NewGuid(),
                        ID_FORNECEDOR = entradaFornecedor.Fornecedor.Id,
                        ID_TAXA = taxa.Id,
                        NUM_VALOR = taxa.Valor,
                        ORD_PRIORIDADE = taxa.Prioridade,
                        LOGIN_USUARIO = entradaFornecedor.UsuarioLogado,
                        DATA_ATUALIZACAO = DateTime.Now
                    });
                }
            }    
        }

        /// <summary>
        /// Atualiza os produtos do fornecedor
        /// </summary>
        /// <param name="entradaFornecedor">Contrato.EntradaFornecedor </param>
        private static void SalvarProdutosFornecedor(Contrato.EntradaFornecedor entradaFornecedor)
        {
            // Define o filtro para recuperar os produtos
            Contrato.EntradaProduto entProduto = new Contrato.EntradaProduto()
            {
                Chave = entradaFornecedor.Chave,
                UsuarioLogado = entradaFornecedor.UsuarioLogado,
                Produto = new Contrato.Produto() { Fornecedor = entradaFornecedor.Fornecedor, Ativo = true }
            };

            // Recupera todos os produtos do fornecedor
            Contrato.RetornoProduto retProduto = Negocio.Produto.ListarProduto(entProduto);

            // Se existem produtos
            if (retProduto.Produtos != null)
            {
                // Define a entrada para salvar os produtos
                Contrato.EntradaProdutos entProdutos = new Contrato.EntradaProdutos()
                {
                    Chave = entradaFornecedor.Chave,
                    UsuarioLogado = entradaFornecedor.UsuarioLogado,
                    Produtos = retProduto.Produtos,
                    Fornecedor = entradaFornecedor.Fornecedor
                };

                // Salva os produtos
                Negocio.Produto.SalvarProdutos(entProdutos);
            }
        }

        /// <summary>
        /// Método para verificar se as informações do fornecedor foram preenchidas
        /// </summary>
        /// <param name="Usuario">Objeto com o dados do fornecedor</param>
        /// <returns></returns>
        private static string ValidarFornecedorPreenchido(Contrato.Fornecedor Fornecedor)
        {
            // Cria a variável de retorno
            string strRetorno = string.Empty;                        

            // Verifica se a Nome foi preenchida
            if (string.IsNullOrWhiteSpace(Fornecedor.Nome))
                strRetorno += "O campo 'Nome' não foi informado!\n";

            // retorna a variável de retorno
            return strRetorno;

        }       
    }
}
