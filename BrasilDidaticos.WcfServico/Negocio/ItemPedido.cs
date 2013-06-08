using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrasilDidaticos.WcfServico;

namespace BrasilDidaticos.WcfServico.Negocio
{
    static class ItemPedido
    {
        /// <summary>
        /// Método para listar os Itens
        /// </summary>
        /// <param name="Item">Objeto com os dados do filtro</param>
        /// <returns>Contrato.RetornoItem</returns>
        internal static Contrato.RetornoItemPedido ListarItem(Contrato.EntradaItemPedido entradaItemPedido)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoItemPedido retItemPedido = new Contrato.RetornoItemPedido();
            
            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaItemPedido.UsuarioLogado, Chave = entradaItemPedido.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Verifica se o fornecedor não foi informado
                if (entradaItemPedido.ItemPedido.Pedido == null)
                {
                    entradaItemPedido.ItemPedido.Pedido = new Contrato.Pedido();
                }

                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();
                                                
                // Busca o produto no banco
                List<Dados.ITEM_PEDIDO> lstItems = null;
                    
                if (entradaItemPedido.Paginar)
                {
                    lstItems = (from i in context.T_ITEM_PEDIDO
                                where
                                (entradaItemPedido.ItemPedido.Pedido.Id == Guid.Empty || i.ID_PEDIDO == entradaItemPedido.ItemPedido.Pedido.Id)
                                select i                                                           
                                ).OrderBy(o => o.ID_PRODUTO).Skip(entradaItemPedido.PosicaoUltimoItem).Take(entradaItemPedido.CantidadeItens).ToList();
                }
                else
                {
                    lstItems = (from i in context.T_ITEM_PEDIDO
                                where
                                (entradaItemPedido.ItemPedido.Pedido.Id == Guid.Empty || i.ID_PEDIDO == entradaItemPedido.ItemPedido.Pedido.Id)
                                select i
                                ).ToList();
                }
                                
                // Verifica se foi encontrado algum registro
                if (lstItems.Count > 0)
                {
                    // Preenche o objeto de retorno
                    retItemPedido.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    retItemPedido.Itens = new List<Contrato.ItemPedido>();
                    foreach (Dados.ITEM_PEDIDO item in lstItems)
                    {
                        retItemPedido.Itens.Add(new Contrato.ItemPedido()
                        {
                            Id = item.ID_ITEM_PEDIDO,
                            Produto = Negocio.Produto.BuscarProduto(item.T_PRODUTO),
                            Quantidade = item.NUM_QUANTIDADE,
                            Valor = item.NUM_VALOR,                            
                            ValorDesconto = item.NUM_DESCONTO,
                            Pedido = Negocio.Pedido.BuscarPedido(item.T_PEDIDO)
                        });
                    };
                }
                else
                {
                    // Preenche o objeto de retorno
                    retItemPedido.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;
                    retItemPedido.Mensagem = "Não existe dados para o filtro informado.";
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retItemPedido.Codigo = retSessao.Codigo;
                retItemPedido.Mensagem = retSessao.Mensagem;
            }
            
            // retorna os dados
            return retItemPedido;
        }

        /// <summary>
        /// Retorna uma lista de itens 
        /// </summary>
        /// <param name="lstPedidoItens">Recebe os itens do pedido recuperado do banco</param>
        /// <returns>List<Contrato.Taxa></returns>
        internal static List<Contrato.ItemPedido> ListarPedidoItem(System.Data.Objects.DataClasses.EntityCollection<Dados.ITEM_PEDIDO> lstPedidoItens)
        {
            List<Contrato.ItemPedido> lstItem = null;

            if (lstPedidoItens != null)
            {
                lstItem = new List<Contrato.ItemPedido>();

                foreach (Dados.ITEM_PEDIDO item in lstPedidoItens)
                {
                    lstItem.Add(new Contrato.ItemPedido
                    {
                        Id = item.ID_ITEM_PEDIDO,                        
                        Produto = Negocio.Produto.BuscarProduto(item.T_PRODUTO),
                        UnidadeMedida = item.T_PRODUTO == null ? null : Negocio.UnidadeMedida.BuscarProdutoUnidadeMedida(item.T_PRODUTO.T_PRODUTO_UNIDADE_MEDIDA.Where(pum => pum.ID_UNIDADE_MEDIDA == item.ID_UNIDADE_MEDIDA).FirstOrDefault()),
                        Quantidade = item.NUM_QUANTIDADE,
                        Valor = item.NUM_VALOR,                        
                        ValorDesconto = item.NUM_DESCONTO,
                        Pedido = Negocio.Pedido.BuscarPedido(item.T_PEDIDO, false)
                    });
                }
            }

            return lstItem;
        }

