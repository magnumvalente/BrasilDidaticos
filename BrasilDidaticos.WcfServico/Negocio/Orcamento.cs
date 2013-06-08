using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrasilDidaticos.WcfServico;

namespace BrasilDidaticos.WcfServico.Negocio
{
    static class Orcamento
    {
        /// <summary>
        /// Método para buscar o código do orçamento
        /// </summary>        
        /// <returns>string</returns>
        internal static string BuscarCodigoOrcamento(Guid IdEmpresa)
        {
            // Objeto que recebe o retorno do método
            string retCodigoOrcamento = string.Empty;

            // Loga no banco de dados
            Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();
            System.Data.Objects.ObjectParameter objCodigoOrcamento = new System.Data.Objects.ObjectParameter("P_CODIGO", typeof(global::System.Int32));            
            context.RETORNAR_CODIGO(Contrato.Constantes.TIPO_COD_ORCAMENTO, IdEmpresa, objCodigoOrcamento);

            // Recupera o código do fornecedor
            retCodigoOrcamento = Util.RecuperaCodigo((int)objCodigoOrcamento.Value, Contrato.Constantes.TIPO_COD_ORCAMENTO);

            // retorna os dados
            return retCodigoOrcamento;
        }

        /// <summary>
        /// Método para buscar o orçamento
        /// </summary>
        /// <param name="Orcamento">Objeto com o identificador do orçamento</param>
        /// <returns>Contrato.RetornoOrcamento</returns>
        internal static Contrato.Orcamento BuscarOrcamento(Dados.ORCAMENTO orcamento)
        {
            // retorna os dados
            return BuscarOrcamento(orcamento, true);
        }

        /// <summary>
        /// Método para buscar o orçamento
        /// </summary>
        /// <param name="Orcamento">Objeto com o identificador do orçamento</param>
        /// <param name="carregarItens">Define se vai carregar os itens</param>
        /// <returns>Contrato.RetornoOrcamento</returns>
        internal static Contrato.Orcamento BuscarOrcamento(Dados.ORCAMENTO orcamento, bool carregarItens)
        {
            // Objeto que recebe o retorno do método
            Contrato.Orcamento retOrcamento = new Contrato.Orcamento();

            // Verifica se foi encontrado algum registro
            if (orcamento != null)
            {
                retOrcamento = new Contrato.Orcamento()
                {
                    Id = orcamento.ID_ORCAMENTO,
                    Codigo = orcamento.COD_ORCAMENTO,
                    Data = orcamento.DATA_ORCAMENTO,
                    ValorDesconto = orcamento.NUM_DESCONTO,
                    PrazoEntrega = orcamento.NUM_PRAZO_ENTREGA,
                    ValidadeOrcamento = orcamento.NUM_VALIDADE_ORCAMENTO,
                    Estado = Negocio.EstadoOrcamento.BuscarOrcamentoEstadoOrcamento(orcamento.T_ESTADO_ORCAMENTO),
                    Cliente = Negocio.Cliente.BuscarCliente(orcamento.T_CLIENTE),
                    Responsavel = Negocio.Usuario.BuscarUsuario(orcamento.T_USUARIO_RESPOSANVEL),
                    Vendedor = Negocio.Usuario.BuscarUsuario(orcamento.T_USUARIO_VENDEDOR),
                    Itens = carregarItens ? Negocio.Item.ListarOrcamentoItem(orcamento.T_ITEM) : null
                };
            }

            // retorna os dados
            return retOrcamento;
        }

        /// <summary>
        /// Método para listar os orcamentos
        /// </summary>
        /// <param name="Orcamento">Objeto com os dados do filtro</param>
        /// <returns>Contrato.RetornoOrcamento</returns>
        internal static Contrato.RetornoOrcamento ListarOrcamento(Contrato.EntradaOrcamento entradaOrcamento)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoOrcamento retOrcamento = new Contrato.RetornoOrcamento();
            
            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaOrcamento.UsuarioLogado, Chave = entradaOrcamento.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Verifica se a empresa não foi informada
                if (string.IsNullOrWhiteSpace(entradaOrcamento.EmpresaLogada.Id.ToString()))
                {
                    entradaOrcamento.EmpresaLogada.Id = Guid.Empty;
                }

