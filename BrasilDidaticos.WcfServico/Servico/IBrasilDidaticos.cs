using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BrasilDidaticos.WcfServico.Servico
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    internal interface IBrasilDidaticos
    {
        [OperationContract]
        string TestarServico(string NomeHost);

        [OperationContract]
        Contrato.RetornoUsuario UsuarioLogar(Contrato.EntradaUsuario Usuario);

        [OperationContract]
        Contrato.RetornoUsuario UsuarioListar(Contrato.EntradaUsuario Usuario);

        [OperationContract]
        Contrato.RetornoUsuario UsuarioSalvar(Contrato.EntradaUsuario Usuario);

        [OperationContract]
        Contrato.RetornoSessao SessaoExcluir(Contrato.Sessao Sessao);

        [OperationContract]
        Contrato.RetornoSessao SessaoListar(Contrato.Sessao Sessao);

        [OperationContract]
        Contrato.RetornoPerfil PerfilListar(Contrato.EntradaPerfil Perfil);

        [OperationContract]
        Contrato.RetornoPerfil PerfilSalvar(Contrato.EntradaPerfil Perfil);

        [OperationContract]
        Contrato.RetornoPermissao PermissaoListar(Contrato.EntradaPermissao Permissao);

        [OperationContract]
        Contrato.RetornoTaxa TaxaListar(Contrato.EntradaTaxa Taxa);

        [OperationContract]
        Contrato.RetornoTaxa TaxaSalvar(Contrato.EntradaTaxa Taxa);

        [OperationContract]
        string FornecedorBuscarCodigo(Guid IdEmpresa);

        [OperationContract]
        Contrato.RetornoFornecedor FornecedorListar(Contrato.EntradaFornecedor Fornecedor);
        
        [OperationContract]
        Contrato.RetornoFornecedor FornecedorSalvar(Contrato.EntradaFornecedor Fornecedor);

        [OperationContract]
        string ProdutoBuscarCodigo(Guid IdEmpresa);

        [OperationContract]
        Contrato.RetornoProduto ProdutoListar(Contrato.EntradaProduto Produto);

        [OperationContract]
        Contrato.RetornoProduto ProdutoListarRelatorio(Contrato.EntradaProduto Produto);

        [OperationContract]
        Contrato.RetornoProduto ProdutoSalvar(Contrato.EntradaProduto Produto);

        [OperationContract]
        Contrato.RetornoProduto ProdutosSalvar(Contrato.EntradaProdutos Produtos);

        [OperationContract]
        Contrato.RetornoParametro ParametroListar(Contrato.EntradaParametro Parametro);

        [OperationContract]
        Contrato.RetornoParametro ParametrosSalvar(Contrato.EntradaParametros Parametros);

        [OperationContract]
        string ClienteBuscarCodigo(Guid IdEmpresa);

        [OperationContract]
        Contrato.RetornoCliente ClienteListar(Contrato.EntradaCliente Cliente);

        [OperationContract]
        Contrato.RetornoCliente ClienteSalvar(Contrato.EntradaCliente Cliente);

        [OperationContract]
        string OrcamentoBuscarCodigo(Guid IdEmpresa);

        [OperationContract]
        Contrato.RetornoEstadoOrcamento EstadoOrcamentoListar(Contrato.EntradaEstadoOrcamento EstadoOrcamento);

        [OperationContract]
        Contrato.RetornoOrcamento OrcamentoListar(Contrato.EntradaOrcamento Orcamento);

        [OperationContract]
        Contrato.RetornoOrcamento OrcamentoSalvar(Contrato.EntradaOrcamento Orcamento);

        [OperationContract]
        Contrato.RetornoUnidadeMedida UnidadeMedidaListar(Contrato.EntradaUnidadeMedida UnidadeMedida);

        [OperationContract]
        Contrato.RetornoUnidadeMedida UnidadeMedidaSalvar(Contrato.EntradaUnidadeMedida UnidadeMedida);

        [OperationContract]
        string PedidoBuscarCodigo(Guid IdEmpresa);

        [OperationContract]
        Contrato.RetornoPedido PedidoListar(Contrato.EntradaPedido Pedido);

        [OperationContract]
        Contrato.RetornoPedido PedidoSalvar(Contrato.EntradaPedido Pedido);

        [OperationContract]
        Contrato.RetornoEstadoPedido EstadoPedidoListar(Contrato.EntradaEstadoPedido EstadoPedido);

        [OperationContract]
        Contrato.RetornoEmpresa EmpresaListar(Contrato.EntradaEmpresa Empresa);

        [OperationContract]
        Contrato.RetornoUnidadeFederativa UnidadeFederativaListar();
    }
}
