using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrasilDidaticos.WcfServico;

namespace BrasilDidaticos.WcfServico.Negocio
{
    static class Cliente
    {
        /// <summary>
        /// Método para buscar o código do cliente
        /// </summary>        
        /// <returns>string</returns>
        internal static string BuscarCodigoCliente()
        {
            // Objeto que recebe o retorno do método
            string retCodigoCliente = string.Empty;

            // Loga no banco de dados
            Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();
            System.Data.Objects.ObjectParameter objCodigoOrcamento = new System.Data.Objects.ObjectParameter("P_CODIGO", typeof(global::System.Int32));
            context.RETORNAR_CODIGO(Contrato.Constantes.TIPO_COD_CLIENTE, objCodigoOrcamento);

            // Recupera o código do fornecedor
            retCodigoCliente = Util.RecuperaCodigo((int)objCodigoOrcamento.Value, Contrato.Constantes.TIPO_COD_CLIENTE);

            // retorna os dados
            return retCodigoCliente;
        }

        /// <summary>
        /// Método para buscar o cliente
        /// </summary>
        /// <param name="Cliente">Objeto com o identificador do cliente</param>
        /// <returns>Contrato.Cliente</returns>
        internal static Contrato.Cliente BuscarCliente(Dados.CLIENTE cliente)
        {
            // Objeto que recebe o retorno do método
            Contrato.Cliente retCliente = new Contrato.Cliente();
            
            // Verifica se foi encontrado algum registro
            if (cliente != null)
            {
                retCliente = new Contrato.Cliente()
                {
                    Id = cliente.ID_CLIENTE,
                    Nome = cliente.NOME_CLIENTE,
                    Codigo = cliente.COD_CLIENTE,
                    Ativo = cliente.BOL_ATIVO,
                    Cpf_Cnpj = cliente.CPF_CNJP_CLIENTE,
                    Tipo = cliente.BOL_PESSOA_FISICA ? Contrato.Enumeradores.Pessoa.Fisica : Contrato.Enumeradores.Pessoa.Juridica,
                    Email = cliente.DES_EMAIL,
                    Telefone = cliente.NUM_TELEFONE,
                    Celular = cliente.NUM_CELULAR,
                    InscricaoEstadual = cliente.DES_INSCRICAO_ESTADUAL,
                    Endereco = cliente.DES_ENDERECO,
                    Numero = cliente.NUM_ENDERECO,
                    Complemento = cliente.CMP_ENDERECO,
                    Cep = cliente.NUM_CEP,
                    Bairro = cliente.DES_BAIRRO,
                    Cidade = cliente.DES_CIDADE,
                    Uf = new Contrato.UnidadeFederativa() { Codigo = cliente.COD_ESTADO, Nome = cliente.DES_ESTADO },
                    ClienteMatriz = Negocio.Cliente.BuscarCliente(cliente.T_CLIENTE_MATRIZ)
                };
            }

            // retorna os dados
            return retCliente;
        }

        /// <summary>
        /// Método para listar os clientees
        /// </summary>
        /// <param name="Cliente">Objeto com os dados do filtro</param>
        /// <returns>Contrato.RetornoCliente</returns>
        internal static Contrato.RetornoCliente ListarCliente(Contrato.EntradaCliente entradaCliente)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoCliente retCliente = new Contrato.RetornoCliente();
            
            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaCliente.UsuarioLogado, Chave = entradaCliente.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Loga no banco de dados
                Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();
                                                
