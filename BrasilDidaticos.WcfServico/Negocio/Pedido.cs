using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrasilDidaticos.WcfServico;

namespace BrasilDidaticos.WcfServico.Negocio
{
    static class Pedido
    {
        /// <summary>
        /// Método para buscar o código do pedido
        /// </summary>        
        /// <returns>string</returns>
        internal static string BuscarCodigoPedido(Guid IdEmpresa)
        {
            // Objeto que recebe o retorno do método
            string retCodigoPedido = string.Empty;

            // Loga no banco de dados
            Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();
            System.Data.Objects.ObjectParameter objCodigoPedido = new System.Data.Objects.ObjectParameter("P_CODIGO", typeof(global::System.Int32));
            context.RETORNAR_CODIGO(Contrato.Constantes.TIPO_COD_PEDIDO, IdEmpresa, objCodigoPedido);

            // Recupera o código do fornecedor
            retCodigoPedido = Util.RecuperaCodigo((int)objCodigoPedido.Value, Contrato.Constantes.TIPO_COD_PEDIDO);

            // retorna os dados
            return retCodigoPedido;
        }

        /// <summary>
        /// Método para buscar o pedido
        /// </summary>
        /// <param name="Pedido">Objeto com o identificador do pedido</param>
        /// <returns>Contrato.RetornoPedido</returns>
        internal static Contrato.Pedido BuscarPedido(Dados.PEDIDO pedido)
        {
            // retorna os dados
            return BuscarPedido(pedido, true);
        }

        /// <summary>
        /// Método para buscar o pedido
        /// </summary>
        /// <param name="Pedido">Objeto com o identificador do pedido</param>
        /// <param name="carregarItens">Define se vai carregar os itens do pedido</param>
        /// <returns>Contrato.RetornoPedido</returns>
        internal static Contrato.Pedido BuscarPedido(Dados.PEDIDO pedido, bool carregarItens)
        {
            // Objeto que recebe o retorno do método
            Contrato.Pedido retPedido = new Contrato.Pedido();

            // Verifica se foi encontrado algum registro
            if (pedido != null)
            {
                retPedido = new Contrato.Pedido()
                {
                    Id = pedido.ID_PEDIDO,
                    Codigo = pedido.COD_PEDIDO,
                    Data = pedido.DATA_PEDIDO,
                    ValorDesconto = pedido.NUM_DESCONTO,
                    Estado = Negocio.EstadoPedido.BuscarPedidoEstadoPedido(pedido.T_ESTADO_PEDIDO),                    
                    Responsavel = Negocio.Usuario.BuscarUsuario(pedido.T_USUARIO),                    
                    ItensPedido = carregarItens ? Negocio.ItemPedido.ListarPedidoItem(pedido.T_ITEM_PEDIDO) : null
                };
            }

            // retorna os dados
            return retPedido;
        }

        /// <summary>
        /// Método para listar os itens do pedido
        /// </summary>
        /// <param name="Pedido">Objeto com os dados do filtro</param>
        /// <returns>Contrato.RetornoPedido</returns>
        internal static Contrato.RetornoPedido ListarPedido(Contrato.EntradaPedido entradaPedido)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoPedido retPedido = new Contrato.RetornoPedido();
            
            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaPedido.UsuarioLogado, Chave = entradaPedido.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Verifica se a empresa não foi informada
                if (string.IsNullOrWhiteSpace(entradaPedido.EmpresaLogada.Id.ToString()))
                {
                    entradaPedido.EmpresaLogada.Id = Guid.Empty;
                }

                // Verifica se o código não foi informado
                if (string.IsNullOrWhiteSpace(entradaPedido.Pedido.Codigo))
                {
                    entradaPedido.Pedido.Codigo = string.Empty;
                }                
                
                // Verifica se o Responsável não foi informado
                if (entradaPedido.Pedido.Responsavel == null)
                {
                    entradaPedido.Pedido.Responsavel = new Contrato.Usuario();
                }

