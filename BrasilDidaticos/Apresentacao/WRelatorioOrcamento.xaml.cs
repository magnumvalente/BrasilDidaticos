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
    /// Interaction logic for WRelatorioOrcamento.xaml
    /// </summary>
    public partial class WRelatorioOrcamento : Window
    {
        #region "[Atributos]"

        private bool _isReportViewerLoaded;

        #endregion

        #region "Propriedades"

        public Contrato.Orcamento Orcamento;

        #endregion

        #region "[Metodos]"

        public WRelatorioOrcamento()
        {
            InitializeComponent();
            _reportViewer.Load += ReportViewer_Load;
        }

        private void ValidarPermissao()
        {
            // Permissão botões da tela         
        }

        private void ListarOrcamento()
        {
            this._reportViewer.LocalReport.DataSources.Clear();

            if (Orcamento != null)
            {
                List<Contrato.Cliente> lstClientes = new List<Contrato.Cliente>();
                lstClientes.Add(Orcamento.Cliente);
                List<Contrato.Orcamento> lstOrcamentos = new List<Contrato.Orcamento>();
                lstOrcamentos.Add(Orcamento);
                List<Contrato.UnidadeFederativa> lstUnidadesFederativa = new List<Contrato.UnidadeFederativa>();
                lstUnidadesFederativa.Add(Orcamento.Cliente.Uf);
                List<Contrato.Usuario> lstVendedores = new List<Contrato.Usuario>();
                lstVendedores.Add(Orcamento.Vendedor);

                this._reportViewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("Itens", Orcamento.Itens));
                this._reportViewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("Clientes", lstClientes));
                this._reportViewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("Orcamentos", lstOrcamentos));
                this._reportViewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("UnidadesFederativa", lstUnidadesFederativa));
                this._reportViewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("Vendedores", lstVendedores));
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
                ListarOrcamento();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Orçamento", MessageBoxButton.OK, MessageBoxImage.Error);
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
                this._reportViewer.LocalReport.ReportPath = "Relatorio/RelatorioOrcamento.rdlc";                
                this._isReportViewerLoaded = true;
            }
        }    
        
        #endregion                               
    }
}
