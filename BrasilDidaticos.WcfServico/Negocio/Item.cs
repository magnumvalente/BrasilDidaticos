using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrasilDidaticos.WcfServico;

namespace BrasilDidaticos.WcfServico.Negocio
{
    static class Item
    {
        /// <summary>
        /// Método para listar os Itens
        /// </summary>
        /// <param name="Item">Objeto com os dados do filtro</param>
        /// <returns>Contrato.RetornoItem</returns>
        internal static Contrato.RetornoItem ListarItem(Contrato.EntradaItem entradaItem)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoItem retItem = new Contrato.RetornoItem();
            
            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaItem.UsuarioLogado, Chave = entradaItem.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Verifica se o fornecedor não foi informado
                if (entradaItem.Item.Orcamento == null)
                {
                    entradaItem.Item.Orcamento = new Contrato.Orcamento();
                }

                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();
                                                
                // Busca o produto no banco
                List<Dados.ITEM> lstItems = null;
                    
                if (entradaItem.Paginar)
                {
                    lstItems = (from i in context.T_ITEM
                                where 
                                (entradaItem.Item.Orcamento.Id == Guid.Empty || i.ID_ORCAMENTO == entradaItem.Item.Orcamento.Id)
                                select i                                                           
                                ).OrderBy(o => o.ID_PRODUTO).Skip(entradaItem.PosicaoUltimoItem).Take(entradaItem.CantidadeItens).ToList();
                }
                else
                {
                    lstItems = (from i in context.T_ITEM
                                where                                   
                                (entradaItem.Item.Orcamento.Id == Guid.Empty || i.ID_ORCAMENTO == entradaItem.Item.Orcamento.Id)
                                select i
                                ).ToList();
                }
                                
                // Verifica se foi encontrado algum registro
                if (lstItems.Count > 0)
                {
                    // Preenche o objeto de retorno
                    retItem.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    retItem.Itens = new List<Contrato.Item>();
                    foreach (Dados.ITEM item in lstItems)
                    {
                        retItem.Itens.Add(new Contrato.Item()
                        {
                            Id = item.ID_ITEM,
                            Descricao = item.DES_ITEM,                            
                            Quantidade = item.NUM_QUANTIDADE,
                            ValorCusto = item.NUM_VALOR_CUSTO,
                            ValorUnitario = item.NUM_VALOR_UNITARIO,
                            ValorDesconto = item.NUM_DESCONTO,
                            Produto = Negocio.Produto.BuscarProduto(item.T_PRODUTO),
                            UnidadeMedida = item.T_PRODUTO == null ? null : Negocio.UnidadeMedida.BuscarProdutoUnidadeMedida(item.T_PRODUTO.T_PRODUTO_UNIDADE_MEDIDA.Where(pum => pum.ID_UNIDADE_MEDIDA == item.ID_UNIDADE_MEDIDA).FirstOrDefault()),
                            Orcamento = Negocio.Orcamento.BuscarOrcamento(item.T_ORCAMENTO)
                        });
                    };
                }
                else
                {
                    // Preenche o objeto de retorno
                    retItem.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;
                    retItem.Mensagem = "Não existe dados para o filtro informado.";
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retItem.Codigo = retSessao.Codigo;
                retItem.Mensagem = retSessao.Mensagem;
            }
            
            // retorna os dados
            return retItem;
        }

