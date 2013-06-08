using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrasilDidaticos.WcfServico;

namespace BrasilDidaticos.WcfServico.Negocio
{
    static class EstadoPedido
    {
        /// <summary>
        /// Método para listar os estados do pedido
        /// </summary>
        /// <param name="entradaEstadoPedido.Permissoes">Objeto com os dados do filtro</param>
        /// <returns>Contrato.RetornoEstadoPedido</returns>
        internal static Contrato.RetornoEstadoPedido ListarEstadoPedido(Contrato.EntradaEstadoPedido entradaEstadoPedido)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoEstadoPedido retEstadoPedido = new Contrato.RetornoEstadoPedido();
             
            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaEstadoPedido.UsuarioLogado, Chave = entradaEstadoPedido.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {

                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                // Busca o estadoPedido no banco
                List<Dados.ESTADO_PEDIDO> lstPermissoes = (from f in context.T_ESTADO_PEDIDO
                                                              where (entradaEstadoPedido.EstadoPedido.Ativo == null || f.BOL_ATIVO == entradaEstadoPedido.EstadoPedido.Ativo)
                                                select f).ToList();

                // Verifica se foi encontrado algum registro
                if (lstPermissoes.Count > 0)
                {
                    // Preenche o objeto de retorno
                    retEstadoPedido.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    retEstadoPedido.EstadosPedido = new List<Contrato.EstadoPedido>();
                    foreach (Dados.ESTADO_PEDIDO estadoPedido in lstPermissoes)
                    {
                        retEstadoPedido.EstadosPedido.Add(new Contrato.EstadoPedido()
                        {
                            Id = estadoPedido.ID_ESTADO_PEDIDO,
                            Codigo = estadoPedido.COD_ESTADO_PEDIDO,
                            Nome = estadoPedido.NOME_ESTADO_PEDIDO,
                            Ativo = estadoPedido.BOL_ATIVO,
                            Anterior = Negocio.EstadoPedido.BuscarPedidoEstadoPedido(estadoPedido.T_ESTADO_PEDIDO_ANTERIOR),
                            Sucessor = Negocio.EstadoPedido.BuscarPedidoEstadoPedido(estadoPedido.T_ESTADO_PEDIDO_SUCESSOR)
                        });
                    };
                }
                else
                {
                    // Preenche o objeto de retorno
                    retEstadoPedido.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;
                    retEstadoPedido.Mensagem = "Não existe dados para o filtro informado.";
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retEstadoPedido.Codigo = retSessao.Codigo;
                retEstadoPedido.Mensagem = retSessao.Mensagem;
            }
            
            // retorna os dados
            return retEstadoPedido;
        }

        /// <summary>
        /// Retorna o estado do orçamento
        /// </summary>
        /// <param name="estadoPedido">Recebe o estado do pedido recuperado do banco</param>
        /// <returns>Contrato.EstadoPedido</returns>
        internal static Contrato.EstadoPedido BuscarPedidoEstadoPedido(Dados.ESTADO_PEDIDO estadoPedido)
        {
            // Objeto que recebe o retorno do método
            Contrato.EstadoPedido retEstadoPedido = new Contrato.EstadoPedido();

            // Verifica se foi encontrado algum registro
            if (estadoPedido != null)
            {
                retEstadoPedido = new Contrato.EstadoPedido()
                {
                    Id = estadoPedido.ID_ESTADO_PEDIDO,
                    Codigo = estadoPedido.COD_ESTADO_PEDIDO,
                    Nome = estadoPedido.NOME_ESTADO_PEDIDO,
                    Ativo = estadoPedido.BOL_ATIVO
                };
            }

            // retorna os dados
            return retEstadoPedido;
        }
    }
}