                // Busca o cliente no banco
                List<Dados.CLIENTE> lstClientes = (from c in context.T_CLIENTE
                                                          where
                                                                (entradaCliente.Cliente.Codigo == null || entradaCliente.Cliente.Codigo == string.Empty || c.COD_CLIENTE.ToLower().Contains(entradaCliente.Cliente.Codigo.ToLower()))
                                                             && (entradaCliente.Cliente.Nome == null || entradaCliente.Cliente.Nome == string.Empty || c.NOME_CLIENTE.ToLower().Contains(entradaCliente.Cliente.Nome.ToLower()))
                                                             && (entradaCliente.Cliente.CaixaEscolar == null || entradaCliente.Cliente.CaixaEscolar == string.Empty || c.CAIXA_ESCOLAR.ToLower().Contains(entradaCliente.Cliente.CaixaEscolar.ToLower()))
                                                             && (entradaCliente.Cliente.Tipo == null || c.BOL_PESSOA_FISICA == (entradaCliente.Cliente.Tipo == Contrato.Enumeradores.Pessoa.Fisica ? true : false))
                                                             && (entradaCliente.Cliente.Cpf_Cnpj == null || entradaCliente.Cliente.Cpf_Cnpj == string.Empty || c.CPF_CNJP_CLIENTE != null && c.CPF_CNJP_CLIENTE.StartsWith(entradaCliente.Cliente.Cpf_Cnpj))
                                                          select c).ToList();
                               
                // Verifica se foi encontrado algum registro
                if (lstClientes.Count > 0)
                {
                    // Preenche o objeto de retorno
                    retCliente.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    retCliente.Clientes = new List<Contrato.Cliente>();
                    foreach (Dados.CLIENTE cliente in lstClientes)
                    {          
                        retCliente.Clientes.Add( new Contrato.Cliente()
                        {
                            Id = cliente.ID_CLIENTE,                            
                            Codigo = cliente.COD_CLIENTE,
                            Nome = cliente.NOME_CLIENTE,
                            Ativo = cliente.BOL_ATIVO,
                            CaixaEscolar = cliente.CAIXA_ESCOLAR,
                            Cpf_Cnpj = cliente.CPF_CNJP_CLIENTE,
                            Tipo = cliente.BOL_PESSOA_FISICA ? Contrato.Enumeradores.Pessoa.Fisica : Contrato.Enumeradores.Pessoa.Juridica,
                            Email = cliente.DES_EMAIL,
                            Telefone = cliente.NUM_TELEFONE,
                            Celular = cliente.NUM_CELULAR,
                            InscricaoEstadual = cliente.DES_INSCRICAO_ESTADUAL,
                            Endereco = cliente.DES_ENDERECO,
                            Numero = cliente.NUM_ENDERECO,
                            Complemento = cliente.CMP_ENDERECO,
                            Cep = cliente.NUM_CEP,
                            Bairro = cliente.DES_BAIRRO,
                            Cidade = cliente.DES_CIDADE,
                            Uf = Negocio.UnidadeFederativa.BuscarUnidadeFederativa(cliente.COD_ESTADO),
                            ClienteMatriz = Negocio.Cliente.BuscarCliente(cliente.T_CLIENTE_MATRIZ)
                        });
                    };
                }
                else
                {
                    // Preenche o objeto de retorno
                    retCliente.Codigo = Contrato.Constantes.COD_RETORNO_VAZIO;
                    retCliente.Mensagem = "Não existe dados para o filtro informado.";
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retCliente.Codigo = retSessao.Codigo;
                retCliente.Mensagem = retSessao.Mensagem;
            }
            
            // retorna os dados
            return retCliente;
        }