                // Verifica se o código não foi informado
                if (string.IsNullOrWhiteSpace(entradaOrcamento.Orcamento.Codigo))
                {
                    entradaOrcamento.Orcamento.Codigo = string.Empty;
                }                

                // Verifica se o cliente não foi informado
                if (entradaOrcamento.Orcamento.Cliente == null)
                {
                    entradaOrcamento.Orcamento.Cliente = new Contrato.Cliente();
                }

                // Verifica se o Vendedor não foi informado
                if (entradaOrcamento.Orcamento.Vendedor == null)
                {
                    entradaOrcamento.Orcamento.Vendedor = new Contrato.Usuario();
                }

                // Verifica se o Responsável não foi informado
                if (entradaOrcamento.Orcamento.Responsavel == null)
                {
                    entradaOrcamento.Orcamento.Responsavel = new Contrato.Usuario();
                }

                // Verifica se o estado do orçamento não foi informado
                if (entradaOrcamento.Orcamento.Estado == null)
                {
                    entradaOrcamento.Orcamento.Estado = new Contrato.EstadoOrcamento();
                }

                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();                                                
                    
                if (entradaOrcamento.Paginar)
                {
                    // Busca o orcamento no banco
                    var lstOrcamentos = (from o in context.T_ORCAMENTO
                                    where
                                        (entradaOrcamento.EmpresaLogada.Id == Guid.Empty || o.ID_EMPRESA == entradaOrcamento.EmpresaLogada.Id)
                                    &&  (entradaOrcamento.Orcamento.Codigo == string.Empty || o.COD_ORCAMENTO.Contains(entradaOrcamento.Orcamento.Codigo))
                                    &&  (entradaOrcamento.Orcamento.Cliente.Id == Guid.Empty || o.ID_CLIENTE == entradaOrcamento.Orcamento.Cliente.Id)
                                    &&  (entradaOrcamento.Orcamento.Vendedor.Id == Guid.Empty || o.ID_USUARIO_VENDEDOR == entradaOrcamento.Orcamento.Vendedor.Id)
                                    &&  (entradaOrcamento.Orcamento.Responsavel.Id == Guid.Empty || o.ID_USUARIO_RESPONSAVEL == entradaOrcamento.Orcamento.Responsavel.Id)
                                    &&  (entradaOrcamento.Orcamento.Estado.Id == Guid.Empty || o.ID_ESTADO_ORCAMENTO == entradaOrcamento.Orcamento.Estado.Id)                                    
                                    select o                                                           
                                    ).OrderByDescending(o => o.DATA_ORCAMENTO).Skip(entradaOrcamento.PosicaoUltimoItem).Take(entradaOrcamento.CantidadeItens)
                                     .Select(o => new
                                     {
                                         o,
                                         e = o.T_ESTADO_ORCAMENTO,
                                         c = o.T_CLIENTE,
                                         v = o.T_USUARIO_VENDEDOR,
                                         r = o.T_USUARIO_RESPOSANVEL,
                                         i = o.T_ITEM
                                     }).ToList();

                    // Verifica se foi encontrado algum registro
                    if (lstOrcamentos.Count > 0)
                    {
                        // Preenche o objeto de retorno
                        retOrcamento.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                        retOrcamento.Orcamentos = new List<Contrato.Orcamento>();

                        foreach (var item in lstOrcamentos)
                        {
                            retOrcamento.Orcamentos.Add(new Contrato.Orcamento()
                            {
                                Id = item.o.ID_ORCAMENTO,
                                Codigo = item.o.COD_ORCAMENTO,
                                Data = item.o.DATA_ORCAMENTO,
                                ValorDesconto = item.o.NUM_DESCONTO,
                                PrazoEntrega = item.o.NUM_PRAZO_ENTREGA,
                                ValidadeOrcamento = item.o.NUM_VALIDADE_ORCAMENTO,
                                Estado = Negocio.EstadoOrcamento.BuscarOrcamentoEstadoOrcamento(item.e),
                                Cliente = Negocio.Cliente.BuscarCliente(item.c),
                                Responsavel = Negocio.Usuario.BuscarUsuario(item.r),
                                Vendedor = Negocio.Usuario.BuscarUsuario(item.v),
                                Itens = Negocio.Item.ListarOrcamentoItem(item.i)
                            });
                        }
                    }
                }
                else
                {
                    var lstOrcamentos = (from o in context.T_ORCAMENTO
                                   where
                                        (entradaOrcamento.Orcamento.Codigo == string.Empty || o.COD_ORCAMENTO.StartsWith(entradaOrcamento.Orcamento.Codigo))                                   
                                   &&   (entradaOrcamento.Orcamento.Cliente.Id == Guid.Empty || o.ID_CLIENTE == entradaOrcamento.Orcamento.Cliente.Id)
                                   select new { o, i = o.T_ITEM } ).ToList();

                    // Verifica se foi encontrado algum registro
                    if (lstOrcamentos.Count > 0)
                    {
                        // Preenche o objeto de retorno
                        retOrcamento.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                        retOrcamento.Orcamentos = new List<Contrato.Orcamento>();

                        foreach (var item in lstOrcamentos)
                        {
                            retOrcamento.Orcamentos.Add(new Contrato.Orcamento()
                            {
                                Id = item.o.ID_ORCAMENTO,
                                Codigo = item.o.COD_ORCAMENTO,
                                Data = item.o.DATA_ORCAMENTO,
                                ValorDesconto = item.o.NUM_DESCONTO,
                                PrazoEntrega = item.o.NUM_PRAZO_ENTREGA,
                                ValidadeOrcamento = item.o.NUM_VALIDADE_ORCAMENTO,
                                Estado = Negocio.EstadoOrcamento.BuscarOrcamentoEstadoOrcamento(item.o.T_ESTADO_ORCAMENTO),
                                Cliente = Negocio.Cliente.BuscarCliente(item.o.T_CLIENTE),
                                Responsavel = Negocio.Usuario.BuscarUsuario(item.o.T_USUARIO_RESPOSANVEL),
                                Vendedor = Negocio.Usuario.BuscarUsuario(item.o.T_USUARIO_VENDEDOR),
                                Itens = Negocio.Item.ListarOrcamentoItem(item.i)
                            });
                        }
                    }
                }                                
                
