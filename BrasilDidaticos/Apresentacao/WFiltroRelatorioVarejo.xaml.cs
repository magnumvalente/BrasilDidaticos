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
    /// Interaction logic for WFiltroRelatorioVarejo.xaml
    /// </summary>
    public partial class WFiltroRelatorioVarejo : Window
    {
        #region "[Metodos]"

        public WFiltroRelatorioVarejo()
        {
            InitializeComponent();
        }

        private void ValidarPermissao()
        {
            // Permissão botões da tela         
        }

        private void ListarProdutos()
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
            Contrato.RetornoProduto retProduto = servBrasilDidaticos.ProdutoListarRelatorio(entradaProduto);
            servBrasilDidaticos.Close();

            if (retProduto.Codigo == Contrato.Constantes.COD_RETORNO_VAZIO)
                MessageBox.Show(retProduto.Mensagem, "Relatório Varejo", MessageBoxButton.OK, MessageBoxImage.Information);
            else if (retProduto.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                WRelatorioVarejo wRelatorio = new WRelatorioVarejo();
                wRelatorio.Produtos = retProduto.Produtos;
                wRelatorio.ShowActivated = true;
                wRelatorio.Show();
            }
        }

        private void PreencherFornecedores()
        {
            Contrato.EntradaFornecedor entradaFornecedor = new Contrato.EntradaFornecedor();
            entradaFornecedor.Chave = Comum.Util.Chave;
            entradaFornecedor.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaFornecedor.PreencherListaSelecao = true;
            entradaFornecedor.Fornecedor = new Contrato.Fornecedor() { Ativo = true };

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
            Contrato.RetornoFornecedor retFornecedor = servBrasilDidaticos.FornecedorListar(entradaFornecedor);
            servBrasilDidaticos.Close();

            if (retFornecedor.Fornecedores != null)
            {
                // Cria uma nova lista para receber os fornecedores
                List<Objeto.Fornecedor> lstFornecedor = new List<Objeto.Fornecedor>();
                
                // Adiciona a lista os novos produtos que foram buscados
                foreach (Contrato.Fornecedor f in retFornecedor.Fornecedores.OrderBy(f => f.Nome))
                    lstFornecedor.Add(new Objeto.Fornecedor { Selecionado = false, Id = f.Id, Codigo = f.Codigo, Nome = f.Nome });

                // Define os novos produtos
                dgFornecedores.ItemsSource = lstFornecedor;
            }
        }

        private void PreencherFiltro(Contrato.Produto produto)
        {
            produto.Codigo = txtCodigo.Conteudo;
            produto.Nome = txtNome.Conteudo;            
            produto.Ativo = (bool)chkAtivo.Selecionado;
            PreencherDadosFornecedores(produto);
        }

        private void PreencherDadosFornecedores(Contrato.Produto produto)
        {
            foreach (var item in dgFornecedores.Items)
            {
                if (item.GetType() == typeof(Objeto.Fornecedor) && ((Objeto.Fornecedor)item).Selecionado == true)
                {
                    if (produto.Fornecedores == null)
                        produto.Fornecedores = new List<Contrato.Fornecedor>();

                    produto.Fornecedores.Add(new Contrato.Fornecedor()
                    {
                        Id = ((Objeto.Fornecedor)item).Id,
                        Nome = ((Objeto.Fornecedor)item).Nome,
                        Ativo = ((Objeto.Fornecedor)item).Ativo
                    });
                }
            }
        }

        private void Limpar()
        {
            txtCodigo.Conteudo = string.Empty;
            txtNome.Conteudo = string.Empty;
            txtCodigo.txtBox.Focus();
            foreach (Objeto.Fornecedor forn in (List<Objeto.Fornecedor>)dgFornecedores.ItemsSource)
                forn.Selecionado = false;
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

        private void btnLimpar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                Limpar();
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
                ListarProdutos();
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
