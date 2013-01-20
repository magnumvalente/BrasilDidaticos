using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BrasilDidaticos.Apresentacao
{
    /// <summary>
    /// Interaction logic for WRelatorioVarejo.xaml
    /// </summary>
    public partial class WRelatorioVarejo : Window
    {
        #region "[Atributos]"

        private bool _isReportViewerLoaded;

        #endregion

        #region "[Metodos]"

        public WRelatorioVarejo()
        {
            InitializeComponent();
            _reportViewer.Load += ReportViewer_Load;
        }

        private void ValidarPermissao()
        {
            // Permissão botões da tela         
        }

        private void ListarProdutos()
        {
            ListarProdutos(false);
        }

        private void ListarProdutos(bool mostrarMsgVazio)
        {
            Contrato.EntradaProduto entradaProduto = new Contrato.EntradaProduto();            
            entradaProduto.Chave = Comum.Util.Chave;
            entradaProduto.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaProduto.Produto = new Contrato.Produto();

            Contrato.EntradaParametro entradaParametro = new Contrato.EntradaParametro();
            entradaParametro.Chave = Comum.Util.Chave;
            entradaParametro.UsuarioLogado = Comum.Util.UsuarioLogado.Login;

            PreencherFiltro(entradaProduto.Produto);

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
            Contrato.RetornoProduto retProduto = servBrasilDidaticos.ProdutoListar(entradaProduto);            
            Contrato.RetornoParametro retParametro = servBrasilDidaticos.ParametroListar(entradaParametro);
            servBrasilDidaticos.Close();
            
            List<Objeto.Produto> lstProdutos = null;            
            this._reportViewer.LocalReport.DataSources.Clear();
            
            if (retProduto.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                lstProdutos = (from p in retProduto.Produtos select new Objeto.Produto { Codigo = p.Codigo, Nome = p.Nome, Fornecedor = p.Fornecedor, Taxas = p.Taxas, ValorBase = p.ValorBase }).ToList();
                this._reportViewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("Produto", lstProdutos));
                this._reportViewer.RefreshReport();
            }

            if (mostrarMsgVazio && retProduto.Codigo == Contrato.Constantes.COD_RETORNO_VAZIO)
                MessageBox.Show(retProduto.Mensagem, "Relatório Varejo", MessageBoxButton.OK, MessageBoxImage.Information);                              
        }

        private void PreencherFornecedores()
        {
            Contrato.EntradaFornecedor entradaFornecedor = new Contrato.EntradaFornecedor();
            entradaFornecedor.Chave = Comum.Util.Chave;
            entradaFornecedor.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaFornecedor.Fornecedor = new Contrato.Fornecedor() { Ativo = true };

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
            Contrato.RetornoFornecedor retFornecedor = servBrasilDidaticos.FornecedorListar(entradaFornecedor);
            servBrasilDidaticos.Close();

            if (retFornecedor.Fornecedores != null)
            {
                cmbFornecedor.ComboBox.Items.Add(new ComboBoxItem() { Uid = Guid.Empty.ToString(), Content = "Todos" });
                foreach (Contrato.Fornecedor fornecedor in retFornecedor.Fornecedores)
                {
                    cmbFornecedor.ComboBox.Items.Add(new ComboBoxItem() { Uid = fornecedor.Id.ToString(), Content = fornecedor.Nome, Tag = fornecedor });
                }
            }
        }

        private void PreencherFiltro(Contrato.Produto Produto)
        {
            Produto.Codigo = txtCodigo.Conteudo;
            Produto.Nome = txtNome.Conteudo;
            if (cmbFornecedor.ValorSelecionado != null)
                Produto.Fornecedor = (Contrato.Fornecedor)cmbFornecedor.ValorSelecionado;
            Produto.Ativo = (bool)chkAtivo.Selecionado;
        } 

        #endregion

        #region "[Eventos]"

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                ValidarPermissao();
                PreencherFornecedores();
                txtCodigo.txtBox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Relatório Varejo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }   
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                ListarProdutos(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Relatório Varejo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }            
        }

        private void ReportViewer_Load(object sender, EventArgs e)
        {
            if (!_isReportViewerLoaded)
            {                
                this._reportViewer.LocalReport.ReportPath = "Relatorio/RelatorioVarejo.rdlc";                
                this._isReportViewerLoaded = true;
            }
        }    
        
        #endregion                               
    }
}
