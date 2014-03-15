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
    /// Interaction logic for WRelatorioAtacado.xaml
    /// </summary>
    public partial class WRelatorioAtacado : Window
    {
        #region "[Atributos]"

        private bool _isReportViewerLoaded;

        #endregion

        #region "[Propriedades]"

        public List<Contrato.Produto> Produtos;

        #endregion

        #region "[Metodos]"

        public WRelatorioAtacado()
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
            List<Objeto.Produto> lstProdutos = null;
            this._reportViewer.LocalReport.DataSources.Clear();

            if (Produtos != null && Produtos.Count > 0)
            {
                lstProdutos = (from p in Produtos select new Objeto.Produto { Codigo = p.Codigo, CodigoFornecedor = p.CodigoFornecedor, Nome = p.Nome, ValorPercentagemAtacado = p.ValorPercentagemAtacado, ValorPercentagemVarejo = p.ValorPercentagemVarejo, Fornecedor = p.Fornecedor, Taxas = p.Taxas, ValorBase = p.ValorBase }).ToList();
                this._reportViewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("Produto", lstProdutos));
                this._reportViewer.RefreshReport();
            }
        }

        #endregion

        #region "[Eventos]"

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                ValidarPermissao();
                ListarProdutos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Relatório Atacado", MessageBoxButton.OK, MessageBoxImage.Error);
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
                this._reportViewer.LocalReport.ReportPath = "Relatorio/Atacado.rdlc";                
                this._isReportViewerLoaded = true;
            }
        }    
        
        #endregion                               
    }
}