                // Verifica se o estado do pedido não foi informado
                if (entradaPedido.Pedido.Estado == null)
                {
                    entradaPedido.Pedido.Estado = new Contrato.EstadoPedido();
                }

                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();
                context.ContextOptions.LazyLoadingEnabled = true;                               
                    
                if (entradaPedido.Paginar)
                {
                    // Busca o pedido no banco
                    var lstPedidos = (from p in context.T_PEDIDO
                                      where
                                          (entradaPedido.EmpresaLogada.Id == Guid.Empty || p.ID_EMPRESA == entradaPedido.EmpresaLogada.Id)
                                      &&  (p.DATA_PEDIDO >= entradaPedido.Pedido.Data)
                                      &&  (entradaPedido.Pedido.Codigo == string.Empty || p.COD_PEDIDO.Contains(entradaPedido.Pedido.Codigo))                                    
                                      &&  (entradaPedido.Pedido.Responsavel.Id == Guid.Empty || p.ID_USUARIO_RESPONSAVEL == entradaPedido.Pedido.Responsavel.Id)
                                      &&  (entradaPedido.Pedido.Estado.Id == Guid.Empty || p.ID_ESTADO_PEDIDO == entradaPedido.Pedido.Estado.Id)                                    
                                      select p                                                           
                                     ).OrderByDescending(p => p.DATA_PEDIDO).Skip(entradaPedido.PosicaoUltimoItem).Take(entradaPedido.CantidadeItens)
                                     .Select(p => new
                                     {
                                         o = p,
                                         e = p.T_ESTADO_PEDIDO,                                                                                  
                                         r = p.T_USUARIO,
                                         i = p.T_ITEM_PEDIDO
                                     }).ToList();

                    // Verifica se foi encontrado algum registro
                    if (lstPedidos.Count > 0)
                    {
                        // Preenche o objeto de retorno
                        retPedido.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                        retPedido.Pedidos = new List<Contrato.Pedido>();

                        foreach (var item in lstPedidos)
                        {
                            retPedido.Pedidos.Add(new Contrato.Pedido()
                            {
                                Id = item.o.ID_PEDIDO,
                                Codigo = item.o.COD_PEDIDO,
                                Data = item.o.DATA_PEDIDO,
                                ValorDesconto = item.o.NUM_DESCONTO,                                
                                Estado = Negocio.EstadoPedido.BuscarPedidoEstadoPedido(item.e),
                                Responsavel = Negocio.Usuario.BuscarUsuario(item.r),
                                ItensPedido = Negocio.ItemPedido.ListarPedidoItem(item.i)
                            });
                        }
                    }
                }
                else
                {
                    var lstPedidos = (from p in context.T_PEDIDO
                                      where
                                          (entradaPedido.EmpresaLogada.Id == Guid.Empty || p.ID_EMPRESA == entradaPedido.EmpresaLogada.Id)
                                      &&  (entradaPedido.Pedido.Codigo == string.Empty || p.COD_PEDIDO.Contains(entradaPedido.Pedido.Codigo))
                                      &&  (entradaPedido.Pedido.Responsavel.Id == Guid.Empty || p.ID_USUARIO_RESPONSAVEL == entradaPedido.Pedido.Responsavel.Id)
                                      &&  (entradaPedido.Pedido.Estado.Id == Guid.Empty || p.ID_ESTADO_PEDIDO == entradaPedido.Pedido.Estado.Id)
                                      select new { p, i = p.T_ITEM_PEDIDO }).ToList();

                    // Verifica se foi encontrado algum registro
                    if (lstPedidos.Count > 0)
                    {
                        // Preenche o objeto de retorno
                        retPedido.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                        retPedido.Pedidos = new List<Contrato.Pedido>();

                        foreach (var item in lstPedidos)
                        {
                            retPedido.Pedidos.Add(new Contrato.Pedido()
                            {
                                Id = item.p.ID_PEDIDO,
                                Codigo = item.p.COD_PEDIDO,
                                Data = item.p.DATA_PEDIDO,
                                ValorDesconto = item.p.NUM_DESCONTO,                                
                                Estado = Negocio.EstadoPedido.BuscarPedidoEstadoPedido(item.p.T_ESTADO_PEDIDO),
                                Responsavel = Negocio.Usuario.BuscarUsuario(item.p.T_USUARIO),
                                ItensPedido = Negocio.ItemPedido.ListarPedidoItem(item.i)
                            });
                        }
                    }
                }                                
                
