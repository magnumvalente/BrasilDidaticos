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
    /// Interaction logic for WFornecedorCadastro.xaml
    /// </summary>
    public partial class WFornecedorCadastro : Window
    {
        #region "[Constantes]"

        const int MAX_ITEM_REMOVE = 100;
        const double TAM_COLUNA_CODIGO = 40;

        #endregion

        #region "[Atributos]"

        private Contrato.Fornecedor _fornecedor = null;
        private bool _cancelou = false;
        private List<Contrato.Produto> _produtos = null;

        #endregion

        #region "[Propriedades]"

        public Contrato.Fornecedor Fornecedor
        {
            get 
            {
                return _fornecedor;
            }
            set 
            {
                _fornecedor = value;
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

        public WFornecedorCadastro()
        {
            InitializeComponent();
        }

        private void ValidarPermissao()
        {
            btnImportarProduto.Visibility = _fornecedor == null ? Visibility.Collapsed : Visibility.Visible;
            // Permissão módulos operacionais sistema
            btnSalvar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_MODIFICAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnImportarProduto.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_IMPORTAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            gpbTaxas.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_MODIFICAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            txtCodigo.IsEnabled = false;
            txtNome.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_MODIFICAR);
            rlbPessoa.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtCPFCNP.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtValorAtacado.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtValorVarejo.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_MODIFICAR);
            chkAtivo.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_MODIFICAR);
            dgTaxas.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_MODIFICAR);
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

            return strValidacao;
        }

        private bool SalvarFornecedor()
        {
            bool salvou = true;

            StringBuilder strValidacao = ValidarCampos();

            // Verifica se as informações do usuário são válidas
            if (strValidacao.Length > 0)
            {
                MessageBox.Show(strValidacao.ToString(), "Fornecedor", MessageBoxButton.OK, MessageBoxImage.Information);
                salvou = false;
            }
            else
            {
                Contrato.EntradaFornecedor entradaFornecedor = new Contrato.EntradaFornecedor();
                entradaFornecedor.Chave = Comum.Util.Chave;
                entradaFornecedor.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
                entradaFornecedor.EmpresaLogada = Comum.Parametros.EmpresaProduto;
                if (_fornecedor == null) entradaFornecedor.Novo = true;
                entradaFornecedor.Fornecedor = new Contrato.Fornecedor();

                PreencherFornecedor(entradaFornecedor.Fornecedor);

                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
                Contrato.RetornoFornecedor retFornecedor = servBrasilDidaticos.FornecedorSalvar(entradaFornecedor);

                if (retFornecedor.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO && _produtos != null)
                {
                    Contrato.EntradaProdutos entradaProdutos = new Contrato.EntradaProdutos();
                    entradaProdutos.Chave = Comum.Util.Chave;
                    entradaProdutos.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
                    entradaProdutos.EmpresaLogada = Comum.Util.UsuarioLogado.Empresa;
                    entradaProdutos.Fornecedor = entradaFornecedor.Fornecedor;
                    Contrato.RetornoProduto retProduto = null;

                    while (_produtos.Count > 0)
                    {
                        entradaProdutos.Produtos = _produtos.Take(_produtos.Count > MAX_ITEM_REMOVE ? MAX_ITEM_REMOVE : _produtos.Count).ToList();
                        retProduto = servBrasilDidaticos.ProdutosSalvar(entradaProdutos);
                        _produtos.RemoveRange(0, _produtos.Count > MAX_ITEM_REMOVE ? MAX_ITEM_REMOVE : _produtos.Count);                      
                    }

                    if (retProduto.Codigo != Contrato.Constantes.COD_RETORNO_SUCESSO)
                    {
                        MessageBox.Show(retFornecedor.Mensagem, "Fornecedor", MessageBoxButton.OK, MessageBoxImage.Error);
                        salvou = false;
                    }
                }               

                if (retFornecedor.Codigo != Contrato.Constantes.COD_RETORNO_SUCESSO)
                {
                    MessageBox.Show(retFornecedor.Mensagem, "Fornecedor", MessageBoxButton.OK, MessageBoxImage.Error);
                    salvou = false;

                    if (retFornecedor.Codigo == Contrato.Constantes.COD_REGISTRO_DUPLICADO)
                    {
                        gdFornecedorDados.ColumnDefinitions[1].Width = new GridLength(TAM_COLUNA_CODIGO);
                    }
                }

                servBrasilDidaticos.Close();
            }

            return salvou;
        }

        private void GerarNovoCodigo()
        {
            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            string retCodigoFornecedor = servBrasilDidaticos.FornecedorBuscarCodigo(Comum.Util.UsuarioLogado.Empresa.Id);
            servBrasilDidaticos.Close();
            txtCodigo.Conteudo = retCodigoFornecedor;
        }

        private void PreencherFornecedor(Contrato.Fornecedor Fornecedor)
        {
            Fornecedor.Id = _fornecedor != null ? _fornecedor.Id : Guid.Empty;
            Fornecedor.Codigo = txtCodigo.Conteudo;
            Fornecedor.Nome = txtNome.Conteudo;
            Fornecedor.Tipo = txtCPFCNP.Tipo == Comum.Enumeradores.TipoMascara.CPF ? Contrato.Enumeradores.Pessoa.Fisica : Contrato.Enumeradores.Pessoa.Juridica;
            Fornecedor.Cpf_Cnpj = txtCPFCNP.Valor != null ? (string)txtCPFCNP.Valor : string.Empty;
            Fornecedor.ValorPercentagemAtacado = txtValorAtacado.Valor;
            Fornecedor.ValorPercentagemVarejo = txtValorVarejo.Valor;
            Fornecedor.Ativo = (bool)chkAtivo.Selecionado;
            
            PreencherTaxas(Fornecedor);
        }

        private void PreencherTaxas(Contrato.Fornecedor Fornecedor)
        {
            foreach (var item in dgTaxas.Items)
            {
                if (item.GetType() == typeof(Objeto.Taxa) && ((Objeto.Taxa)item).Selecionado == true)
                {
                    if (Fornecedor.Taxas == null)
                        Fornecedor.Taxas = new List<Contrato.Taxa>();

                    Fornecedor.Taxas.Add(new Contrato.Taxa()
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

        private void PreencherDadosFornecedor()
        {
            if (_fornecedor != null)
            {
                Item.Header = Comum.Util.GroupHeader("Edição", "/BrasilDidaticos;component/Imagens/ico_editar.png");

                txtCodigo.Conteudo = _fornecedor.Codigo;
                txtNome.Conteudo = _fornecedor.Nome;
                rlbPessoa.ValorSelecionado = _fornecedor.Tipo;
                txtCPFCNP.Valor = _fornecedor.Cpf_Cnpj;
                if (_fornecedor.Tipo == Contrato.Enumeradores.Pessoa.Fisica)
                    txtCPFCNP.Tipo = Comum.Enumeradores.TipoMascara.CPF;
                else if (_fornecedor.Tipo == Contrato.Enumeradores.Pessoa.Juridica)
                    txtCPFCNP.Tipo = Comum.Enumeradores.TipoMascara.CNPJ;
                if (_fornecedor.ValorPercentagemAtacado.HasValue)
                    txtValorAtacado.Conteudo = _fornecedor.ValorPercentagemAtacado.Value.ToString();
                if (_fornecedor.ValorPercentagemVarejo.HasValue)
                    txtValorVarejo.Conteudo = _fornecedor.ValorPercentagemVarejo.Value.ToString();
                chkAtivo.Selecionado = _fornecedor.Ativo;
            }
            else
            {
                GerarNovoCodigo();
            }
        }

        private void PreencherDadosPessoa()
        {
            rlbPessoa.ListBox.Items.Add(new ListBoxItem() { Content = "Física", Tag = Contrato.Enumeradores.Pessoa.Fisica });
            rlbPessoa.ListBox.Items.Add(new ListBoxItem() { Content = "Jurídica", Tag = Contrato.Enumeradores.Pessoa.Juridica });
        }

        private void PreencherDadosTela()
        {            
            PreencherDadosPessoa();
            PreencherDadosFornecedor();
            PreencherDadosTaxas();
        }        

        private void PreencherDadosTaxas()
        {
            Contrato.EntradaTaxa entTaxa = new Contrato.EntradaTaxa();
            entTaxa.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entTaxa.Chave = Comum.Util.Chave;
            entTaxa.EmpresaLogada = Comum.Parametros.EmpresaProduto;
            entTaxa.Taxa = new Contrato.Taxa() { Ativo = true, Fornecedor = true };

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoTaxa retTaxa = servBrasilDidaticos.TaxaListar(entTaxa);
            servBrasilDidaticos.Close();

            if (retTaxa.Codigo != Contrato.Constantes.COD_RETORNO_VAZIO)
            {
                List<Objeto.Taxa> objTaxas = null;

                if (_fornecedor != null && _fornecedor.Taxas != null)
                {
                    objTaxas = new List<Objeto.Taxa>();

                    foreach (Contrato.Taxa taxa in retTaxa.Taxas)
                    {
                        objTaxas.Add(new Objeto.Taxa { Selecionado = false, Id = taxa.Id, Nome = taxa.Nome, Ativo = taxa.Ativo});
                        Contrato.Taxa objTaxa = (from ft in _fornecedor.Taxas where ft != null && ft.Nome == taxa.Nome select ft).FirstOrDefault();

                        if (objTaxa != null)
                        {
                            objTaxas.Last().Selecionado = true;
                            objTaxas.Last().Valor = objTaxa.Valor;
                            objTaxas.Last().Prioridade = objTaxa.Prioridade;
                        }
                    }                    
                }
                else
                    objTaxas = (from t in retTaxa.Taxas
                                select new Objeto.Taxa { Selecionado = false, Id = t.Id, Nome = t.Nome, Ativo = t.Ativo }).ToList();

                dgTaxas.ItemsSource = objTaxas;

            }
        }

        private void ConfigurarControles()
        {
            this.Title = Comum.Util.UsuarioLogado != null ? Comum.Util.UsuarioLogado.Empresa.Nome : this.Title;
            this.txtNome.txtBox.Focus();
        }
        
        private void ImportarProduto()
        {            
            WImportarProdutos importarProduto = new WImportarProdutos();
            importarProduto.Owner = this;
            importarProduto.ShowDialog();

            if (_produtos != null) _produtos.Clear();

            if (!importarProduto.Cancelou)
                _produtos = importarProduto.LerProdutosArquivo();
        }

        #endregion

        #region "[Eventos]"

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                ConfigurarControles();
                ValidarPermissao();
                PreencherDadosTela();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Fornecedor", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), "Fornecedor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void btnImportarProduto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                ImportarProduto();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Fornecedor", MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (SalvarFornecedor())
                    this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Fornecedor", MessageBoxButton.OK, MessageBoxImage.Error);
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
                this._cancelou = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Fornecedor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void rlbPessoa_SelectionChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;

                if (rlbPessoa.ValorSelecionado != null)
                {
                    switch ((Contrato.Enumeradores.Pessoa)rlbPessoa.ValorSelecionado)
                    {
                        case Contrato.Enumeradores.Pessoa.Fisica:
                            txtCPFCNP.Tipo = Comum.Enumeradores.TipoMascara.CPF;
                            break;
                        case Contrato.Enumeradores.Pessoa.Juridica:
                            txtCPFCNP.Tipo = Comum.Enumeradores.TipoMascara.CNPJ;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Fornecedor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            } 
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;

                if (_produtos != null && _produtos.Count > 0 && _cancelou)
                {
                    if (MessageBox.Show("Existem produtos importados para esse fornecedor!\nTem certeza que deseja cancelar?", "Fornecedor", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
                        e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Fornecedor", MessageBoxButton.OK, MessageBoxImage.Error);
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