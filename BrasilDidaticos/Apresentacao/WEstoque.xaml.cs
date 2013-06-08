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
    /// Interaction logic for WEstoque.xaml
    /// </summary>
    public partial class WEstoque : Window
    {

        #region "[Atributos]"

        private bool _alterou = false;
        bool _BuscarProduto = true;

        #endregion

        #region "[Propriedades]"

        public bool Alterou
        {
            get
            {
                return _alterou;
            }
        }

        #endregion

        #region "[Metodos]"

        public WEstoque()
        {
            InitializeComponent();
        }

        private void ConfigurarControles()
        {
            this.Title = Comum.Util.UsuarioLogado != null ? Comum.Util.UsuarioLogado.Empresa.Nome : this.Title;
            this.txtCodigo.txtBox.Focus();
        }

        private void ValidarPermissao()
        {
            // Permissão módulos operacionais sistema
            btnBuscar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ESTOQUE, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private void ListarProdutos()
        {            
            ListarProdutos(false);
        }

        private void ListarProdutos(bool mostrarMsgVazio)
        {
            dgProdutos.Items.Clear();

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
                foreach (Contrato.Produto p in retProduto.Produtos)
                    dgProdutos.Items.Add(p);
            }    

            if (mostrarMsgVazio && retProduto.Codigo == Contrato.Constantes.COD_RETORNO_VAZIO)
                MessageBox.Show(retProduto.Mensagem, "Produto", MessageBoxButton.OK, MessageBoxImage.Information);                              
        }

        private void PreencherFornecedores()
        {
            Contrato.EntradaFornecedor entradaFornecedor = new Contrato.EntradaFornecedor();
            entradaFornecedor.Chave = Comum.Util.Chave;
            entradaFornecedor.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaFornecedor.EmpresaLogada = Comum.Parametros.EmpresaProduto;
            entradaFornecedor.PreencherListaSelecao = true;
            entradaFornecedor.Fornecedor = new Contrato.Fornecedor() { Ativo = true };

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoFornecedor retFornecedor = servBrasilDidaticos.FornecedorListar(entradaFornecedor);
            servBrasilDidaticos.Close();

            if (retFornecedor.Fornecedores != null)
            {
                foreach (Contrato.Fornecedor fornecedor in retFornecedor.Fornecedores.OrderBy(f => f.Nome))
                {
                    cmbFornecedor.ComboBox.Items.Add(new ComboBoxItem() { Uid = fornecedor.Id.ToString(), Content = fornecedor.Nome, Tag = fornecedor });
                }
            }
        }

        private void PreencherFiltro(Contrato.Produto Produto)
        {
            Produto.Codigo = txtCodigo.Conteudo;
            Produto.CodigoFornecedor = txtCodigoFornecedor.Conteudo;
            Produto.Nome = txtNome.Conteudo;
            if (cmbFornecedor.ValorSelecionado != null)
                Produto.Fornecedor = (Contrato.Fornecedor)cmbFornecedor.ValorSelecionado;
            Produto.Ativo = (bool)chkAtivo.Selecionado;
        }

        private void CadastrarProduto()
        {
            WProdutoCadastro produtoCadastro = new WProdutoCadastro();
            produtoCadastro.Owner = this;
            produtoCadastro.ShowDialog();

            if (!produtoCadastro.Cancelou)
                ListarProdutos();
        }

        private void EditarProduto(Contrato.Produto produto)
        {
            WEstoqueCadastro estoqueCadastro = new WEstoqueCadastro();
            estoqueCadastro.Produto = produto;
            estoqueCadastro.ShowDialog();

            if (!estoqueCadastro.Cancelou)
            {
                this._alterou = true;
                this.ListarProdutos();
            }
        }

        private void Limpar()
        {
            txtCodigo.Conteudo = string.Empty;
            txtCodigoFornecedor.Conteudo = string.Empty;
            txtNome.Conteudo = string.Empty;
            cmbFornecedor.ValorSelecionado = null;
            txtCodigo.txtBox.Focus();
        }

        #endregion

        #region "[Eventos]"

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                this.ConfigurarControles();
                ValidarPermissao();
                PreencherFornecedores();
                ListarProdutos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Estoque", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }   
        }

        private void btnNovo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                CadastrarProduto();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Estoque", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), "Estoque", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), "Estoque", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }            
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Verifica se o botão esquerdo foi pressionado
                if (((DataGridRow)sender).IsSelected && e.ChangedButton == MouseButton.Left)
                {
                    this.Cursor = Cursors.Wait;

                    // verifica se existe algum item selecionado da edição
                    if (((DataGridRow)sender).Item != null && ((DataGridRow)sender).Item.GetType() == typeof(Contrato.Produto))
                    {
                        // salva as alterações
                        EditarProduto((Contrato.Produto)((DataGridRow)sender).Item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Estoque", MessageBoxButton.OK, MessageBoxImage.Error);
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

                         if (retProduto.Codigo == 0)
                         {
                             // Verifica se será necessário buscar mais produtos
                             _BuscarProduto = retProduto.Produtos.Count == e.ViewportHeight;
                             // Se existem produtos preenche o grid
                             if (retProduto.Produtos.Count > 0)
                             {
                                 foreach (Contrato.Produto p in retProduto.Produtos)
                                     dgProdutos.Items.Add(p);
                             }
                         }
                     }
                 }             
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Estoque", MessageBoxButton.OK, MessageBoxImage.Error);
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
                throw ex;
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }
        
        #endregion
                 
    }
}