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
    /// Interaction logic for OrcamentoCadastro.xaml
    /// </summary>
    public partial class WOrcamentoCadastro : Window
    {
        #region "[Constantes]"

        const double TAM_COLUNA_CODIGO = 40;

        #endregion

        #region "[Atributos]"

        private Contrato.Orcamento _orcamento = null;
        private bool _cancelou = false;

        #endregion

        #region "[Propriedades]"

        public Contrato.Orcamento Orcamento
        {
            get 
            {
                return _orcamento;
            }
            set 
            {
                _orcamento = value;
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

        public WOrcamentoCadastro()
        {
            InitializeComponent();
        }

        private void ValidarPermissao()
        {
            btnSalvar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_CRIAR) || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_MODIFICAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            txtCodigo.IsEnabled = false;
            dtpData.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_CRIAR) || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_MODIFICAR);
            cmbCliente.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_CRIAR) || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_MODIFICAR);
            cmbResponsavel.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_CRIAR) || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_MODIFICAR);
            cmbVendedor.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_CRIAR) || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtDesconto.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_CRIAR)|| Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_MODIFICAR);
            dgItens.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_CRIAR) || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_MODIFICAR);
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

            // Verifica se o cliente foi informado
            if (string.IsNullOrWhiteSpace(cmbCliente.Codigo) || string.IsNullOrWhiteSpace(cmbCliente.Nome))
            {
                cmbCliente.Erro = Visibility.Visible;
                strValidacao.Append("O campo 'Cliente' não foi informado!\n");
            }
            else
                cmbCliente.Erro = Visibility.Hidden;

            // Verifica se o responsável foi informado
            if (cmbResponsavel.ValorSelecionado == null)
            {
                cmbResponsavel.Erro = Visibility.Visible;
                strValidacao.Append("O campo 'Responsável' não foi informado!\n");
            }
            else
                cmbResponsavel.Erro = Visibility.Hidden;

            // Verifica se o vendedor foi informado
            if (cmbVendedor.ValorSelecionado == null)
            {
                cmbVendedor.Erro = Visibility.Visible;
                strValidacao.Append("O campo 'Vendedor' não foi informado!\n");
            }
            else
                cmbVendedor.Erro = Visibility.Hidden;

            return strValidacao;
        }

        private bool SalvarOrcamento()
        {
            bool salvou = true;

            StringBuilder strValidacao = ValidarCampos();

            // Verifica se as informações do usuário são válidas
            if (strValidacao.Length > 0)
            {
                MessageBox.Show(strValidacao.ToString(), "Orçamento", MessageBoxButton.OK, MessageBoxImage.Information);
                salvou = false;
            }
            else
            {
                Contrato.EntradaOrcamento entradaOrcamento = new Contrato.EntradaOrcamento();
                entradaOrcamento.Chave = Comum.Util.Chave;
                entradaOrcamento.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
                if (_orcamento == null) entradaOrcamento.Novo = true;
                entradaOrcamento.Orcamento = new Contrato.Orcamento();

                PreencherDados(entradaOrcamento.Orcamento);

                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
                Contrato.RetornoOrcamento retOrcamento = servBrasilDidaticos.OrcamentoSalvar(entradaOrcamento);
                servBrasilDidaticos.Close();

                if (retOrcamento.Codigo != Contrato.Constantes.COD_RETORNO_SUCESSO)
                {
                    MessageBox.Show(retOrcamento.Mensagem, "Orçamento", MessageBoxButton.OK, MessageBoxImage.Error);
                    salvou = false;

                    if (retOrcamento.Codigo == Contrato.Constantes.COD_REGISTRO_DUPLICADO)
                    {
                        gdOrcamentoDados.ColumnDefinitions[1].Width = new GridLength(TAM_COLUNA_CODIGO);
                    }
                }
            }

            return salvou;
        }

        private void GerarNovoCodigo()
        {
            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
            string retCodigoOrcamento = servBrasilDidaticos.OrcamentoBuscarCodigo();
            servBrasilDidaticos.Close();
            txtCodigo.Conteudo = retCodigoOrcamento;
        }

        private void PreencherDados(Contrato.Orcamento Orcamento)
        {
            Orcamento.Codigo = txtCodigo.Conteudo;
            Orcamento.Data = DateTime.Parse(dtpData.Conteudo);
            Orcamento.Cliente = new Contrato.Cliente() { Id = cmbCliente.Id, Codigo = cmbCliente.Codigo, Nome = cmbCliente.Nome };
            Orcamento.Responsavel = (Contrato.Usuario)cmbResponsavel.ValorSelecionado;
            Orcamento.Vendedor = (Contrato.Usuario)cmbVendedor.ValorSelecionado;
            Orcamento.ValorDesconto = txtDesconto.Valor != null?(decimal)txtDesconto.Valor:0;
            Orcamento.Estado = (Contrato.EstadoOrcamento)cmbEstadoOrcamento.ValorSelecionado;
            PreencherDadosItens(Orcamento);
        }

        private void PreencherDadosItens(Contrato.Orcamento Orcamento)
        {
            foreach (var item in dgItens.Items)
            {                
                if (Orcamento.Itens == null)
                    Orcamento.Itens = new List<Contrato.Item>();

                Orcamento.Itens.Add(new Contrato.Item()
                {
                    Id = ((Contrato.Item)item).Id,
                    Descricao = ((Contrato.Item)item).Descricao,
                    Produto = new Contrato.Produto() 
                            {
                                Id = ((Contrato.Item)item).Produto.Id,
                                Nome = ((Contrato.Item)item).Produto.Nome,
                                Codigo = ((Contrato.Item)item).Produto.Codigo,
                                Fornecedor = ((Contrato.Item)item).Produto.Fornecedor,
                                Ncm = ((Contrato.Item)item).Produto.Ncm,
                                ValorBase = ((Contrato.Item)item).Produto.ValorBase,
                                Taxas = ((Contrato.Item)item).Produto.Taxas,
                                Ativo = ((Contrato.Item)item).Produto.Ativo
                            },
                    Quantidade = ((Contrato.Item)item).Quantidade,
                    ValorCusto = ((Contrato.Item)item).ValorCusto,
                    ValorDesconto = ((Contrato.Item)item).ValorDesconto,
                    ValorUnitario = ((Contrato.Item)item).ValorUnitario
                });
            }
        }

        private void PreencherTela()
        {
            txtDesconto.FormatString = "P2";

            PreencherTipoOrcamento();
            PreencherResponsavel();
            PreencherVendedor();
            PreencherEstadosOrcamento();

            if (_orcamento != null)
            {
                Item.Header = Comum.Util.GroupHeader("Edição", "/BrasilDidaticos;component/Imagens/ico_editar.png");

                txtCodigo.Conteudo = _orcamento.Codigo;
                dtpData.Conteudo = _orcamento.Data.ToShortDateString();
                if (_orcamento.ValorDesconto.HasValue)
                    txtDesconto.Conteudo = _orcamento.ValorDesconto.Value.ToString("P2");
                cmbCliente.Id = _orcamento.Cliente.Id;
                cmbCliente.Codigo = _orcamento.Cliente.Codigo;
                cmbCliente.Nome = _orcamento.Cliente.Nome;
            }
            else
            {
                GerarNovoCodigo();
            }
        }

        private void PreencherEstadosOrcamento()
        {
            Contrato.EntradaEstadoOrcamento entradaEstadoOrcamento = new Contrato.EntradaEstadoOrcamento();
            entradaEstadoOrcamento.Chave = Comum.Util.Chave;
            entradaEstadoOrcamento.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaEstadoOrcamento.EstadoOrcamento = new Contrato.EstadoOrcamento();
            if (_orcamento == null) entradaEstadoOrcamento.EstadoOrcamento.Ativo = true;

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
            Contrato.RetornoEstadoOrcamento retFornecedor = servBrasilDidaticos.EstadoOrcamentoListar(entradaEstadoOrcamento);
            servBrasilDidaticos.Close();

            if (retFornecedor.EstadosOrcamento != null)
            {
                foreach (Contrato.EstadoOrcamento estadoOrcamento in retFornecedor.EstadosOrcamento)
                {
                    if (_orcamento == null)                        
                    {
                        if (string.IsNullOrWhiteSpace(estadoOrcamento.Anterior.Codigo))
                            cmbEstadoOrcamento.ComboBox.Items.Add(new ComboBoxItem()
                            {
                                Uid = estadoOrcamento.Id.ToString(),
                                Content = estadoOrcamento.Nome,
                                Tag = estadoOrcamento,
                                IsSelected = true
                            });
                    }
                    else
                    {
                        if (_orcamento.Estado.Codigo == estadoOrcamento.Codigo || (estadoOrcamento.Anterior != null && _orcamento.Estado.Codigo == estadoOrcamento.Anterior.Codigo))
                        {
                            cmbEstadoOrcamento.ComboBox.Items.Add(new ComboBoxItem()
                            {
                                Uid = estadoOrcamento.Id.ToString(),
                                Content = estadoOrcamento.Nome,
                                Tag = estadoOrcamento,
                                IsSelected = (_orcamento != null && _orcamento.Estado != null ? estadoOrcamento.Id == _orcamento.Estado.Id : false)
                            });
                        }
                    }
                }
            }
        }

        private void PreencherTipoOrcamento()
        {
            rlbTipoOrcamento.ListBox.Items.Add(new ListBoxItem() { Content = "Varejo", Tag = Contrato.Enumeradores.TipoOrcamento.Varejo, IsSelected = true });
            rlbTipoOrcamento.ListBox.Items.Add(new ListBoxItem() { Content = "Atacado", Tag = Contrato.Enumeradores.TipoOrcamento.Atacado });
        }

        private void BuscarClientePorCodigo()
        {
            if (!string.IsNullOrWhiteSpace(cmbCliente.Codigo))
            {
                Contrato.EntradaCliente entradaCliente = new Contrato.EntradaCliente();
                entradaCliente.Chave = Comum.Util.Chave;
                entradaCliente.PreencherListaSelecao = true;
                entradaCliente.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
                entradaCliente.Cliente = new Contrato.Cliente();
                entradaCliente.Cliente.Ativo = true;
                entradaCliente.Cliente.Codigo = cmbCliente.Codigo;

                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
                Contrato.RetornoCliente retCliente = servBrasilDidaticos.ClienteListar(entradaCliente);
                servBrasilDidaticos.Close();

                if (retCliente.Clientes != null)
                {
                    if (retCliente.Clientes.Count == 1)
                    {
                        cmbCliente.Id = retCliente.Clientes.First().Id;
                        cmbCliente.Codigo = retCliente.Clientes.First().Codigo;
                        cmbCliente.Nome = retCliente.Clientes.First().Nome;
                        cmbResponsavel.cmbComboBox.Focus();
                    }
                }
                else
                {
                    cmbCliente.Limpar();
                }
            }
        }

        private void BuscarClientePorNome()
        {
            if (!string.IsNullOrWhiteSpace(cmbCliente.Nome))
            {
                Contrato.EntradaCliente entradaCliente = new Contrato.EntradaCliente();
                entradaCliente.Chave = Comum.Util.Chave;
                entradaCliente.PreencherListaSelecao = true;
                entradaCliente.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
                entradaCliente.Cliente = new Contrato.Cliente();
                entradaCliente.Cliente.Ativo = true;
                entradaCliente.Cliente.Nome = cmbCliente.Nome;

                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
                Contrato.RetornoCliente retCliente = servBrasilDidaticos.ClienteListar(entradaCliente);
                servBrasilDidaticos.Close();

                if (retCliente.Clientes != null)
                {
                    if (retCliente.Clientes.Count == 1)
                    {
                        cmbCliente.Id = retCliente.Clientes.First().Id;
                        cmbCliente.Codigo = retCliente.Clientes.First().Codigo;
                        cmbCliente.Nome = retCliente.Clientes.First().Nome;
                        cmbResponsavel.cmbComboBox.Focus();
                    }
                }
                else
                {
                    cmbCliente.Limpar();
                }
            }
        }

        private void BuscarCliente()
        {

            Objeto.Cliente cliente = new Objeto.Cliente();
            cliente.Codigo = cmbCliente.Codigo;
            cliente.Nome = cmbCliente.Nome;

            WCliente wCliente = new WCliente(cliente);
            wCliente.Owner = this;
            wCliente.ShowDialog();

            if (wCliente.Cliente != null)
            {
                cmbCliente.Id = wCliente.Cliente.Id;
                cmbCliente.Codigo = wCliente.Cliente.Codigo;
                cmbCliente.Nome = wCliente.Cliente.Nome;
                cmbResponsavel.cmbComboBox.Focus();
            }
            else
            {
                if (_orcamento.Cliente == null)
                    cmbCliente.Limpar();
            }
        }

        private void PreencherVendedor()
        {
            Contrato.EntradaUsuario entradaUsuario = new Contrato.EntradaUsuario();
            entradaUsuario.Chave = Comum.Util.Chave;
            entradaUsuario.PreencherListaSelecao = true;
            entradaUsuario.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaUsuario.Usuario = new Contrato.Usuario() { Ativo = true };

            // Se o perfil para vendedor está definido
            if (Comum.Parametros.CodigoPerfilVendedor != null)
            {
                entradaUsuario.Usuario.Perfis = new List<Contrato.Perfil>();
                entradaUsuario.Usuario.Perfis.Add(new Contrato.Perfil() { Codigo = Comum.Parametros.CodigoPerfilVendedor });

                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
                Contrato.RetornoUsuario retUsuario = servBrasilDidaticos.UsuarioListar(entradaUsuario);
                servBrasilDidaticos.Close();

                if (retUsuario.Usuarios != null)
                {
                    foreach (Contrato.Usuario usuario in retUsuario.Usuarios.OrderBy(u => u.Nome))
                    {
                        cmbVendedor.ComboBox.Items.Add(new ComboBoxItem() 
                        { 
                            Uid = usuario.Id.ToString(), 
                            Content = usuario.Nome, 
                            Tag = usuario,
                            IsSelected = (_orcamento != null && _orcamento.Vendedor != null ? usuario.Id == _orcamento.Vendedor.Id : false)
                        });
                    }
                }
            }
            else
            {
                MessageBox.Show("Para incluir um novo Orçamento é necessário que exista um perfil 'Vendedor' configurado corretamente.", "Orçamento", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void PreencherResponsavel()
        {
            Contrato.EntradaUsuario entradaUsuario = new Contrato.EntradaUsuario();
            entradaUsuario.Chave = Comum.Util.Chave;
            entradaUsuario.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaUsuario.PreencherListaSelecao = true;
            entradaUsuario.Usuario = new Contrato.Usuario() { Ativo = true };

            // Se o perfil para orçamentista está definido
            if (Comum.Parametros.CodigoPerfilOrcamentista != null)
            {
                entradaUsuario.Usuario.Perfis = new List<Contrato.Perfil>();
                entradaUsuario.Usuario.Perfis.Add(new Contrato.Perfil() { Codigo = Comum.Parametros.CodigoPerfilOrcamentista });
            }

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
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
                        IsSelected = (_orcamento != null && _orcamento.Responsavel != null ? usuario.Id == _orcamento.Responsavel.Id : false)
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
                    IsSelected = _orcamento == null || (_orcamento != null && _orcamento.Responsavel != null ? Comum.Util.UsuarioLogado.Id == _orcamento.Responsavel.Id : false)
                });
            }
        }
        
        private void ListarItens()
        {
            // Se encontrou itens
            if (_orcamento != null && _orcamento.Itens != null)
            {
                // Adiciona os itens a lista de itens Orcamento
                ObservableCollection<Contrato.Item> lstItens = new ObservableCollection<Contrato.Item>();
                foreach (Contrato.Item item in _orcamento.Itens)
                {
                    lstItens.Add(item);
                    lstItens.Last().Produto = new Objeto.Produto { Selecionado = true, Id = item.Produto.Id, Codigo = item.Produto.Codigo, Nome = item.Produto.Nome, Fornecedor = item.Produto.Fornecedor, ValorBase = item.Produto.ValorBase, Taxas = item.Produto.Taxas };
                }

                //  Adiciona os itens ao grid
                dgItens.ItemsSource = lstItens;
            }
        }

        private void AtualizarItens(ObservableCollection<Contrato.Item> lstItens)
        { 
            this.AtualizarItens(lstItens, false);
        }

        private void AtualizarItens(ObservableCollection<Contrato.Item> lstItens, bool ModificouTipoOrcamento)
        {
            if (lstItens != null)
            {                
                foreach (Contrato.Item item in lstItens)
                {
                    if (item.Descricao != item.Produto.Nome) item.Descricao = item.Produto.Nome;
                    item.ValorCusto = item.Produto.ValorCusto;
                    
                    switch ((Contrato.Enumeradores.TipoOrcamento)rlbTipoOrcamento.ListBox.SelectedValue)
                    {
                        case Contrato.Enumeradores.TipoOrcamento.Atacado:
                            item.ValorUnitario = ModificouTipoOrcamento || (((Objeto.Produto)item.Produto).ValorAtacado == item.ValorUnitario || item.ValorUnitario == 0) ? ((Objeto.Produto)item.Produto).ValorAtacado : item.ValorUnitario;
                            break;
                        case Contrato.Enumeradores.TipoOrcamento.Varejo:
                            item.ValorUnitario = ModificouTipoOrcamento || (((Objeto.Produto)item.Produto).ValorVarejo == item.ValorUnitario || item.ValorUnitario == 0) ? ((Objeto.Produto)item.Produto).ValorVarejo : item.ValorUnitario;
                            break;
                    }
                }
                // Atualiza o Grid de itens
                dgItens.ItemsSource = lstItens;
            }            
        }

        private void ConfigurarControles()
        {
            dtpData.Focus();
        }

        #endregion

        #region "[Eventos]"

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                ValidarPermissao();
                PreencherTela();
                ListarItens();
                ConfigurarControles();
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

        private void btnGerarNovoCodigo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                GerarNovoCodigo();
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

        private void btnAdicionarProduto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WOrcamentoProduto orcamentoProduto = new WOrcamentoProduto();
                orcamentoProduto.Itens = (ObservableCollection<Contrato.Item>)dgItens.ItemsSource;
                orcamentoProduto.Owner = this;
                orcamentoProduto.ShowDialog();
                if (orcamentoProduto.Alterou)
                    this.AtualizarItens(orcamentoProduto.Itens);
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

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                if (SalvarOrcamento())
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

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                _cancelou = true;
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

        private void rlbTipoOrcamento_SelectionChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                this.AtualizarItens((ObservableCollection<Contrato.Item>)dgItens.ItemsSource, true);
                
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

        private void cmbCliente_BuscaClickEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                this.BuscarCliente();
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

        private void cmbCliente_CodigoGotFocusEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                BuscarClientePorCodigo();
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

        private void cmbCliente_NomeGotFocusEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                BuscarClientePorNome();
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