        /// <summary>
        /// Retorna uma lista de itens 
        /// </summary>
        /// <param name="lstOrcamentoItem">Recebe os itens do orçamento recuperado do banco</param>
        /// <returns>List<Contrato.Item></returns>
        internal static List<Contrato.Item> ListarOrcamentoItem(System.Data.Objects.DataClasses.EntityCollection<Dados.ITEM> lstOrcamentoItem)
        {
            List<Contrato.Item> lstItem = null;

            if (lstOrcamentoItem != null)
            {
                lstItem = new List<Contrato.Item>();

                foreach (Dados.ITEM item in lstOrcamentoItem)
                {
                    lstItem.Add(new Contrato.Item
                    {
                        Id = item.ID_ITEM,
                        Descricao = item.DES_ITEM,
                        Quantidade = item.NUM_QUANTIDADE,
                        ValorCusto = item.NUM_VALOR_CUSTO,
                        ValorUnitario = item.NUM_VALOR_UNITARIO,
                        ValorDesconto = item.NUM_DESCONTO,
                        Produto = Negocio.Produto.BuscarProduto(item.T_PRODUTO),
                        UnidadeMedida = item.T_PRODUTO == null ? null : Negocio.UnidadeMedida.BuscarProdutoUnidadeMedida(item.T_PRODUTO.T_PRODUTO_UNIDADE_MEDIDA.Where(pum => pum.ID_UNIDADE_MEDIDA == item.ID_UNIDADE_MEDIDA).FirstOrDefault()),
                        Orcamento = Negocio.Orcamento.BuscarOrcamento(item.T_ORCAMENTO, false)
                    });
                }
            }

            return lstItem;
        }

        /// <summary>
        /// Método para salvar os item do orçamento
        /// </summary>
        /// <param name="Item">Objeto com os dados do item</param>
        /// <returns>Contrato.RetornoTaxa</returns>
        internal static Contrato.RetornoItem SalvarItemOrcamento(Dados.ORCAMENTO Orcamento, string UsuarioLogado, Contrato.Item Item)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoItem retItem = new Contrato.RetornoItem();

            // Loga no banco de dados
            Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

            // Cria o item
            Dados.ITEM tItem = new Dados.ITEM()
            {
                ID_ITEM = Guid.NewGuid(),
                DES_ITEM = Item.Descricao,
                ID_ORCAMENTO = Orcamento.ID_ORCAMENTO,
                NUM_QUANTIDADE = Item.Quantidade,
                NUM_VALOR_CUSTO = Item.ValorCusto,
                NUM_VALOR_UNITARIO = Item.ValorUnitario,
                NUM_DESCONTO = Item.ValorDesconto,
                LOGIN_USUARIO = UsuarioLogado,
                DATA_ATUALIZACAO = DateTime.Now
            };

            if (Item.Produto != null)
            {
                tItem.ID_PRODUTO = Item.Produto.Id;
                if (Item.UnidadeMedida != null)
                    tItem.ID_UNIDADE_MEDIDA = Item.UnidadeMedida.Id;

                // Verifica se o orçamento foi aprovado
                if (Orcamento.T_ESTADO_ORCAMENTO != null && Orcamento.T_ESTADO_ORCAMENTO.COD_ESTADO_ORCAMENTO == string.Format("0{0}", (int)Contrato.Enumeradores.EstadoOrcamento.Aprovado))
                {
                    // Atualiza a quantidade de produtos
                    if (Item.UnidadeMedida != null && Item.UnidadeMedida.Id != Guid.Empty)
                    {
                        Contrato.UnidadeMedida uMedida = Item.Produto.UnidadeMedidas.Where(um => um.Id == Item.UnidadeMedida.Id).FirstOrDefault();
                        if (uMedida != null)
                        {
                            uMedida.Quantidade = uMedida.Quantidade - Item.Quantidade;
                            context.T_PRODUTO_UNIDADE_MEDIDA.Where(pum => pum.ID_UNIDADE_MEDIDA == uMedida.Id && pum.ID_PRODUTO == Item.Produto.Id).FirstOrDefault().NUM_QUANTIDADE = uMedida.Quantidade;
                        }
                    }
                    else
                    {
                        Item.Produto.Quantidade = Item.Produto.Quantidade - Item.Quantidade;
                        context.T_PRODUTO.Where(p => p.ID_PRODUTO == Item.Produto.Id).FirstOrDefault().NUM_QUANTIDADE = Item.Produto.Quantidade;
                    }
                }
            }

            Orcamento.T_ITEM.Add(tItem);

            // Salva as alterações
            context.SaveChanges();

            // Preenche o objeto de retorno
            retItem.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;

            // retorna dos dados 
            return retItem;
        }

    }
}