        /// <summary>
        /// Método para salvar os item do pedido
        /// </summary>
        /// <param name="ItemPedido">Objeto com os dados do item</param>
        /// <returns>Contrato.RetornoItem</returns>
        internal static Contrato.RetornoItemPedido SalvarItemPedidoPedido(Dados.PEDIDO Pedido, string UsuarioLogado, Contrato.ItemPedido ItemPedido)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoItemPedido retItemPedido = new Contrato.RetornoItemPedido();

            // Loga no banco de dados
            Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

            // Cria o item
            Dados.ITEM_PEDIDO tItemPedido = new Dados.ITEM_PEDIDO()
            {
                ID_ITEM_PEDIDO = Guid.NewGuid(),
                ID_PEDIDO = Pedido.ID_PEDIDO,
                NUM_QUANTIDADE = ItemPedido.Quantidade,
                NUM_VALOR = ItemPedido.Valor,                
                NUM_DESCONTO = ItemPedido.ValorDesconto,                
                LOGIN_USUARIO = UsuarioLogado,
                DATA_ATUALIZACAO = DateTime.Now
            };

            if (ItemPedido.Produto != null)
            {
                tItemPedido.ID_PRODUTO = ItemPedido.Produto.Id;
                if (ItemPedido.UnidadeMedida != null)
                    tItemPedido.ID_UNIDADE_MEDIDA = ItemPedido.UnidadeMedida.Id;

                // Verifica se o pedido foi aprovado
                if (Pedido.T_ESTADO_PEDIDO != null && Pedido.T_ESTADO_PEDIDO.COD_ESTADO_PEDIDO == string.Format("0{0}", (int)Contrato.Enumeradores.EstadoPedido.Aprovado))
                {
                    // Atualiza a quantidade de produtos
                    if (ItemPedido.UnidadeMedida != null && ItemPedido.UnidadeMedida.Id != Guid.Empty)
                    {
                        Contrato.UnidadeMedida uMedida = ItemPedido.Produto.UnidadeMedidas.Where(um => um.Id == ItemPedido.UnidadeMedida.Id).FirstOrDefault();
                        if (uMedida != null)
                        {
                            uMedida.Quantidade = uMedida.Quantidade + ItemPedido.Quantidade;
                            context.T_PRODUTO.Where(p => p.ID_PRODUTO == ItemPedido.Produto.Id).FirstOrDefault().NUM_VALOR = ItemPedido.Valor / uMedida.QuantidadeItens;
                            context.T_PRODUTO_UNIDADE_MEDIDA.Where(pum => pum.ID_UNIDADE_MEDIDA == uMedida.Id && pum.ID_PRODUTO == ItemPedido.Produto.Id).FirstOrDefault().NUM_QUANTIDADE = uMedida.Quantidade;                           
                        }
                    }
                    else
                    {
                        ItemPedido.Produto.Quantidade = ItemPedido.Produto.Quantidade + ItemPedido.Quantidade;
                        Dados.PRODUTO objProduto = context.T_PRODUTO.Where(p => p.ID_PRODUTO == ItemPedido.Produto.Id).FirstOrDefault();
                        objProduto.NUM_QUANTIDADE = ItemPedido.Quantidade;
                        objProduto.NUM_VALOR = ItemPedido.Valor;
                    }
                }                
            }
 
            Pedido.T_ITEM_PEDIDO.Add(tItemPedido);

            // Salva as alterações
            context.SaveChanges();

            // Preenche o objeto de retorno
            retItemPedido.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;

            // retorna dos dados 
            return retItemPedido;
        }
    }
}
