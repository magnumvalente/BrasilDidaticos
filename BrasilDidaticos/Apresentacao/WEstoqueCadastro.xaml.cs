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
    /// Interaction logic for WEstoqueCadastro.xaml
    /// </summary>
    public partial class WEstoqueCadastro : Window
    {
        #region"[Constantes]"
        const double TAM_COLUNA_CODIGO = 40;
        #endregion

        #region "[Atributos]"

        private Contrato.Produto _produto = null;
        private bool _cancelou = false;
        private List<Contrato.Fornecedor> _lstFornecedores = null;

        #endregion

        #region "[Propriedades]"

        public Contrato.Produto Produto
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

        public WEstoqueCadastro()
        {
            InitializeComponent();
        }
        
        private void ConfigurarControles()
        {
            this.Title = Comum.Util.UsuarioLogado != null ? Comum.Util.UsuarioLogado.Empresa.Nome : this.Title;
            if ((_produto != null && _produto.UnidadeMedidas != null && _produto.UnidadeMedidas.Count > 0))                
                this.gdProdutoDados.RowDefinitions.Last().Height = new GridLength(0);
            else
                this.txtQuantidade.txtDecimalTextBox.Focus();
        }

        private void ValidarPermissao()
        {
            btnSalvar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ESTOQUE, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ESTOQUE, Comum.Constantes.PERMISSAO_MODIFICAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            txtCodigo.IsEnabled = false;
            txtCodigoFornecedor.IsEnabled = false;
            txtNome.IsEnabled = false;
            cmbFornecedor.IsEnabled = false;
            txtQuantidade.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ESTOQUE, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ESTOQUE, Comum.Constantes.PERMISSAO_MODIFICAR);
        }

        /// <summary>
        /// Valida os campos do formulário
        /// </summary>
        private StringBuilder ValidarCampos()
        {
            StringBuilder strValidacao = new StringBuilder();                       

            // Verifica se a Nome foi informada
            if (string.IsNullOrWhiteSpace(txtNome.Conteudo.ToString()))
            {
                txtNome.Erro = Visibility.Visible;
                strValidacao.Append("O campo 'Nome' não foi informado!\n");
            }
            else
                txtNome.Erro = Visibility.Hidden;

            // Verifica se o fornecedor foi informado
            if (cmbFornecedor.ValorSelecionado == null)
            {
                cmbFornecedor.Erro = Visibility.Visible;
                strValidacao.Append("O campo 'Fornecedor' não foi informado!\n");
            }
            else
                cmbFornecedor.Erro = Visibility.Hidden;

            return strValidacao;
        }

        private bool SalvarProduto()
        {
            bool salvou = true;

            StringBuilder strValidacao = ValidarCampos();

            // Verifica se as informações do usuário são válidas
            if (strValidacao.Length > 0)
            {
                MessageBox.Show(strValidacao.ToString(), "Produto", MessageBoxButton.OK, MessageBoxImage.Information);
                salvou = false;
            }
            else
            {
                Contrato.EntradaProduto entradaProduto = new Contrato.EntradaProduto();
                entradaProduto.Chave = Comum.Util.Chave;
                entradaProduto.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
                entradaProduto.EmpresaLogada = Comum.Parametros.EmpresaProduto;
                if (_produto == null) entradaProduto.Novo = true;
                entradaProduto.Produto = _produto;

                PreencherProduto(entradaProduto.Produto);

                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
                Contrato.RetornoProduto retProduto = servBrasilDidaticos.ProdutoSalvar(entradaProduto);
                servBrasilDidaticos.Close();

                if (retProduto.Codigo != Contrato.Constantes.COD_RETORNO_SUCESSO)
                {
                    MessageBox.Show(retProduto.Mensagem, "Produto", MessageBoxButton.OK, MessageBoxImage.Error);
                    salvou = false;

                    if (retProduto.Codigo == Contrato.Constantes.COD_REGISTRO_DUPLICADO)
                    {
                        gdProdutoDados.ColumnDefinitions[1].Width = new GridLength(TAM_COLUNA_CODIGO);
                    }
                }
            }

            return salvou;
        }

        private void PreencherProduto(Contrato.Produto Produto)
        {
            Produto.Id = _produto == null ? Guid.Empty : _produto.Id;
            Produto.Codigo = txtCodigo.Conteudo;
            Produto.CodigoFornecedor = txtCodigoFornecedor.Conteudo;
            Produto.Nome = txtNome.Conteudo;
            Produto.Fornecedor = (Contrato.Fornecedor)cmbFornecedor.ValorSelecionado;
            Produto.Quantidade = txtQuantidade.Valor.HasValue ? (int)txtQuantidade.Valor : 0;
            Produto.Ativo = _produto.Ativo;
            PreencherDadosUnidadeMedidas(Produto);
        }

        private void PreencherDadosUnidadeMedidas(Contrato.Produto Produto)
        {
            foreach (var item in dgUnidadeMedidas.Items)
            {
                if (item.GetType() == typeof(Objeto.UnidadeMedida))
                {                    
                    Produto.UnidadeMedidas = new List<Contrato.UnidadeMedida>();
                    Produto.UnidadeMedidas.Add(new Contrato.UnidadeMedida()
                    {
                        Id = ((Objeto.UnidadeMedida)item).Id,
                        Nome = ((Objeto.UnidadeMedida)item).Nome,
                        Quantidade = ((Objeto.UnidadeMedida)item).Quantidade,
                        QuantidadeItens = ((Objeto.UnidadeMedida)item).QuantidadeItens,
                        Ativo = ((Objeto.UnidadeMedida)item).Ativo
                    });
                }
            }
        }

        private void PreencherDadosTela()
        {
            PreencherFornecedores();

            if (_produto != null)
            {
                Item.Header = Comum.Util.GroupHeader("Edição", "/BrasilDidaticos;component/Imagens/ico_editar.png");

                txtCodigo.Conteudo = _produto.Codigo;
                txtCodigoFornecedor.Conteudo = _produto.CodigoFornecedor;
                txtNome.Conteudo = _produto.Nome;
                txtQuantidade.Conteudo = _produto.Quantidade.ToString();
            }
            else
            {
                GerarNovoCodigo();
            }

            this.ListarUnidadeMedidas();            
        }

        private void GerarNovoCodigo()
        {
            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            string retCodigoProduto = servBrasilDidaticos.ProdutoBuscarCodigo(Comum.Util.UsuarioLogado.Empresa.Id);
            servBrasilDidaticos.Close();
            txtCodigo.Conteudo = retCodigoProduto;
        }

        private void PreencherFornecedores()
        {
            Contrato.EntradaFornecedor entradaFornecedor = new Contrato.EntradaFornecedor();
            entradaFornecedor.Chave = Comum.Util.Chave;
            entradaFornecedor.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaFornecedor.EmpresaLogada = Comum.Parametros.EmpresaProduto;
            entradaFornecedor.Fornecedor = new Contrato.Fornecedor();
            if (_produto == null) entradaFornecedor.Fornecedor.Ativo = true;

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoFornecedor retFornecedor = servBrasilDidaticos.FornecedorListar(entradaFornecedor);            
            servBrasilDidaticos.Close();
            
            if (retFornecedor.Fornecedores != null)
            {
                // Guarda os fornecedores recuperados
                _lstFornecedores = retFornecedor.Fornecedores;

                foreach (Contrato.Fornecedor fornecedor in retFornecedor.Fornecedores.OrderBy(f => f.Nome))
                {
                    cmbFornecedor.ComboBox.Items.Add(new ComboBoxItem() 
                    { 
                        Uid = fornecedor.Id.ToString(), 
                        Content = fornecedor.Nome, 
                        Tag = fornecedor, 
                        IsSelected = (_produto != null && _produto.Fornecedor != null ? fornecedor.Id == _produto.Fornecedor.Id : false)
                    });
                }
            }
        }
        
        private void ListarUnidadeMedidas()
        {
            if (_produto != null && _produto.UnidadeMedidas != null)
            {
                dgUnidadeMedidas.ItemsSource = (from t in _produto.UnidadeMedidas
                                                select new Objeto.UnidadeMedida { Selecionado = false, Id = t.Id, Nome = t.Nome, Quantidade = t.Quantidade, QuantidadeItens = t.QuantidadeItens, Ativo = t.Ativo }).ToList();
            }
        }

        #endregion

        #region "[Eventos]"

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;                
                this.ValidarPermissao();
                this.PreencherDadosTela();
                this.ConfigurarControles();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Produto", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }            
        }

        private void btnGerarNovoCodigo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                GerarNovoCodigo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Produto", MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (SalvarProduto())
                    this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Produto", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), "Produto", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }  
        }
        
        private void NumericOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = Comum.Util.IsNumeric(e.Text);
        }

        private void NumericFloatOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            string valorDecimal = e.Text;

            if (sender != null && sender.GetType() == typeof(TextBox))
                valorDecimal = ((TextBox)sender).Text + e.Text;

            e.Handled = Comum.Util.IsNumericFloat(e.Text) || !Comum.Util.IsDecimal(valorDecimal);
        }

        private void DataGridCell_NumericFloatOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (((DataGridCell)sender).Content.GetType() == typeof(TextBlock))
            {
                switch (((DataGridCell)sender).Column.Header.ToString())
                {
                    case "Quantidade":
                        e.Handled = Comum.Util.IsNumeric(e.Text);
                        break;
                    case "Desconto":
                    case "Valor Real":
                        e.Handled = Comum.Util.IsNumericFloat(e.Text) || !Comum.Util.IsDecimal(e.Text);
                        break;
                }
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
