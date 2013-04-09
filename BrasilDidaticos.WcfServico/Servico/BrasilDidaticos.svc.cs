using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BrasilDidaticos.WcfServico.Servico
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class BrasilDidaticos : IBrasilDidaticos
    {
        public string TestarServico(string NomeHost)
        {
            // Loga no banco de dados
            Dados.BRASIL_DIDATICOS context = new Dados.BRASIL_DIDATICOS();
            context.T_USUARIO.ToList();
            return string.Format("Serviço ativo!\nHost: {0}.\nConexão: {1}\nServidor: {2}.\nEstado: {3}", NomeHost, context.Connection.ConnectionString, context.Connection.DataSource, context.Connection.State);
        }

        public Contrato.RetornoUsuario UsuarioLogar(Contrato.EntradaUsuario Usuario)
        {
            return Negocio.Usuario.Logar(Usuario);
        }

        public Contrato.RetornoUsuario UsuarioListar(Contrato.EntradaUsuario Usuario)
        {
            return Negocio.Usuario.ListarUsuario(Usuario);
        }

        public Contrato.RetornoUsuario UsuarioSalvar(Contrato.EntradaUsuario Usuario)
        {
            return Negocio.Usuario.SalvarUsuario(Usuario);
        }

        public Contrato.RetornoSessao SessaoExcluir(Contrato.Sessao Sessao)
        {
            return Negocio.Sessao.ExcluirSessao(Sessao);
        }

        public Contrato.RetornoSessao SessaoListar(Contrato.Sessao Sessao)
        {
            return Negocio.Sessao.ListarSessao(Sessao);
        }

        public Contrato.RetornoPerfil PerfilListar(Contrato.EntradaPerfil Perfil)
        {
            return Negocio.Perfil.ListarPerfil(Perfil);
        }

        public Contrato.RetornoPerfil PerfilSalvar(Contrato.EntradaPerfil Perfil)
        {
            return Negocio.Perfil.SalvarPerfil(Perfil);
        }

        public Contrato.RetornoPermissao PermissaoListar(Contrato.EntradaPermissao Permissao)
        {
            return Negocio.Permissao.ListarPermissao(Permissao);
        }

        public Contrato.RetornoTaxa TaxaListar(Contrato.EntradaTaxa Taxa)
        {
            return Negocio.Taxa.ListarTaxa(Taxa);
        }

        public Contrato.RetornoTaxa TaxaSalvar(Contrato.EntradaTaxa Taxa)
        {
            return Negocio.Taxa.SalvarTaxa(Taxa);
        }

        public string FornecedorBuscarCodigo()
        {
            return Negocio.Fornecedor.BuscarCodigoFornecedor();
        }

        public Contrato.RetornoFornecedor FornecedorListar(Contrato.EntradaFornecedor Fornecedor)
        {
            return Negocio.Fornecedor.ListarFornecedor(Fornecedor);
        }

        public Contrato.RetornoFornecedor FornecedorListar2(Contrato.EntradaFornecedor Fornecedor)
        {
            return Negocio.Fornecedor.ListarFornecedor2(Fornecedor);
        }

        public Contrato.RetornoFornecedor FornecedorSalvar(Contrato.EntradaFornecedor Fornecedor)
        {
            return Negocio.Fornecedor.SalvarFornecedor(Fornecedor);
        }

        public string ProdutoBuscarCodigo()
        {
            return Negocio.Produto.BuscarCodigoProduto();
        }

        public Contrato.RetornoProduto ProdutoListar(Contrato.EntradaProduto Produto)
        {
            return Negocio.Produto.ListarProduto(Produto);
        }

        public Contrato.RetornoProduto ProdutoListar2(Contrato.EntradaProduto Produto)
        {
            return Negocio.Produto.ListarProduto2(Produto);
        }

        public Contrato.RetornoProduto ProdutoListarRelatorio(Contrato.EntradaProduto Produto)
        {
            return Negocio.Produto.ListarProdutoRelatorio(Produto);
        }

        public Contrato.RetornoProduto ProdutoSalvar(Contrato.EntradaProduto Produto)
        {
            return Negocio.Produto.SalvarProduto(Produto);
        }

        public Contrato.RetornoProduto ProdutosSalvar(Contrato.EntradaProdutos Produtos)
        {
            return Negocio.Produto.SalvarProdutos(Produtos);
        }

        public string ClienteBuscarCodigo()
        {
            return Negocio.Cliente.BuscarCodigoCliente();
        }

        public Contrato.RetornoCliente ClienteListar(Contrato.EntradaCliente Cliente)
        {
            return Negocio.Cliente.ListarCliente(Cliente);
        }

        public Contrato.RetornoCliente ClienteSalvar(Contrato.EntradaCliente Cliente)
        {
            return Negocio.Cliente.SalvarCliente(Cliente);
        }

        public Contrato.RetornoParametro ParametroListar(Contrato.EntradaParametro Parametro)
        {
            return Negocio.Parametro.ListarParametro(Parametro);
        }

        public Contrato.RetornoParametro ParametrosSalvar(Contrato.EntradaParametros Parametros)
        {
            return Negocio.Parametro.SalvarParametros(Parametros);
        }

        public string OrcamentoBuscarCodigo()
        {
            return Negocio.Orcamento.BuscarCodigoOrcamento();
        }

        public Contrato.RetornoOrcamento OrcamentoListar(Contrato.EntradaOrcamento Orcamento)
        {
            return Negocio.Orcamento.ListarOrcamento(Orcamento);
        }

        public Contrato.RetornoOrcamento OrcamentoSalvar(Contrato.EntradaOrcamento Orcamento)
        {
            return Negocio.Orcamento.SalvarOrcamento(Orcamento);
        }

        public Contrato.RetornoEstadoOrcamento EstadoOrcamentoListar(Contrato.EntradaEstadoOrcamento EstadoOrcamento)
        {
            return Negocio.EstadoOrcamento.ListarEstadoOrcamento(EstadoOrcamento);
        }

        public Contrato.RetornoUnidadeFederativa UnidadeFederativaListar()
        {
            return Negocio.UnidadeFederativa.ListarUnidadeFederativa();
        }
    }
}
