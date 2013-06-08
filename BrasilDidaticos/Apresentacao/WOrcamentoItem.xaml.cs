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
using System.Collections.ObjectModel;

namespace BrasilDidaticos.Apresentacao
{
    /// <summary>
    /// Interaction logic for WOrcamentoItem.xaml
    /// </summary>
    public partial class WOrcamentoItem : Window
    {

        #region "[Atributos]"

        private bool _alterou = false;
        private bool _BuscarProduto = true;
        private ObservableCollection<Objeto.Produto> _lstProduto = new ObservableCollection<Objeto.Produto>();

        #endregion

        #region "[Propriedades]"

        public bool Alterou
        {
            get
            {
                return _alterou;
            }
        }

        public ObservableCollection<Contrato.Item> Itens
        {
            get;
            set;
        }

        #endregion

        #region "[Metodos]"

        public WOrcamentoItem()
        {
            InitializeComponent();
        }

        private void ValidarPermissao()
        {
            // Permissão módulos operacionais sistema
            btnAdicionar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_CRIAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnBuscar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
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
            entradaProduto.EmpresaLogada = Comum.Parametros.EmpresaProduto;
            entradaProduto.Produto = new Contrato.Produto();
            entradaProduto.Paginar = true;
            entradaProduto.PosicaoUltimoItem = 0;
            entradaProduto.CantidadeItens = Comum.Parametros.QuantidadeItensPagina;

            PreencherFiltro(entradaProduto.Produto);

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoProduto retProduto = servBrasilDidaticos.ProdutoListar(entradaProduto);
            servBrasilDidaticos.Close();

            if (retProduto.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                // Remove todos os produtos que não foram selecionados
                while ((from p in _lstProduto where p.Selecionado == false select p).Count() > 0)
                {
                    _lstProduto.Remove((from p in _lstProduto where p.Selecionado == false select p).First());
                }
                
                // Adiciona a lista os novos produtos que foram buscados
                foreach (Contrato.Produto p in retProduto.Produtos)
                    _lstProduto.Add(new Objeto.Produto { Selecionado = false, Id = p.Id, Codigo = p.Codigo, Nome = p.Nome, Fornecedor = p.Fornecedor, CodigoFornecedor = p.CodigoFornecedor, Quantidade = p.Quantidade, ValorBase = p.ValorBase, Taxas = p.Taxas, UnidadeMedidas = p.UnidadeMedidas });
            }

            // Define os novos produtos
            dgProdutos.ItemsSource = _lstProduto;
            
            if (mostrarMsgVazio && retProduto.Codigo == Contrato.Constantes.COD_RETORNO_VAZIO)
                MessageBox.Show(retProduto.Mensagem, "Orçamento", MessageBoxButton.OK, MessageBoxImage.Information);                              
        }

        private void PreencherFornecedores()
        {
            Contrato.EntradaFornecedor entradaFornecedor = new Contrato.EntradaFornecedor();
            entradaFornecedor.Chave = Comum.Util.Chave;
            entradaFornecedor.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaFornecedor.EmpresaLogada = Comum.Parametros.EmpresaProduto;
            entradaFornecedor.Fornecedor = new Contrato.Fornecedor() { Ativo = true };

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoFornecedor retFornecedor = servBrasilDidaticos.FornecedorListar(entradaFornecedor);
            servBrasilDidaticos.Close();

            if (retFornecedor.Fornecedores != null)
            {
                cmbFornecedor.ComboBox.Items.Add(new ComboBoxItem() { Uid = Guid.Empty.ToString(), Content = "Todos" });
                foreach (Contrato.Fornecedor fornecedor in retFornecedor.Fornecedores.OrderBy(f => f.Nome))
                {
                    cmbFornecedor.ComboBox.Items.Add(new ComboBoxItem() { Uid = fornecedor.Id.ToString(), Content = fornecedor.Nome, Tag = fornecedor });
                }
            }
        }

        private void PreencherFiltro(Contrato.Produto Produto)
        {
            Produto.Codigo = txtCodigo.Conteudo;
            Produto.Nome = txtNome.Conteudo;
            Produto.CodigoFornecedor = txtCodigoFornecedor.Conteudo;
            if (cmbFornecedor.ValorSelecionado != null)
                Produto.Fornecedor = (Contrato.Fornecedor)cmbFornecedor.ValorSelecionado;
            Produto.Ativo = true;
        }