                if (retPedido.Pedidos == null || retPedido.Pedidos.Count == 0)
                {
                    // Preenche o objeto de retorno
                    retPedido.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;
                    retPedido.Mensagem = "Não existe dados para o filtro informado.";
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retPedido.Codigo = retSessao.Codigo;
                retPedido.Mensagem = retSessao.Mensagem;
            }
            
            // retorna os dados
            return retPedido;
        }

        /// <summary>
        /// Método para salvar o pedido
        /// </summary>
        /// <param name="entradaPedido">Objeto com os dados do pedido</param>
        /// <returns>Contrato.RetornoPedido</returns>
        internal static Contrato.RetornoPedido SalvarPedido(Contrato.EntradaPedido entradaPedido)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoPedido retPedido = new Contrato.RetornoPedido();

            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaPedido.UsuarioLogado, Chave = entradaPedido.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Verifica se as informações do pedido foram informadas
                string strValidacao = ValidarPedidoPreenchido(entradaPedido.Pedido);
            
                // Se existe algum erro
                if (strValidacao.Length > 0)
                {
                    retPedido.Codigo = Contrato.Constantes.COD_FILTRO_VAZIO;
                    retPedido.Mensagem = strValidacao;
                }
                else
                {
                    // Loga no banco de dados
                    Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();
                    context.ContextOptions.LazyLoadingEnabled = true;

                    // Busca o pedido no banco
                    List<Dados.PEDIDO> lstPedidos = (from p in context.T_PEDIDO
                                                           where (p.COD_PEDIDO == entradaPedido.Pedido.Codigo
                                                                 && (entradaPedido.EmpresaLogada.Id == Guid.Empty || p.ID_EMPRESA == entradaPedido.EmpresaLogada.Id))
                                                              || (entradaPedido.Novo == null && entradaPedido.Pedido.Id == p.ID_PEDIDO)
                                                           select p).ToList();

                    // Verifica se foi encontrado algum registro
                    if (lstPedidos.Count > 0 && entradaPedido.Novo != null && (bool)entradaPedido.Novo)
                    {
                        // Preenche o objeto de retorno
                        retPedido.Codigo = Contrato.Constantes.COD_REGISTRO_DUPLICADO;
                        retPedido.Mensagem = string.Format("O pedido de código '{0}' já existe!", lstPedidos.First().COD_PEDIDO);
                    }
                    else
                    {
                        // Se existe o pedido
                        if (lstPedidos.Count > 0)
                        {
                            // Atualiza o pedido                            
                            lstPedidos.First().COD_PEDIDO = entradaPedido.Pedido.Codigo;
                            lstPedidos.First().DATA_PEDIDO = entradaPedido.Pedido.Data;                            
                            lstPedidos.First().ID_ESTADO_PEDIDO = entradaPedido.Pedido.Estado.Id;                            
                            lstPedidos.First().ID_USUARIO_RESPONSAVEL = entradaPedido.Pedido.Responsavel.Id;
                            lstPedidos.First().NUM_DESCONTO = entradaPedido.Pedido.ValorDesconto;
                            lstPedidos.First().DATA_ATUALIZACAO = DateTime.Now;
                            lstPedidos.First().LOGIN_USUARIO = entradaPedido.UsuarioLogado;

                            // Apaga todos os itens que estão relacionados
                            while (lstPedidos.First().T_ITEM_PEDIDO.Count > 0)
                            {
                                context.T_ITEM_PEDIDO.DeleteObject(lstPedidos.First().T_ITEM_PEDIDO.First());
                            }

                            // Verifica se existe algum item associado ao pedido
                            if (entradaPedido.Pedido.ItensPedido != null)
                            {
                                // Para cada item associado
                                foreach (Contrato.ItemPedido item in entradaPedido.Pedido.ItensPedido)
                                {
                                    Negocio.ItemPedido.SalvarItemPedidoPedido(lstPedidos.First(), entradaPedido.UsuarioLogado, item);
                                }
                            }
                        }
                        else
                        {
                            // Recupera o código do pedido
                            string codigoPedido = string.Empty;
                            if (entradaPedido.Pedido.Codigo != string.Empty)
                                codigoPedido = entradaPedido.Pedido.Codigo;
                            else
                            {
                                System.Data.Objects.ObjectParameter objCodigoPedido = new System.Data.Objects.ObjectParameter("P_CODIGO", typeof(global::System.Int32));
                                context.RETORNAR_CODIGO(Contrato.Constantes.TIPO_COD_PEDIDO, entradaPedido.EmpresaLogada.Id, objCodigoPedido);
                                codigoPedido = Util.RecuperaCodigo((int)objCodigoPedido.Value, Contrato.Constantes.TIPO_COD_PEDIDO);
                            }

                            // Cria o pedido
                            Dados.PEDIDO tPedido = new Dados.PEDIDO();
                            tPedido.ID_PEDIDO = Guid.NewGuid();
                            tPedido.COD_PEDIDO = codigoPedido;
                            tPedido.DATA_PEDIDO = entradaPedido.Pedido.Data;
                            tPedido.ID_EMPRESA = entradaPedido.EmpresaLogada.Id;
                            tPedido.ID_ESTADO_PEDIDO = entradaPedido.Pedido.Estado.Id;
                            tPedido.ID_USUARIO_RESPONSAVEL = entradaPedido.Pedido.Responsavel.Id;
                            tPedido.NUM_DESCONTO = entradaPedido.Pedido.ValorDesconto;
                            tPedido.DATA_ATUALIZACAO = DateTime.Now;
                            tPedido.LOGIN_USUARIO = entradaPedido.UsuarioLogado;                                                       

                            // Verifica se existe algum item associado ao orçamento
                            if (entradaPedido.Pedido.ItensPedido != null)
                            {
                                // Para cada item associado
                                foreach (Contrato.ItemPedido item in entradaPedido.Pedido.ItensPedido)
                                {
                                    Negocio.ItemPedido.SalvarItemPedidoPedido(tPedido, entradaPedido.UsuarioLogado, item);
                                }
                            }

                            context.AddToT_PEDIDO(tPedido);
                        }

                        // Salva as alterações
                        context.SaveChanges();

                        // Preenche o objeto de retorno
                        retPedido.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    }
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retPedido.Codigo = retSessao.Codigo;
                retPedido.Mensagem = retSessao.Mensagem;
            }

            // retorna dos dados 
            return retPedido;
        }

        /// <summary>
        /// Método para verificar se as informações do pedido foram preenchidas
        /// </summary>
        /// <param name="Usuario">Objeto com o dados do pedido</param>
        /// <returns></returns>
        private static string ValidarPedidoPreenchido(Contrato.Pedido Pedido)
        {
            // Cria a variável de retorno
            string strRetorno = string.Empty;

            // Verifica se a Data foi preenchida
            if (Pedido.Data == null)
                strRetorno = "O campo 'Data' não foi informado!\n";
                        
            // Verifica se o Responsável foi informado
            if (string.IsNullOrWhiteSpace(Pedido.Responsavel.Id.ToString()))
                strRetorno += "O campo 'Responsavel' não foi informado!\n";

            // retorna a variável de retorno
            return strRetorno;
        }       
    }
}
