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
    /// Interaction logic for WRelatorioPedido.xaml
    /// </summary>
    public partial class WRelatorioPedido : Window
    {
        #region "[Atributos]"

        private bool _isReportViewerLoaded;

        #endregion

        #region "Propriedades"

        public Contrato.Pedido Pedido;

        #endregion

        #region "[Metodos]"

        public WRelatorioPedido()
        {
            InitializeComponent();
            _reportViewer.Load += ReportViewer_Load;
        }

        private void ValidarPermissao()
        {
            // Permissão botões da tela         
        }

        private void ListarPedido()
        {
            this._reportViewer.LocalReport.DataSources.Clear();

            if (Pedido != null)
            {
                List<Contrato.Pedido> lstPedidos = new List<Contrato.Pedido>();
                lstPedidos.Add(Pedido);

                List<Contrato.Usuario> lstResponsaveis = new List<Contrato.Usuario>();
                lstResponsaveis.Add(Pedido.Responsavel);

                this._reportViewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("ItensPedido", Pedido.ItensPedido));
                this._reportViewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("Responsaveis", lstResponsaveis));
                this._reportViewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("Pedidos", lstPedidos));
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
                ListarPedido();
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
                this._reportViewer.LocalReport.ReportPath = "Relatorio/Pedido.rdlc";                
                this._isReportViewerLoaded = true;
            }
        }    
        
        #endregion                               
    }
}
