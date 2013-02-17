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
                            Produto = Negocio.Produto.BuscarProduto(item.T_PRODUTO),
                            Quantidade = item.NUM_QUANTIDADE,
                            ValorCusto = item.NUM_VALOR_CUSTO,
                            ValorUnitario = item.NUM_VALOR_UNITARIO,
                            ValorDesconto = item.NUM_DESCONTO,
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
        /// <param name="lstUsuarioTaxa">Recebe os itens do orçamento recuperado do banco</param>
        /// <returns>List<Contrato.Taxa></returns>
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
                        Produto = Negocio.Produto.BuscarProduto(item.T_PRODUTO),
                        Quantidade = item.NUM_QUANTIDADE,
                        ValorCusto = item.NUM_VALOR_CUSTO,
                        ValorUnitario = item.NUM_VALOR_UNITARIO,
                        ValorDesconto = item.NUM_DESCONTO,
                        Orcamento = Negocio.Orcamento.BuscarOrcamento(item.T_ORCAMENTO, false)
                    });
                }
            }

            return lstItem;
        }

        /// <summary>
        /// Método para verificar se as informações do produto foram preenchidas
        /// </summary>
        /// <param name="Usuario">Objeto com o dados do produto</param>
        /// <returns></returns>
        private static string ValidarItemPreenchido(Contrato.Item Item)
        {
            // Cria a variável de retorno
            string strRetorno = string.Empty;

            // Verifica se o Codigo foi preenchido
            if (string.IsNullOrWhiteSpace(Item.Orcamento.Id.ToString()))
                strRetorno = "O campo 'Orçamento' não foi informado!\n";

            // Verifica se a Nome foi preenchida
            if (string.IsNullOrWhiteSpace(Item.Produto.Id.ToString()))
                strRetorno += "O campo 'Produto' não foi informado!\n";

            // retorna a variável de retorno
            return strRetorno;
        }       
    }
}
