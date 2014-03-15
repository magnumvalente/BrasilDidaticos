using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrasilDidaticos.WcfServico;

namespace BrasilDidaticos.WcfServico.Negocio
{
    static class EstadoOrcamento
    {
        /// <summary>
        /// Método para listar os permissões
        /// </summary>
        /// <param name="entradaEstadoOrcamento.Permissoes">Objeto com os dados do filtro</param>
        /// <returns>Contrato.RetornoEstadoOrcamento</returns>
        internal static Contrato.RetornoEstadoOrcamento ListarEstadoOrcamento(Contrato.EntradaEstadoOrcamento entradaEstadoOrcamento)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoEstadoOrcamento retEstadoOrcamento = new Contrato.RetornoEstadoOrcamento();
             
            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaEstadoOrcamento.UsuarioLogado, Chave = entradaEstadoOrcamento.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {

                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();
                context.ContextOptions.LazyLoadingEnabled = true;

                // Busca o estadoOrcamento no banco
                List<Dados.ESTADO_ORCAMENTO> lstPermissoes = (from f in context.T_ESTADO_ORCAMENTO
                                                              where (entradaEstadoOrcamento.EstadoOrcamento.Ativo == null || f.BOL_ATIVO == entradaEstadoOrcamento.EstadoOrcamento.Ativo)
                                                select f).ToList();

                // Verifica se foi encontrado algum registro
                if (lstPermissoes.Count > 0)
                {
                    // Preenche o objeto de retorno
                    retEstadoOrcamento.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    retEstadoOrcamento.EstadosOrcamento = new List<Contrato.EstadoOrcamento>();
                    foreach (Dados.ESTADO_ORCAMENTO estadoOrcamento in lstPermissoes)
                    {
                        retEstadoOrcamento.EstadosOrcamento.Add(new Contrato.EstadoOrcamento()
                        {
                            Id = estadoOrcamento.ID_ESTADO_ORCAMENTO,
                            Codigo = estadoOrcamento.COD_ESTADO_ORCAMENTO,
                            Nome = estadoOrcamento.NOME_ESTADO_ORCAMENTO,
                            Ativo = estadoOrcamento.BOL_ATIVO,
                            Anterior = Negocio.EstadoOrcamento.BuscarOrcamentoEstadoOrcamento(estadoOrcamento.T_ESTADO_ORCAMENTO_ANTERIOR),
                            Sucessor = Negocio.EstadoOrcamento.BuscarOrcamentoEstadoOrcamento(estadoOrcamento.T_ESTADO_ORCAMENTO_SUCESSOR)
                        });
                    };
                }
                else
                {
                    // Preenche o objeto de retorno
                    retEstadoOrcamento.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;
                    retEstadoOrcamento.Mensagem = "Não existe dados para o filtro informado.";
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retEstadoOrcamento.Codigo = retSessao.Codigo;
                retEstadoOrcamento.Mensagem = retSessao.Mensagem;
            }
            
            // retorna os dados
            return retEstadoOrcamento;
        }

        /// <summary>
        /// Retorna o estado do orçamento
        /// </summary>
        /// <param name="estadoOrcamento">Recebe o estado do orcamento recuperado do banco</param>
        /// <returns>Contrato.EstadoOrcamento</returns>
        internal static Contrato.EstadoOrcamento BuscarOrcamentoEstadoOrcamento(Dados.ESTADO_ORCAMENTO estadoOrcamento)
        {
            // Objeto que recebe o retorno do método
            Contrato.EstadoOrcamento retEstadoOrcamento = new Contrato.EstadoOrcamento();

            // Verifica se foi encontrado algum registro
            if (estadoOrcamento != null)
            {
                retEstadoOrcamento = new Contrato.EstadoOrcamento()
                {
                    Id = estadoOrcamento.ID_ESTADO_ORCAMENTO,
                    Codigo = estadoOrcamento.COD_ESTADO_ORCAMENTO,
                    Nome = estadoOrcamento.NOME_ESTADO_ORCAMENTO,
                    Ativo = estadoOrcamento.BOL_ATIVO
                };
            }

            // retorna os dados
            return retEstadoOrcamento;
        }
    }
}
