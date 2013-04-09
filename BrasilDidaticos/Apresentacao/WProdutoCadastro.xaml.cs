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
    /// Interaction logic for WProdutoCadastro.xaml
    /// </summary>
    public partial class WProdutoCadastro : Window
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

        public WProdutoCadastro()
        {
            InitializeComponent();
        }

        private void ValidarPermissao()
        {
            btnSalvar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_MODIFICAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            txtCodigo.IsEnabled = false;
            txtNome.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_MODIFICAR);
            cmbFornecedor.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtNcm.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtValor.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_MODIFICAR);
            chkAtivo.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_MODIFICAR);
            dgTaxas.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_MODIFICAR);
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
                if (_produto == null) entradaProduto.Novo = true;
                entradaProduto.Produto = new Contrato.Produto();

                PreencherDados(entradaProduto.Produto);

                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
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

        private void PreencherDados(Contrato.Produto Produto)
        {
            Produto.Codigo = txtCodigo.Conteudo;
            Produto.CodigoFornecedor = txtCodigoFornecedor.Conteudo;
            Produto.Nome = txtNome.Conteudo;
            Produto.Fornecedor = (Contrato.Fornecedor)cmbFornecedor.ValorSelecionado;
            Produto.Ncm = txtNcm.Conteudo;
            Produto.ValorBase = txtValor.Valor != null?(decimal)txtValor.Valor:0;
            Produto.Ativo = (bool)chkAtivo.Selecionado;
            PreencherDadosTaxas(Produto);
        }

        private void PreencherDadosTaxas(Contrato.Produto Produto)
        {
            foreach (var item in dgTaxas.Items)
            {
                if (item.GetType() == typeof(Objeto.Taxa) && ((Objeto.Taxa)item).Selecionado == true)
                {
                    if (Produto.Taxas == null)
                        Produto.Taxas = new List<Contrato.Taxa>();

                    Produto.Taxas.Add(new Contrato.Taxa()
                    {
                        Id = ((Objeto.Taxa)item).Id,
                        Nome = ((Objeto.Taxa)item).Nome,
                        Valor = ((Objeto.Taxa)item).Valor,
                        Prioridade = ((Objeto.Taxa)item).Prioridade,
                        Ativo = ((Objeto.Taxa)item).Ativo
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
                txtNcm.Conteudo = _produto.Ncm;
                txtValor.Valor = Produto.ValorBase;
                chkAtivo.Selecionado = _produto.Ativo;
            }
            else
            {
                GerarNovoCodigo();
            }
        }

        private void GerarNovoCodigo()
        {
            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
            string retCodigoProduto = servBrasilDidaticos.ProdutoBuscarCodigo();
            servBrasilDidaticos.Close();
            txtCodigo.Conteudo = retCodigoProduto;
        }

        private void PreencherFornecedores()
        {
            Contrato.EntradaFornecedor entradaFornecedor = new Contrato.EntradaFornecedor();
            entradaFornecedor.Chave = Comum.Util.Chave;
            entradaFornecedor.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaFornecedor.Fornecedor = new Contrato.Fornecedor();
            if (_produto == null) entradaFornecedor.Fornecedor.Ativo = true;

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
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
        
        private void ListarTaxas()
        {
            List<Contrato.Taxa> lstTaxas = new List<Contrato.Taxa>();

            Contrato.EntradaTaxa entTaxa = new Contrato.EntradaTaxa();
            entTaxa.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entTaxa.Chave = Comum.Util.Chave;
            entTaxa.Taxa = new Contrato.Taxa() { Ativo = true, Produto = true };

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
            Contrato.RetornoTaxa retTaxa = servBrasilDidaticos.TaxaListar(entTaxa);
            servBrasilDidaticos.Close();
            
            // Se encontrou taxas
            if (retTaxa.Taxas != null)
                // Adiciona as taxas do Produto
                lstTaxas.AddRange(retTaxa.Taxas);

            if (cmbFornecedor.ValorSelecionado == null)
            {
                entTaxa = new Contrato.EntradaTaxa();
                entTaxa.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
                entTaxa.Chave = Comum.Util.Chave;
                entTaxa.Taxa = new Contrato.Taxa() { Ativo = true, Fornecedor = true, Produto = false };

                servBrasilDidaticos = new Servico.BrasilDidaticosClient();
                retTaxa = servBrasilDidaticos.TaxaListar(entTaxa);
                servBrasilDidaticos.Close();

                // Se encontrou taxas
                if (retTaxa.Taxas != null)
                    // Adiciona as taxas do Fornecedor
                    lstTaxas.AddRange(retTaxa.Taxas);
            }
            else
            {
                // Recupera as taxas do fornecedor
                List<Contrato.Taxa> taxas = (from f in _lstFornecedores
                                            where f.Id == ((Contrato.Fornecedor)cmbFornecedor.ValorSelecionado).Id
                                            select f).First().Taxas;
                
                // Se encontrou as taxas do fornecedor
                if (taxas != null)
                {
                    // Para cada taxa dentro da listagem de taxa
                    foreach (Contrato.Taxa tx in taxas)
                    {
                        if (tx != null)
                        {                           
                            lstTaxas.RemoveAll(t => t.Nome == tx.Nome && t.Valor == tx.Valor || t.Valor == 0);

                            if (lstTaxas.Where(t => t.Nome == tx.Nome && t.Valor != tx.Valor).Count() == 0)
                                lstTaxas.Add(tx);
                        }
                    }
                }
            }

            if (lstTaxas != null)
            {
                List<Objeto.Taxa> objTaxas = null;

                if (_produto != null && _produto.Taxas != null)
                {
                    objTaxas = new List<Objeto.Taxa>();

                    foreach (Contrato.Taxa taxa in lstTaxas)
                    {
                        if (taxa != null)
                        {
                            objTaxas.Add(new Objeto.Taxa { Selecionado = false, Id = taxa.Id, Nome = taxa.Nome, Ativo = taxa.Ativo });
                            Contrato.Taxa objTaxa = (from ft in _produto.Taxas where ft.Nome == taxa.Nome select ft).FirstOrDefault();

                            if (objTaxa != null)
                            {
                                objTaxas.Last().Selecionado = true;
                                objTaxas.Last().Valor = objTaxa.Valor;
                                objTaxas.Last().Prioridade = objTaxa.Prioridade;
                            }
                        }
                    }                    
                }
                else
                    objTaxas = (from t in lstTaxas
                                select new Objeto.Taxa { Selecionado = false, Id = t.Id, Nome = t.Nome, Valor = t.Valor , Ativo = t.Ativo }).ToList();

                dgTaxas.ItemsSource = objTaxas;
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
                PreencherDadosTela();
                ListarTaxas();
                txtNome.txtBox.Focus();
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

        private void cmbFornecedor_SelectionChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ListarTaxas();
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
            e.Handled = Comum.Util.IsTextNumeric(e.Text);
        }

        private void NumericFloatOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            string valorDecimal = e.Text;

            if (sender != null && sender.GetType() == typeof(TextBox))
                valorDecimal = ((TextBox)sender).Text + e.Text;

            e.Handled = Comum.Util.IsTextNumericFloat(e.Text) || !Comum.Util.IsDecimal(valorDecimal);
        }

        private void DataGridCell_NumericFloatOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (((DataGridCell)sender).Content.GetType() == typeof(TextBlock))
            {
                switch (((DataGridCell)sender).Column.Header.ToString())
                {
                    case "Quantidade":
                        e.Handled = Comum.Util.IsTextNumeric(e.Text);
                        break;
                    case "Desconto":
                    case "Valor Real":
                        e.Handled = Comum.Util.IsTextNumericFloat(e.Text) || !Comum.Util.IsDecimal(e.Text);
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