        private void PreencherDadosProdutos()
        {
            foreach (var item in dgProdutos.Items)
            {
                if (item.GetType() == typeof(Objeto.Produto) && ((Objeto.Produto)item).Selecionado == true)
                {
                    // Verifica se a lista é nula
                    if (this.Itens == null)
                        this.Itens = new ObservableCollection<Contrato.Item>();
                    
                    // Se o produto ainda não foi adicionado
                    if ((from i in Itens where i.Produto != null && i.Produto.Codigo == ((Contrato.Produto)item).Codigo select i).Count() == 0)
                    {
                        Itens.Add(new Contrato.Item()
                        {
                            Produto = ((Contrato.Produto)item)
                        });

                        if (!_alterou)
                            _alterou = true;
                    }
                }
            }
        }

        private void ConfigurarProduto(Objeto.Produto produto)
        {
            produto.UnidadeMedidaSelecionada = new Contrato.UnidadeMedida() { QuantidadeItens = 1 };

            // Verififica se o produto possui unidades de medidada associados a ele
            if (produto.UnidadeMedidas != null)
            {
                // Verifica se existe somente uma unidade de medida
                if (produto.UnidadeMedidas.Count == 1)
                {
                    produto.UnidadeMedidaSelecionada = produto.UnidadeMedidas.First();
                }
                // Verifica se existe mais de uma unidade de medida
                else if (produto.UnidadeMedidas != null && produto.UnidadeMedidas.Count > 1)
                {
                    WProdutoUnidadeMedida wProdutoUnidadeMedida = new WProdutoUnidadeMedida();
                    wProdutoUnidadeMedida.Owner = this;
                    wProdutoUnidadeMedida.Produto = produto;
                    wProdutoUnidadeMedida.ShowDialog();
                }
            }
        }   

        private void ConfigurarControles()
        {
            this.Title = Comum.Util.UsuarioLogado != null ? Comum.Util.UsuarioLogado.Empresa.Nome : this.Title;
            this.txtCodigo.txtBox.Focus();
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
                this.PreencherFornecedores();
                this.ListarProdutos();
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

        private void btnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                PreencherDadosProdutos();
                this.Close();
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

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;                
                _BuscarProduto = true;
                ListarProdutos(true);
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

        private void dgProdutos_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            try
            {
                if (e.VerticalChange != 0)
                {
                    if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight && _BuscarProduto)
                    {
                        Contrato.EntradaProduto entradaProduto = new Contrato.EntradaProduto();
                        entradaProduto.Chave = Comum.Util.Chave;
                        entradaProduto.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
                        entradaProduto.EmpresaLogada = Comum.Parametros.EmpresaProduto;
                        entradaProduto.Produto = new Contrato.Produto() { Ativo = true };
                        entradaProduto.Paginar = true;
                        entradaProduto.PosicaoUltimoItem = int.Parse(e.ExtentHeight.ToString());
                        entradaProduto.CantidadeItens = int.Parse(e.ViewportHeight.ToString());

                        PreencherFiltro(entradaProduto.Produto);

                        Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
                        Contrato.RetornoProduto retProduto = servBrasilDidaticos.ProdutoListar(entradaProduto);
                        servBrasilDidaticos.Close();

                        if (retProduto.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
                        {
                            // Verifica se será necessário buscar mais produtos
                            _BuscarProduto = retProduto.Produtos.Count == e.ViewportHeight;
                            
                            // Se existem produtos preenche o grid
                            if (retProduto.Produtos.Count > Contrato.Constantes.COD_RETORNO_SUCESSO)
                            {                
                                // Adiciona a lista os novos produtos que foram buscados
                                foreach (Contrato.Produto p in retProduto.Produtos)
                                    _lstProduto.Add(new Objeto.Produto { Selecionado = false, Id = p.Id, Codigo = p.Codigo, Nome = p.Nome, Fornecedor = p.Fornecedor, CodigoFornecedor = p.CodigoFornecedor, ValorBase = p.ValorBase, Taxas = p.Taxas });

                                dgProdutos.ItemsSource = _lstProduto;
                            }
                        }
                    }
                }             
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

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
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

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                DataGridRow row = Comum.Util.FindVisualParent<DataGridRow>((DataGridCell)sender);
                // Verifica se a linha está selecionada
                if (row != null && row.IsSelected)
                {
                    this.Cursor = Cursors.Wait;

                    // verifica se existe algum item selecionado da edição
                    if (row.Item != null && row.Item.GetType() == typeof(Objeto.Produto))
                    {
                        // salva as alterações
                        ConfigurarProduto((Objeto.Produto)dgProdutos.SelectedItem);
                    }
                }
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

        private void dgProdutos_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                dgProdutos.SelectedItem = null;               
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
