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
    /// Interaction logic for Cliente.xaml
    /// </summary>
    public partial class WClienteCadastro : Window
    {
        #region "[Atributos]"

        private Contrato.Cliente _cliente = null;
        private bool _cancelou = false;
        private List<Contrato.Cliente> _lstClientes = null;

        #endregion

        #region "[Propriedades]"

        public Contrato.Cliente Cliente
        {
            get 
            {
                return _cliente;
            }
            set 
            {
                _cliente = value;
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

        public WClienteCadastro()
        {
            InitializeComponent();
        }

        private void ValidarPermissao()
        {            
            // Permissão módulos operacionais sistema
            btnSalvar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_MODIFICAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            // Desabilita ou habilita os controles da tela
            txtCodigo.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtNome.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_MODIFICAR);
            rlbPessoa.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtCPFCNP.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtCaixaEscolar.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtInscricaEstadual.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtTelefone.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_MODIFICAR); ;
            txtCelular.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtEmail.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_MODIFICAR);
            cmbClienteMatriz.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtCep.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtEndereco.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtNumero.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtComplemento.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtBairro.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtCidade.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_MODIFICAR);
            cmbEstado.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_MODIFICAR);
        }

        /// <summary>
        /// Valida os campos do formulário
        /// </summary>
        private StringBuilder ValidarCampos()
        {
            StringBuilder strValidacao = new StringBuilder();

            // Verifica se o Código foi informado
            if (string.IsNullOrWhiteSpace(txtCodigo.Conteudo.ToString()))
            {
                txtCodigo.Erro = Visibility.Visible;
                strValidacao.Append("O campo 'Codigo' não foi informado!\n");
            }
            else
                txtCodigo.Erro = Visibility.Hidden;

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

        private bool SalvarCliente()
        {
            bool salvou = true;

            StringBuilder strValidacao = ValidarCampos();

            // Verifica se as informações do usuário são válidas
            if (strValidacao.Length > 0)
            {
                MessageBox.Show(strValidacao.ToString(), "Cliente", MessageBoxButton.OK, MessageBoxImage.Information);
                salvou = false;
            }
            else
            {
                Contrato.EntradaCliente entradaCliente = new Contrato.EntradaCliente();
                entradaCliente.Chave = Comum.Util.Chave;
                entradaCliente.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
                if (_cliente == null) entradaCliente.Novo = true;
                entradaCliente.Cliente = new Contrato.Cliente();

                PreencherCliente(entradaCliente.Cliente);

                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
                Contrato.RetornoCliente retCliente = servBrasilDidaticos.ClienteSalvar(entradaCliente);
                servBrasilDidaticos.Close();
                
                if (retCliente.Codigo != Contrato.Constantes.COD_RETORNO_SUCESSO)
                {
                    MessageBox.Show(retCliente.Mensagem, "Cliente", MessageBoxButton.OK, MessageBoxImage.Error);
                    salvou = false;
                }
            }

            return salvou;
        }

        private void PreencherCliente(Contrato.Cliente Cliente)
        {
            Cliente.Id = Cliente != null ? Cliente.Id : Guid.NewGuid();
            Cliente.Codigo = txtCodigo.Conteudo;
            Cliente.Nome = txtNome.Conteudo;
            Cliente.CaixaEscolar = txtCaixaEscolar.Conteudo;
            Cliente.Tipo = txtCPFCNP.Tipo == Comum.Enumeradores.TipoMascara.CPF ? Contrato.Enumeradores.Pessoa.Fisica : Contrato.Enumeradores.Pessoa.Juridica;
            Cliente.Cpf_Cnpj = txtCPFCNP.Valor != null ? (string)txtCPFCNP.Valor : string.Empty;
            Cliente.InscricaoEstadual = txtInscricaEstadual.Conteudo;
            Cliente.Telefone = txtTelefone.Valor != null ? (string)txtTelefone.Valor : string.Empty;
            Cliente.Celular = txtCelular.Valor != null ? (string)txtCelular.Valor : string.Empty;
            Cliente.Email = txtEmail.Conteudo;
            Cliente.ClienteMatriz = cmbClienteMatriz.ValorSelecionado != null ? (Contrato.Cliente)cmbClienteMatriz.ValorSelecionado : null;
            Cliente.Cep = txtCep.Valor != null ? (string)txtCep.Valor : string.Empty;
            Cliente.Endereco = txtEndereco.Conteudo;
            if (txtNumero.Conteudo != string.Empty) Cliente.Numero = int.Parse(txtNumero.Conteudo);
            Cliente.Complemento = txtComplemento.Conteudo;
            Cliente.Bairro = txtBairro.Conteudo;
            Cliente.Cidade = txtCidade.Conteudo;
            Cliente.Uf = cmbEstado.ValorSelecionado != null ? (Contrato.UnidadeFederativa)cmbEstado.ValorSelecionado : null;
        }

        private void PreencherDadosCliente()
        {
            txtCep.Tipo = Comum.Enumeradores.TipoMascara.Cep;
            txtTelefone.Tipo = Comum.Enumeradores.TipoMascara.Telefone;
            txtCelular.Tipo = Comum.Enumeradores.TipoMascara.Celular;

            if (_cliente != null)
            {
                Item.Header = Comum.Util.GroupHeader("Edição", "/BrasilDidaticos;component/Imagens/ico_editar.png");
                if (_cliente.Tipo == Contrato.Enumeradores.Pessoa.Fisica)
                    txtCPFCNP.Tipo = Comum.Enumeradores.TipoMascara.CPF;
                else if (_cliente.Tipo == Contrato.Enumeradores.Pessoa.Juridica)
                    txtCPFCNP.Tipo = Comum.Enumeradores.TipoMascara.CNPJ;

                txtCodigo.Conteudo = _cliente.Codigo;
                txtNome.Conteudo = _cliente.Nome;
                rlbPessoa.ValorSelecionado = _cliente.Tipo;
                txtCPFCNP.Valor = _cliente.Cpf_Cnpj;
                txtCaixaEscolar.Conteudo = _cliente.CaixaEscolar;                
                txtInscricaEstadual.Conteudo = _cliente.InscricaoEstadual;
                txtTelefone.Conteudo = _cliente.Telefone;
                txtCelular.Conteudo = _cliente.Celular;
                txtEmail.Conteudo = _cliente.Email;
                cmbClienteMatriz.ValorSelecionado = _cliente.ClienteMatriz;
                txtCep.Valor = _cliente.Cep;
                txtEndereco.Conteudo = _cliente.Endereco;
                txtNumero.Conteudo = _cliente.Numero.ToString();
                txtComplemento.Conteudo = _cliente.Complemento;
                txtBairro.Conteudo = _cliente.Bairro;
                txtCidade.Conteudo = _cliente.Cidade;
                cmbEstado.ValorSelecionado = _cliente.Uf;
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
            PreencherDadosCliente();
            PreencherMatriz();
            PreencherUfs();
        }

        private void PreencherMatriz()
        {
            Contrato.EntradaCliente entradaCliente = new Contrato.EntradaCliente();
            entradaCliente.Chave = Comum.Util.Chave;
            entradaCliente.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaCliente.Cliente = new Contrato.Cliente();
            if (_cliente == null) entradaCliente.Cliente.Ativo = true;

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
            Contrato.RetornoCliente retCliente = servBrasilDidaticos.ClienteListar(entradaCliente);
            servBrasilDidaticos.Close();

            if (retCliente.Clientes != null)
            {
                // Guarda os clientes recuperados
                _lstClientes = retCliente.Clientes;

                foreach (Contrato.Cliente cliente in retCliente.Clientes)
                {
                    cmbClienteMatriz.ComboBox.Items.Add(new ComboBoxItem()
                    {
                        Uid = cliente.Id.ToString(),
                        Content = cliente.Nome,
                        Tag = cliente,
                        IsSelected = (_cliente != null && _cliente.ClienteMatriz != null ? cliente.Id == _cliente.ClienteMatriz.Id : false)
                    });
                }
            }
        }

        private void PreencherUfs()
        {
            if (Comum.Util.UnidadesFederativas != null && Comum.Util.UnidadesFederativas.Count > 0)
            {
                foreach (Contrato.UnidadeFederativa unidadeFederativa in Comum.Util.UnidadesFederativas)
                {
                    cmbEstado.ComboBox.Items.Add(new ComboBoxItem() 
                    { 
                        Uid = unidadeFederativa.Id.ToString(), 
                        Content = unidadeFederativa.Nome,
                        Tag = unidadeFederativa,
                        IsSelected = (_cliente != null && _cliente.Uf != null ? unidadeFederativa.Id == _cliente.Uf.Id : false)
                    });
                }
            }
        }

        private void BuscarDadosCEP()
        {
            if (txtCep.Valor != null)
            {
                CepFacil.API.Localidade objDadosCep = CepFacil.API.CepFacilBusca.BuscarLocalidade(txtCep.Valor.ToString(), Comum.Constantes.CEP_CODIGO_FILIACAO);
                if (objDadosCep != null)
                {
                    txtEndereco.Conteudo = string.Format("{0} {1}", objDadosCep.Tipo, objDadosCep.Logradouro);
                    txtBairro.Conteudo = objDadosCep.Bairro;
                    txtCidade.Conteudo = objDadosCep.Cidade;
                    cmbEstado.ValorSelecionado = Comum.Util.UnidadesFederativas.FirstOrDefault(uf => uf.Codigo == objDadosCep.Uf);
                }
            }
        }
        
        private void ConfigurarControles()
        {
            txtCodigo.txtBox.Focus();
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
                ConfigurarControles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Cliente", MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (SalvarCliente())
                    this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Cliente", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), "Cliente", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), "Cliente", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            } 
        }

        private void txtCep_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                this.BuscarDadosCEP();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Cliente", MessageBoxButton.OK, MessageBoxImage.Error);
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
