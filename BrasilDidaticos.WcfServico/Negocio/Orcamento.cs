﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrasilDidaticos.WcfServico;

namespace BrasilDidaticos.WcfServico.Negocio
{
    static class Orcamento
    {

        /// <summary>
        /// Método para buscar o orçamento
        /// </summary>
        /// <param name="Orcamento">Objeto com o identificador do orçamento</param>
        /// <returns>Contrato.RetornoOrcamento</returns>
        public static Contrato.Orcamento BuscarOrcamento(Dados.ORCAMENTO orcamento)
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
        public static Contrato.Orcamento BuscarOrcamento(Dados.ORCAMENTO orcamento, bool carregarItens)
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
        public static Contrato.RetornoOrcamento ListarOrcamento(Contrato.EntradaOrcamento entradaOrcamento)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoOrcamento retOrcamento = new Contrato.RetornoOrcamento();
            
            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaOrcamento.UsuarioLogado, Chave = entradaOrcamento.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
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

                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();
                                                
                // Busca o orcamento no banco
                List<Dados.ORCAMENTO> lstOrcamentos = null;
                    
                if (entradaOrcamento.Paginar)
                {
                    lstOrcamentos = (from o in context.T_ORCAMENTO
                                    where 
                                        (entradaOrcamento.Orcamento.Codigo == string.Empty || o.COD_ORCAMENTO.StartsWith(entradaOrcamento.Orcamento.Codigo))                                    
                                    &&  (entradaOrcamento.Orcamento.Cliente.Id == Guid.Empty || o.ID_CLIENTE == entradaOrcamento.Orcamento.Cliente.Id)
                                    select o                                                           
                                    ).OrderBy(o => o.DATA_ORCAMENTO).Skip(entradaOrcamento.PosicaoUltimoItem).Take(entradaOrcamento.CantidadeItens).ToList();
                }
                else
                {
                    lstOrcamentos = (from o in context.T_ORCAMENTO
                                   where
                                        (entradaOrcamento.Orcamento.Codigo == string.Empty || o.COD_ORCAMENTO.StartsWith(entradaOrcamento.Orcamento.Codigo))                                   
                                   &&   (entradaOrcamento.Orcamento.Cliente.Id == Guid.Empty || o.ID_CLIENTE == entradaOrcamento.Orcamento.Cliente.Id)
                                   select o
                                    ).ToList();
                }
                                
                // Verifica se foi encontrado algum registro
                if (lstOrcamentos.Count > 0)
                {
                    // Preenche o objeto de retorno
                    retOrcamento.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    retOrcamento.Orcamentos = new List<Contrato.Orcamento>();
                    foreach (Dados.ORCAMENTO orcamento in lstOrcamentos)
                    {          
                        retOrcamento.Orcamentos.Add( new Contrato.Orcamento()
                        {
                            Id = orcamento.ID_ORCAMENTO,                            
                            Codigo = orcamento.COD_ORCAMENTO,
                            Data = orcamento.DATA_ORCAMENTO,                            
                            ValorDesconto = orcamento.NUM_DESCONTO,
                            Estado = Negocio.EstadoOrcamento.BuscarOrcamentoEstadoOrcamento(orcamento.T_ESTADO_ORCAMENTO),
                            Cliente = Negocio.Cliente.BuscarCliente(orcamento.T_CLIENTE),
                            Responsavel = Negocio.Usuario.BuscarUsuario(orcamento.T_USUARIO_RESPOSANVEL),
                            Vendedor = Negocio.Usuario.BuscarUsuario(orcamento.T_USUARIO_VENDEDOR),
                            Itens = Negocio.Item.ListarOrcamentoItem(orcamento.T_ITEM)
                        });
                    };
                }
                else
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
        public static Contrato.RetornoOrcamento SalvarOrcamento(Contrato.EntradaOrcamento entradaOrcamento)
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
                                                          || (entradaOrcamento.Novo == null && entradaOrcamento.Orcamento.Id == p.ID_ORCAMENTO))
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
                            lstOrcamentos.First().DATA_ATUALIZACAO = DateTime.Now;
                            lstOrcamentos.First().LOGIN_USUARIO = entradaOrcamento.UsuarioLogado;

