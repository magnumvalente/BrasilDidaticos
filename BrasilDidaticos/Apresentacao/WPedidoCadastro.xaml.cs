using System;
using System.Data;
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
using System.ComponentModel;

namespace BrasilDidaticos.Apresentacao
{
    /// <summary>
    /// Interaction logic for PedidoCadastro.xaml
    /// </summary>
    public partial class WPedidoCadastro : Window
    {
        #region "[Constantes]"

        const double TAM_COLUNA_CODIGO = 40;

        #endregion

        #region "[Atributos]"

        private Contrato.Pedido _pedido = null;
        private bool _cancelou = false;

        #endregion

        #region "[Propriedades]"

        public Contrato.Pedido Pedido
        {
            get 
            {
                return _pedido;
            }
            set 
            {
                _pedido = value;
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

        public WPedidoCadastro()
        {
            InitializeComponent();
        }

        private void ValidarPermissao()
        {            
            btnSalvar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PEDIDO, Comum.Constantes.PERMISSAO_CRIAR) || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_MODIFICAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            txtCodigo.IsEnabled = false;
            dtpData.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PEDIDO, Comum.Constantes.PERMISSAO_CRIAR) || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_MODIFICAR);
            cmbResponsavel.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PEDIDO, Comum.Constantes.PERMISSAO_CRIAR) || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtDesconto.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PEDIDO, Comum.Constantes.PERMISSAO_CRIAR) || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_MODIFICAR);
            dgItens.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PEDIDO, Comum.Constantes.PERMISSAO_CRIAR) || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_MODIFICAR);
        }

        /// <summary>
        /// Valida os campos do formulário
        /// </summary>
        private StringBuilder ValidarCampos()
        {
            StringBuilder strValidacao = new StringBuilder();                       

            // Verifica se a Nome foi informada
            if (string.IsNullOrWhiteSpace(dtpData.Conteudo.ToString()))
            {
                dtpData.Erro = Visibility.Visible;
                strValidacao.Append("O campo 'Data' não foi informada!\n");
            }
            else
                dtpData.Erro = Visibility.Hidden;

            // Verifica se o responsável foi informado
            if (cmbResponsavel.ValorSelecionado == null)
            {
                cmbResponsavel.Erro = Visibility.Visible;
                strValidacao.Append("O campo 'Responsável' não foi informado!\n");
            }
            else
                cmbResponsavel.Erro = Visibility.Hidden;

            return strValidacao;
        }

        private bool SalvarPedido()
        {
            bool salvou = true;

            StringBuilder strValidacao = ValidarCampos();

            // Verifica se as informações do usuário são válidas
            if (strValidacao.Length > 0)
            {
                MessageBox.Show(strValidacao.ToString(), "Pedido", MessageBoxButton.OK, MessageBoxImage.Information);
                salvou = false;
            }
            else
            {
                Contrato.EntradaPedido entradaPedido = new Contrato.EntradaPedido();
                entradaPedido.Chave = Comum.Util.Chave;
                entradaPedido.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
                entradaPedido.EmpresaLogada = Comum.Util.UsuarioLogado.Empresa;
                if (_pedido == null) entradaPedido.Novo = true;
                entradaPedido.Pedido = new Contrato.Pedido();

                PreencherPedido(entradaPedido.Pedido);

                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
                Contrato.RetornoPedido retPedido = servBrasilDidaticos.PedidoSalvar(entradaPedido);
                servBrasilDidaticos.Close();

                if (retPedido.Codigo != Contrato.Constantes.COD_RETORNO_SUCESSO)
                {
                    MessageBox.Show(retPedido.Mensagem, "Pedido", MessageBoxButton.OK, MessageBoxImage.Error);
                    salvou = false;

                    if (retPedido.Codigo == Contrato.Constantes.COD_REGISTRO_DUPLICADO)
                    {
                        gdPedidoDados.ColumnDefinitions[1].Width = new GridLength(TAM_COLUNA_CODIGO);
                    }
                }
            }

            return salvou;
        }

        private void GerarNovoCodigo()
        {
            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            string retCodigoPedido = servBrasilDidaticos.PedidoBuscarCodigo(Comum.Util.UsuarioLogado.Empresa.Id);
            servBrasilDidaticos.Close();
            txtCodigo.Conteudo = retCodigoPedido;
        }

        private void PreencherPedido(Contrato.Pedido Pedido)
        {
            Pedido.Id = _pedido == null ? Guid.Empty : _pedido.Id;
            Pedido.Codigo = txtCodigo.Conteudo;
            Pedido.Data = DateTime.Parse(dtpData.Conteudo);
            Pedido.Responsavel = (Contrato.Usuario)cmbResponsavel.ValorSelecionado;
            Pedido.ValorDesconto = txtDesconto.Valor;
            Pedido.Estado = (Contrato.EstadoPedido)cmbEstadoPedido.ValorSelecionado;
            PreencherItens(Pedido);
        }

        private void PreencherItens(Contrato.Pedido Pedido)
        {
            foreach (var item in dgItens.Items)
            {                
                if (Pedido.ItensPedido == null)
                    Pedido.ItensPedido = new List<Contrato.ItemPedido>();

                Pedido.ItensPedido.Add(new Contrato.ItemPedido()
                {
                    Id = ((Contrato.ItemPedido)item).Id,
                    Quantidade = ((Contrato.ItemPedido)item).Quantidade,
                    ValorDesconto = ((Contrato.ItemPedido)item).ValorDesconto,
                    Valor = ((Contrato.ItemPedido)item).Valor
                });

                if (((Contrato.ItemPedido)item).Produto != null)
                {
                    Pedido.ItensPedido.Last().Produto = new Contrato.Produto()
                    {
                        Id = ((Contrato.ItemPedido)item).Produto.Id,
                        Nome = ((Contrato.ItemPedido)item).Produto.Nome,
                        Codigo = ((Contrato.ItemPedido)item).Produto.Codigo,
                        Fornecedor = ((Contrato.ItemPedido)item).Produto.Fornecedor,
                        Ncm = ((Contrato.ItemPedido)item).Produto.Ncm,
                        ValorBase = ((Contrato.ItemPedido)item).Produto.ValorBase,
                        Taxas = ((Contrato.ItemPedido)item).Produto.Taxas,
                        UnidadeMedidas = ((Contrato.ItemPedido)item).Produto.UnidadeMedidas,
                        Ativo = ((Contrato.ItemPedido)item).Produto.Ativo
                    };
                }

                if (((Contrato.ItemPedido)item).UnidadeMedida != null)
                {
                    Pedido.ItensPedido.Last().UnidadeMedida = new Contrato.UnidadeMedida()
                    {
                        Id = ((Contrato.ItemPedido)item).UnidadeMedida.Id,
                        Nome = ((Contrato.ItemPedido)item).UnidadeMedida.Nome,
                        Codigo = ((Contrato.ItemPedido)item).UnidadeMedida.Codigo,
                        Descricao = ((Contrato.ItemPedido)item).UnidadeMedida.Descricao,
                        Quantidade = ((Contrato.ItemPedido)item).UnidadeMedida.Quantidade,
                        QuantidadeItens = ((Contrato.ItemPedido)item).UnidadeMedida.QuantidadeItens,
                        Ativo = ((Contrato.ItemPedido)item).UnidadeMedida.Ativo
                    };
                }
            }
        }

        private void PreencherTela()
        {
            txtDesconto.FormatString = "P2";

            PreencherResponsavel();
            PreencherEstadosPedido();

            if (_pedido != null)
            {
                Item.Header = Comum.Util.GroupHeader("Edição", "/BrasilDidaticos;component/Imagens/ico_editar.png");

                txtCodigo.Conteudo = _pedido.Codigo;
                dtpData.Conteudo = _pedido.Data.ToShortDateString();
                if (_pedido.ValorDesconto.HasValue)
                    txtDesconto.Conteudo = _pedido.ValorDesconto.Value.ToString();
            }
            else
            {
                GerarNovoCodigo();
            }
        }

        private void PreencherEstadosPedido()
        {
            Contrato.EntradaEstadoPedido entradaEstadoPedido = new Contrato.EntradaEstadoPedido();
            entradaEstadoPedido.Chave = Comum.Util.Chave;
            entradaEstadoPedido.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaEstadoPedido.EmpresaLogada = Comum.Util.UsuarioLogado.Empresa;
            entradaEstadoPedido.EstadoPedido = new Contrato.EstadoPedido();
            if (_pedido == null) entradaEstadoPedido.EstadoPedido.Ativo = true;

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoEstadoPedido retFornecedor = servBrasilDidaticos.EstadoPedidoListar(entradaEstadoPedido);
            servBrasilDidaticos.Close();

            if (retFornecedor.EstadosPedido != null)
            {
                foreach (Contrato.EstadoPedido estadoPedido in retFornecedor.EstadosPedido)
                {
                    if (_pedido == null)                        
                    {
                        if (string.IsNullOrWhiteSpace(estadoPedido.Anterior.Codigo))
                            cmbEstadoPedido.ComboBox.Items.Add(new ComboBoxItem()
                            {
                                Uid = estadoPedido.Id.ToString(),
                                Content = estadoPedido.Nome,
                                Tag = estadoPedido,
                                IsSelected = true
                            });
                    }
                    else
                    {
                        if (_pedido.Estado.Codigo == estadoPedido.Codigo || (estadoPedido.Anterior != null && _pedido.Estado.Codigo == estadoPedido.Anterior.Codigo))
                        {
                            cmbEstadoPedido.ComboBox.Items.Add(new ComboBoxItem()
                            {
                                Uid = estadoPedido.Id.ToString(),
                                Content = estadoPedido.Nome,
                                Tag = estadoPedido,
                                IsSelected = (_pedido != null && _pedido.Estado != null ? estadoPedido.Id == _pedido.Estado.Id : false)
                            });
                        }
                    }
                }
            }
        }

        private void PreencherResponsavel()
        {
            Contrato.EntradaUsuario entradaUsuario = new Contrato.EntradaUsuario();
            entradaUsuario.Chave = Comum.Util.Chave;
            entradaUsuario.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaUsuario.EmpresaLogada = Comum.Util.UsuarioLogado.Empresa;
            entradaUsuario.PreencherListaSelecao = true;
            entradaUsuario.Usuario = new Contrato.Usuario() { Ativo = true };

            // Se o perfil para orçamentista está definido
            if (Comum.Parametros.CodigoPerfilOrcamentista != null)
            {
                entradaUsuario.Usuario.Perfis = new List<Contrato.Perfil>();
                entradaUsuario.Usuario.Perfis.Add(new Contrato.Perfil() { Codigo = Comum.Parametros.CodigoPerfilOrcamentista });
            }

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoUsuario retUsuario = servBrasilDidaticos.UsuarioListar(entradaUsuario);
            servBrasilDidaticos.Close();

            bool incluiuLogado = false;

            if (retUsuario.Usuarios != null)
            {
                foreach (Contrato.Usuario usuario in retUsuario.Usuarios.OrderBy(u => u.Nome))
                {
                    cmbResponsavel.ComboBox.Items.Add(new ComboBoxItem()
                    {
                        Uid = usuario.Id.ToString(),
                        Content = usuario.Nome,
                        Tag = usuario,
                        IsSelected = (_pedido != null && _pedido.Responsavel != null) ? usuario.Id == _pedido.Responsavel.Id : false
                    });

                    if (!incluiuLogado) incluiuLogado = usuario.Login == Comum.Util.UsuarioLogado.Login;
                }
            }

            if (!incluiuLogado)
            {
                cmbResponsavel.ComboBox.Items.Add(new ComboBoxItem()
                {
                    Uid = Comum.Util.UsuarioLogado.Id.ToString(),
                    Content = Comum.Util.UsuarioLogado.Nome,
                    Tag = Comum.Util.UsuarioLogado,
                    IsSelected = true
                });

            }
        }
        
        private void ListarItens()
        {
            // Se encontrou itens
            if (_pedido != null && _pedido.ItensPedido != null)
            {
                // Adiciona os itens a lista de itens Pedido
                ObservableCollection<Contrato.ItemPedido> lstItens = new ObservableCollection<Contrato.ItemPedido>();
                foreach (Contrato.ItemPedido item in _pedido.ItensPedido)
                {
                    lstItens.Add(item);
                    if (item.Produto != null)
                        lstItens.Last().Produto = new Objeto.Produto { Selecionado = true, Id = item.Produto.Id, Codigo = item.Produto.Codigo, Nome = item.Produto.Nome, Fornecedor = item.Produto.Fornecedor, ValorBase = item.Produto.ValorBase, Taxas = item.Produto.Taxas, UnidadeMedidas = item.Produto.UnidadeMedidas };
                }

                //  Adiciona os itens ao grid
                dgItens.ItemsSource = lstItens;
            }
        }

        private void AtualizarItens(ObservableCollection<Contrato.ItemPedido> lstItens)
        { 
            this.AtualizarItens(lstItens, false);
        }

        private void AtualizarItens(ObservableCollection<Contrato.ItemPedido> lstItens, bool ModificouTipoPedido)
        {
            if (lstItens != null)
            {                
                foreach (Contrato.ItemPedido item in lstItens)
                {
                    if (item.Produto != null && item.Produto.Id != Guid.Empty)
                    {                        
                        item.UnidadeMedida = item.Produto.UnidadeMedidaSelecionada;
                        item.Valor = (item.Valor == 0) ? item.Produto.ValorBase * (item.UnidadeMedida != null ? item.UnidadeMedida.QuantidadeItens : 1) : item.Valor;
                    }
                }
                // Atualiza o Grid de itens
                dgItens.ItemsSource = lstItens;
            }            
        }

        private void ConfigurarControles()
        {
            this.Title = Comum.Util.UsuarioLogado != null ? Comum.Util.UsuarioLogado.Empresa.Nome : this.Title;
            dtpData.Focus();
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
                this.PreencherTela();
                this.ListarItens();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Pedido", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), "Pedido", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void btnAdicionarProduto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WPedidoItem pedidoProduto = new WPedidoItem();
                pedidoProduto.Itens = (ObservableCollection<Contrato.ItemPedido>)dgItens.ItemsSource;
                pedidoProduto.Owner = this;
                pedidoProduto.ShowDialog();
                if (pedidoProduto.Alterou)
                    this.AtualizarItens(pedidoProduto.Itens);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Pedido", MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (SalvarPedido())
                    this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Pedido", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), "Pedido", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void dgItens_CurrentCellChanged(object sender, EventArgs e)
        {
            dgItens.CommitEdit(DataGridEditingUnit.Row, true);
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
                    case "Valor":
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