        /// <summary>
        /// Método para salvar o cliente
        /// </summary>
        /// <param name="entradaCliente">Objeto com os dados do cliente</param>
        /// <returns>Contrato.RetornoCliente</returns>
        internal static Contrato.RetornoCliente SalvarCliente(Contrato.EntradaCliente entradaCliente)
        {
            // Objeto que recebe o retorno do método
            Contrato.RetornoCliente retCliente = new Contrato.RetornoCliente();

            // Objeto que recebe o retorno da sessão
            Contrato.RetornoSessao retSessao = Negocio.Sessao.ValidarSessao(new Contrato.Sessao() { Login = entradaCliente.UsuarioLogado, Chave = entradaCliente.Chave });
            
            // Verifica se o usuário está autenticado
            if (retSessao.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Verifica se as informações do cliente foram informadas
                string strValidacao = ValidarClientePreenchido(entradaCliente.Cliente);
            
                // Se existe algum erro
                if (strValidacao.Length > 0)
                {
                    retCliente.Codigo = Contrato.Constantes.COD_FILTRO_VAZIO;
                    retCliente.Mensagem = strValidacao;
                }
                else
                {
                    // Loga no banco de dados
                    Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();

                    // Busca o cliente no banco
                    List<Dados.CLIENTE> lstClientes = (from f in context.T_CLIENTE
                                                              where (f.COD_CLIENTE == entradaCliente.Cliente.Codigo &&
                                                                    (entradaCliente.Cliente.Cpf_Cnpj != null || f.CPF_CNJP_CLIENTE == entradaCliente.Cliente.Cpf_Cnpj))
                                                                 || (entradaCliente.Novo == null && entradaCliente.Cliente.Id == f.ID_CLIENTE)                                                              
                                                              select f).ToList();

                    // Verifica se foi encontrado algum registro
                    if (lstClientes.Count > 0 && entradaCliente.Novo != null && (bool)entradaCliente.Novo)
                    {
                        // Preenche o objeto de retorno
                        retCliente.Codigo = Contrato.Constantes.COD_REGISTRO_DUPLICADO;
                        retCliente.Mensagem = string.Format("O cliente de código '{0}' já existe!", lstClientes.First().COD_CLIENTE);
                    }
                    else
                    {
                        // Se existe o cliente
                        if (lstClientes.Count > 0)
                        {
                            // Atualiza o cliente
                            lstClientes.First().NOME_CLIENTE = entradaCliente.Cliente.Nome;
                            lstClientes.First().CAIXA_ESCOLAR = entradaCliente.Cliente.CaixaEscolar;
                            lstClientes.First().BOL_ATIVO = entradaCliente.Cliente.Ativo;
                            lstClientes.First().BOL_PESSOA_FISICA = entradaCliente.Cliente.Tipo == Contrato.Enumeradores.Pessoa.Fisica ? true : false;
                            lstClientes.First().CPF_CNJP_CLIENTE = entradaCliente.Cliente.Cpf_Cnpj;
                            lstClientes.First().DES_EMAIL = entradaCliente.Cliente.Email;
                            lstClientes.First().DES_INSCRICAO_ESTADUAL = entradaCliente.Cliente.InscricaoEstadual;
                            lstClientes.First().NUM_TELEFONE = entradaCliente.Cliente.Telefone;
                            lstClientes.First().NUM_CELULAR = entradaCliente.Cliente.Celular;
                            lstClientes.First().DES_ENDERECO = entradaCliente.Cliente.Endereco;
                            lstClientes.First().NUM_ENDERECO = entradaCliente.Cliente.Numero;
                            lstClientes.First().CMP_ENDERECO = entradaCliente.Cliente.Complemento;
                            lstClientes.First().NUM_CEP = entradaCliente.Cliente.Cep;
                            lstClientes.First().DES_BAIRRO = entradaCliente.Cliente.Bairro;
                            lstClientes.First().DES_CIDADE = entradaCliente.Cliente.Cidade;
                            lstClientes.First().COD_ESTADO = entradaCliente.Cliente.Uf == null ? null : entradaCliente.Cliente.Uf.Codigo;
                            lstClientes.First().DES_ESTADO = entradaCliente.Cliente.Uf == null ? null : entradaCliente.Cliente.Uf.Nome;
                            lstClientes.First().DATA_ATUALIZACAO = DateTime.Now;
                            lstClientes.First().LOGIN_USUARIO = entradaCliente.UsuarioLogado;
                            lstClientes.First().ID_CLIENTE_MATRIZ = entradaCliente.Cliente.ClienteMatriz != null ? entradaCliente.Cliente.ClienteMatriz.Id: Guid.Empty;
                        }
                        else
                        {
                            // Recupera o código do cliente
                            string codigoCliente = string.Empty;
                            if (entradaCliente.Cliente.Codigo != string.Empty)
                                codigoCliente = entradaCliente.Cliente.Codigo;
                            else
                            {
                                System.Data.Objects.ObjectParameter objCodigoOrcamento = new System.Data.Objects.ObjectParameter("P_CODIGO", typeof(global::System.Int32));
                                context.RETORNAR_CODIGO(Contrato.Constantes.TIPO_COD_CLIENTE, objCodigoOrcamento);
                                codigoCliente = Util.RecuperaCodigo((int)objCodigoOrcamento.Value, Contrato.Constantes.TIPO_COD_CLIENTE);
                            }

                            // Cria o cliente
                            Dados.CLIENTE tCliente = new Dados.CLIENTE();
                            tCliente.ID_CLIENTE = Guid.NewGuid();
                            tCliente.COD_CLIENTE = codigoCliente;
                            tCliente.NOME_CLIENTE = entradaCliente.Cliente.Nome;
                            tCliente.BOL_ATIVO = entradaCliente.Cliente.Ativo;
                            tCliente.CAIXA_ESCOLAR = entradaCliente.Cliente.CaixaEscolar;
                            tCliente.BOL_PESSOA_FISICA = entradaCliente.Cliente.Tipo == Contrato.Enumeradores.Pessoa.Fisica ? true : false;
                            tCliente.CPF_CNJP_CLIENTE = entradaCliente.Cliente.Cpf_Cnpj;
                            tCliente.DES_EMAIL = entradaCliente.Cliente.Email;
                            tCliente.NUM_TELEFONE = entradaCliente.Cliente.Telefone;
                            tCliente.NUM_CELULAR = entradaCliente.Cliente.Celular;
                            tCliente.DES_INSCRICAO_ESTADUAL = entradaCliente.Cliente.InscricaoEstadual;
                            tCliente.DES_ENDERECO = entradaCliente.Cliente.Endereco;
                            tCliente.NUM_ENDERECO = entradaCliente.Cliente.Numero;
                            tCliente.CMP_ENDERECO = entradaCliente.Cliente.Complemento;
                            tCliente.NUM_CEP = entradaCliente.Cliente.Cep;
                            tCliente.DES_BAIRRO = entradaCliente.Cliente.Bairro;
                            tCliente.DES_CIDADE = entradaCliente.Cliente.Cidade;
                            tCliente.COD_ESTADO = entradaCliente.Cliente.Uf == null ? null : entradaCliente.Cliente.Uf.Codigo;
                            tCliente.DES_ESTADO = entradaCliente.Cliente.Uf == null ? null : entradaCliente.Cliente.Uf.Nome;                      
                            tCliente.DATA_ATUALIZACAO = DateTime.Now;
                            tCliente.LOGIN_USUARIO = entradaCliente.UsuarioLogado;
                            tCliente.ID_CLIENTE_MATRIZ = entradaCliente.Cliente.ClienteMatriz != null ? entradaCliente.Cliente.ClienteMatriz.Id : Guid.Empty;

                            // Adiciona o cliente na tabela
                            context.AddToT_CLIENTE(tCliente);
                        }

                        // Salva as alterações
                        context.SaveChanges();

                        // Preenche o objeto de retorno
                        retCliente.Codigo = Contrato.Constantes.COD_RETORNO_SUCESSO;
                    }
                }
            }
            else
            {
                // retorna quando o usuário não está autenticado
                retCliente.Codigo = retSessao.Codigo;
                retCliente.Mensagem = retSessao.Mensagem;
            }

            // retorna dos dados 
            return retCliente;
        }

        /// <summary>
        /// Método para verificar se as informações do cliente foram preenchidas
        /// </summary>
        /// <param name="Usuario">Objeto com o dados do cliente</param>
        /// <returns></returns>
        private static string ValidarClientePreenchido(Contrato.Cliente Cliente)
        {
            // Cria a variável de retorno
            string strRetorno = string.Empty;
                        
            // Verifica se a Nome foi preenchida
            if (string.IsNullOrWhiteSpace(Cliente.Nome))
                strRetorno += "O campo 'Nome' não foi informado!\n";

            // retorna a variável de retorno
            return strRetorno;

        }       
    }
}