                            // Apaga todos as taxas que estão relacionados
                            while (lstOrcamentos.First().T_ITEM.Count > 0)
                            {
                                context.T_ITEM.DeleteObject(lstOrcamentos.First().T_ITEM.First());
                            }

                            // Verifica se existe alguma taxa  associado ao usuário
                            if (entradaOrcamento.Orcamento.Itens != null)
                            {
                                // Para cada perfil associado
                                foreach (Contrato.Item item in entradaOrcamento.Orcamento.Itens)
                                {
                                    // Associa a taxa ao fornecedor
                                    lstOrcamentos.First().T_ITEM.Add(new Dados.ITEM()
                                    {
                                        ID_ITEM = Guid.NewGuid(),
                                        DES_ITEM = item.Descricao,
                                        ID_PRODUTO = item.Produto.Id,
                                        ID_ORCAMENTO = entradaOrcamento.Orcamento.Id,
                                        NUM_QUANTIDADE = item.Quantidade,
                                        NUM_VALOR_CUSTO = item.ValorCusto,
                                        NUM_VALOR_UNITARIO = item.ValorUnitario,
                                        NUM_DESCONTO = item.ValorDesconto,
                                        LOGIN_USUARIO = entradaOrcamento.UsuarioLogado,
                                        DATA_ATUALIZACAO = DateTime.Now
                                    });
                                }
                            }
                        }
                        else
                        {
                            // Cria o orcamento
                            Dados.ORCAMENTO tOrcamento = new Dados.ORCAMENTO();
                            tOrcamento.ID_ORCAMENTO = Guid.NewGuid();
                            tOrcamento.COD_ORCAMENTO = entradaOrcamento.Orcamento.Codigo;
                            tOrcamento.DATA_ORCAMENTO = entradaOrcamento.Orcamento.Data;
                            tOrcamento.ID_CLIENTE = entradaOrcamento.Orcamento.Cliente.Id;
                            tOrcamento.ID_ESTADO_ORCAMENTO = entradaOrcamento.Orcamento.Estado.Id;
                            tOrcamento.ID_USUARIO_VENDEDOR = entradaOrcamento.Orcamento.Vendedor.Id;
                            tOrcamento.ID_USUARIO_RESPONSAVEL = entradaOrcamento.Orcamento.Responsavel.Id;
                            tOrcamento.NUM_DESCONTO = entradaOrcamento.Orcamento.ValorDesconto;
                            tOrcamento.DATA_ATUALIZACAO = DateTime.Now;
                            tOrcamento.LOGIN_USUARIO = entradaOrcamento.UsuarioLogado;                                                       

                            // Verifica se existe alguma taxa  associado ao usuário
                            if (entradaOrcamento.Orcamento.Itens != null)
                            {
                                // Para cada perfil associado
                                foreach (Contrato.Item item in entradaOrcamento.Orcamento.Itens)
                                {
                                    // Associa a taxa ao fornecedor
                                    tOrcamento.T_ITEM.Add(new Dados.ITEM()
                                    {
                                        ID_ITEM = Guid.NewGuid(),
                                        DES_ITEM = item.Descricao,
                                        ID_PRODUTO = item.Produto.Id,
                                        ID_ORCAMENTO = entradaOrcamento.Orcamento.Id,
                                        NUM_QUANTIDADE = item.Quantidade,
                                        NUM_VALOR_CUSTO = item.ValorCusto,
                                        NUM_VALOR_UNITARIO = item.ValorUnitario,
                                        NUM_DESCONTO = item.ValorDesconto,
                                        LOGIN_USUARIO = entradaOrcamento.UsuarioLogado,
                                        DATA_ATUALIZACAO = DateTime.Now
                                    });
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

            // Verifica se o Codigo foi preenchido
            if (string.IsNullOrWhiteSpace(Orcamento.Codigo))
                strRetorno = "O campo 'Codigo' não foi informado!\n";

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