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
        string FornecedorBuscarCodigo();

        [OperationContract]
        Contrato.RetornoFornecedor FornecedorListar(Contrato.EntradaFornecedor Fornecedor);

        [OperationContract]
        Contrato.RetornoFornecedor FornecedorSalvar(Contrato.EntradaFornecedor Fornecedor);

        [OperationContract]
        string ProdutoBuscarCodigo();

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
        string ClienteBuscarCodigo();

        [OperationContract]
        Contrato.RetornoCliente ClienteListar(Contrato.EntradaCliente Cliente);

        [OperationContract]
        Contrato.RetornoCliente ClienteSalvar(Contrato.EntradaCliente Cliente);

        [OperationContract]
        string OrcamentoBuscarCodigo();

        [OperationContract]
        Contrato.RetornoEstadoOrcamento EstadoOrcamentoListar(Contrato.EntradaEstadoOrcamento EstadoOrcamento);

        [OperationContract]
        Contrato.RetornoOrcamento OrcamentoListar(Contrato.EntradaOrcamento Orcamento);

        [OperationContract]
        Contrato.RetornoOrcamento OrcamentoSalvar(Contrato.EntradaOrcamento Orcamento);

        [OperationContract]
        Contrato.RetornoUnidadeFederativa UnidadeFederativaListar();
    }
}
