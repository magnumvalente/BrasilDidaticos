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
    /// Interaction logic for WProdutoUnidadeMedida.xaml
    /// </summary>
    public partial class WProdutoUnidadeMedida : Window
    {
        #region"[Constantes]"
        const double TAM_COLUNA_CODIGO = 40;
        #endregion

        #region "[Atributos]"

        private Objeto.Produto _produto = null;
        private bool _cancelou = false;

        #endregion

        #region "[Propriedades]"

        public Objeto.Produto Produto
        {
            get 
            {
                return _produto;
            }
            set 
            {
                _produto = value;
            }
        }

        public bool Cancelou
        {
            get 
            {
                return _cancelou;
            }
        }

        #endregion

        #region "[Metodos]"

        public WProdutoUnidadeMedida()
        {
            InitializeComponent();
        }
        
        private void ConfigurarControles()
        {
            this.Title = Comum.Util.UsuarioLogado != null ? Comum.Util.UsuarioLogado.Empresa.Nome : this.Title;
        }

        private void ValidarPermissao()
        {
            btnSalvar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_MODIFICAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            dgUnidadeMedidas.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_MODIFICAR);
        }

        /// <summary>
        /// Valida os campos do formulário
        /// </summary>
        private StringBuilder ValidarCampos()
        {
            StringBuilder strValidacao = new StringBuilder();                       

            // Verifica se foi selecionado alguma unidade
            if (_produto.UnidadeMedidaSelecionada == null)
            {
                strValidacao.Append("Por favor, selecione uma unidade de medida!");
            }

            return strValidacao;
        }

        private bool SalvarProdutoUnidadeMedida()
        {
            bool salvou = true;

            PreencherDadosUnidadeMedidas();

            StringBuilder strValidacao = ValidarCampos();

            // Verifica se as informações do usuário são válidas
            if (strValidacao.Length > 0)
            {
                MessageBox.Show(strValidacao.ToString(), "ProdutoUnidadeMedida", MessageBoxButton.OK, MessageBoxImage.Information);
                salvou = false;
            }

            return salvou;
        }

        private void PreencherDadosUnidadeMedidas()
        {
            if (dgUnidadeMedidas.SelectedItem != null)
            { 
                _produto.UnidadeMedidaSelecionada = (Contrato.UnidadeMedida)dgUnidadeMedidas.SelectedItem;
            }
        }

        private void PreencherDadosTela()
        {
            if (_produto != null)
            {
                Item.Header = Comum.Util.GroupHeader("Edição", "/BrasilDidaticos;component/Imagens/ico_editar.png");
            }
        }

        private void ListarUnidadeMedidas()
        {
            List<Objeto.UnidadeMedida> objUnidadeMedidas = new List<Objeto.UnidadeMedida>();
            objUnidadeMedidas.AddRange(_produto.UnidadeMedidas.Select( um => new Objeto.UnidadeMedida { Selecionado = false, Id = um.Id, Codigo = um.Codigo, Nome = um.Nome, QuantidadeItens = um.QuantidadeItens}));

            if (_produto.UnidadeMedidaSelecionada != null && !string.IsNullOrWhiteSpace( _produto.UnidadeMedidaSelecionada.Codigo))
            { 
                objUnidadeMedidas.Where( um => um.Codigo == _produto.UnidadeMedidaSelecionada.Codigo).FirstOrDefault().Selecionado = true;
            }

            dgUnidadeMedidas.ItemsSource = objUnidadeMedidas;
        }

        #endregion

        #region "[Eventos]"

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                this.ConfigurarControles();
                this.ValidarPermissao();
                this.PreencherDadosTela();
                this.ListarUnidadeMedidas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ProdutoUnidadeMedida", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }            
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                if (SalvarProdutoUnidadeMedida())
                    this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ProdutoUnidadeMedida", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                _cancelou = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ProdutoUnidadeMedida", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
            {
                if (!cell.IsFocused)
                {
                    cell.Focus();
                }
                DataGrid dataGrid = Comum.Util.FindVisualParent<DataGrid>(cell);
                if (dataGrid != null)
                {
                    if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                    {
                        if (!cell.IsSelected)
                            cell.IsSelected = true;
                    }
                    else
                    {
                        DataGridRow row = Comum.Util.FindVisualParent<DataGridRow>(cell);
                        if (row != null && !row.IsSelected)
                        {
                            row.IsSelected = true;
                        }
                    }
                }
            }
        }
      
        #endregion
    }
}