                if (retOrcamento.Orcamentos == null || retOrcamento.Orcamentos.Count == 0)
                {
                    // Preenche o objeto de retorno
                    retOrcamento.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;
                    retOrcamento.Mensagem = "Não existe dados para o filtro informado.";
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retOrcamento.Codigo = retSessao.Codigo;
                retOrcamento.Mensagem = retSessao.Mensagem;
            }
            
            // retorna os dados
            return retOrcamento;
        }

        /// <summary>
        /// Método para salvar o orcamento
        /// </summary>
        /// <param name="entradaOrcamento">Objeto com os dados do orcamento</param>
        /// <returns>Contrato.RetornoOrcamento</returns>
        internal static Contrato.RetornoOrcamento SalvarOrcamento(Contrato.EntradaOrcamento entradaOrcamento)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoOrcamento retOrcamento = new Contrato.RetornoOrcamento();

            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaOrcamento.UsuarioLogado, Chave = entradaOrcamento.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Verifica se as informações do orcamento foram informadas
                string strValidacao = ValidarOrcamentoPreenchido(entradaOrcamento.Orcamento);
            
                // Se existe algum erro
                if (strValidacao.Length > 0)
                {
                    retOrcamento.Codigo = Contrato.Constantes.COD_FILTRO_VAZIO;
                    retOrcamento.Mensagem = strValidacao;
                }
                else
                {
                    // Loga no banco de dados
                    Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                    // Busca o orcamento no banco
                    List<Dados.ORCAMENTO> lstOrcamentos = (from p in context.T_ORCAMENTO
                                                           where (p.COD_ORCAMENTO == entradaOrcamento.Orcamento.Codigo
                                                                 && (entradaOrcamento.EmpresaLogada.Id == Guid.Empty || p.ID_EMPRESA == entradaOrcamento.EmpresaLogada.Id))
                                                              || (entradaOrcamento.Novo == null && entradaOrcamento.Orcamento.Id == p.ID_ORCAMENTO)
                                                           select p).ToList();

                    // Verifica se foi encontrado algum registro
                    if (lstOrcamentos.Count > 0 && entradaOrcamento.Novo != null && (bool)entradaOrcamento.Novo)
                    {
                        // Preenche o objeto de retorno
                        retOrcamento.Codigo = Contrato.Constantes.COD_REGISTRO_DUPLICADO;
                        retOrcamento.Mensagem = string.Format("O orcamento de código '{0}' já existe!", lstOrcamentos.First().COD_ORCAMENTO);
                    }
                    else
                    {
                        // Se existe o orcamento
                        if (lstOrcamentos.Count > 0)
                        {
                            // Atualiza o orcamento                            
                            lstOrcamentos.First().COD_ORCAMENTO = entradaOrcamento.Orcamento.Codigo;
                            lstOrcamentos.First().DATA_ORCAMENTO = entradaOrcamento.Orcamento.Data;
                            lstOrcamentos.First().ID_CLIENTE = entradaOrcamento.Orcamento.Cliente.Id;
                            lstOrcamentos.First().ID_ESTADO_ORCAMENTO = entradaOrcamento.Orcamento.Estado.Id;
                            lstOrcamentos.First().ID_USUARIO_VENDEDOR = entradaOrcamento.Orcamento.Vendedor.Id;
                            lstOrcamentos.First().ID_USUARIO_RESPONSAVEL = entradaOrcamento.Orcamento.Responsavel.Id;
                            lstOrcamentos.First().NUM_DESCONTO = entradaOrcamento.Orcamento.ValorDesconto;
                            lstOrcamentos.First().NUM_PRAZO_ENTREGA = entradaOrcamento.Orcamento.PrazoEntrega;
                            lstOrcamentos.First().NUM_VALIDADE_ORCAMENTO = entradaOrcamento.Orcamento.ValidadeOrcamento;
                            lstOrcamentos.First().DATA_ATUALIZACAO = DateTime.Now;
                            lstOrcamentos.First().LOGIN_USUARIO = entradaOrcamento.UsuarioLogado;

                            // Apaga todos os itens que estão relacionados
                            while (lstOrcamentos.First().T_ITEM.Count > 0)
                            {
                                context.T_ITEM.DeleteObject(lstOrcamentos.First().T_ITEM.First());
                            }

                            // Verifica se existe algum item associado ao orçamento
                            if (entradaOrcamento.Orcamento.Itens != null)
                            {
                                // Para cada item associado
                                foreach (Contrato.Item item in entradaOrcamento.Orcamento.Itens)
                                {
                                    Negocio.Item.SalvarItemOrcamento(lstOrcamentos.First(), entradaOrcamento.UsuarioLogado, item);
                                }
                            }                            
                        }
                        else
                        {
                            // Recupera o código do orçamento
                            string codigoOrcamento = string.Empty;
                            if (entradaOrcamento.Orcamento.Codigo != string.Empty)
                                codigoOrcamento = entradaOrcamento.Orcamento.Codigo;
                            else
                            {
                                System.Data.Objects.ObjectParameter objCodigoOrcamento = new System.Data.Objects.ObjectParameter("P_CODIGO", typeof(global::System.Int32));
                                context.RETORNAR_CODIGO(Contrato.Constantes.TIPO_COD_ORCAMENTO, entradaOrcamento.EmpresaLogada.Id, objCodigoOrcamento);
                                codigoOrcamento = Util.RecuperaCodigo((int)objCodigoOrcamento.Value, Contrato.Constantes.TIPO_COD_ORCAMENTO);
                            }

                            // Cria o orcamento
                            Dados.ORCAMENTO tOrcamento = new Dados.ORCAMENTO();
                            tOrcamento.ID_ORCAMENTO = Guid.NewGuid();
                            tOrcamento.COD_ORCAMENTO = codigoOrcamento;
                            tOrcamento.DATA_ORCAMENTO = entradaOrcamento.Orcamento.Data;
                            tOrcamento.ID_EMPRESA = entradaOrcamento.EmpresaLogada.Id;
                            tOrcamento.ID_CLIENTE = entradaOrcamento.Orcamento.Cliente.Id;
                            tOrcamento.ID_ESTADO_ORCAMENTO = entradaOrcamento.Orcamento.Estado.Id;
                            tOrcamento.ID_USUARIO_VENDEDOR = entradaOrcamento.Orcamento.Vendedor.Id;
                            tOrcamento.ID_USUARIO_RESPONSAVEL = entradaOrcamento.Orcamento.Responsavel.Id;
                            tOrcamento.NUM_DESCONTO = entradaOrcamento.Orcamento.ValorDesconto;
                            tOrcamento.NUM_PRAZO_ENTREGA = entradaOrcamento.Orcamento.PrazoEntrega;
                            tOrcamento.NUM_VALIDADE_ORCAMENTO = entradaOrcamento.Orcamento.ValidadeOrcamento;
                            tOrcamento.DATA_ATUALIZACAO = DateTime.Now;
                            tOrcamento.LOGIN_USUARIO = entradaOrcamento.UsuarioLogado;                                                       

                            // Verifica se existe algum item associado ao orçamento
                            if (entradaOrcamento.Orcamento.Itens != null)
                            {
                                // Para cada item associado
                                foreach (Contrato.Item item in entradaOrcamento.Orcamento.Itens)
                                {
                                    Negocio.Item.SalvarItemOrcamento(tOrcamento, entradaOrcamento.UsuarioLogado, item);
                                }
                            }

                            context.AddToT_ORCAMENTO(tOrcamento);
                        }

                        // Salva as alterações
                        context.SaveChanges();

                        // Preenche o objeto de retorno
                        retOrcamento.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    }
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retOrcamento.Codigo = retSessao.Codigo;
                retOrcamento.Mensagem = retSessao.Mensagem;
            }

            // retorna dos dados 
            return retOrcamento;
        }

        /// <summary>
        /// Método para verificar se as informações do orcamento foram preenchidas
        /// </summary>
        /// <param name="Usuario">Objeto com o dados do orcamento</param>
        /// <returns></returns>
        private static string ValidarOrcamentoPreenchido(Contrato.Orcamento Orcamento)
        {
            // Cria a variável de retorno
            string strRetorno = string.Empty;

            // Verifica se a Data foi preenchida
            if (Orcamento.Data == null)
                strRetorno = "O campo 'Data' não foi informado!\n";

            // Verifica se o Cliente foi preenchido
            if (string.IsNullOrWhiteSpace(Orcamento.Cliente.Id.ToString()))
                strRetorno += "O campo 'Cliente' não foi informado!\n";

            // Verifica se o Responsável foi informado
            if (string.IsNullOrWhiteSpace(Orcamento.Responsavel.Id.ToString()))
                strRetorno += "O campo 'Responsavel' não foi informado!\n";

            // retorna a variável de retorno
            return strRetorno;
        }       
    }
}