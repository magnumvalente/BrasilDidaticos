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
    /// Interaction logic for WCliente.xaml
    /// </summary>
    public partial class WCliente : Window
    {
        #region "[Atributo]"

        private Objeto.Cliente _Cliente = null;

        #endregion

        #region "[Propriedades]"

        public Objeto.Cliente Cliente
        { 
            get { return _Cliente; }
            set { _Cliente = value; }
        }

        #endregion

        #region "[Metodos]"

        public WCliente()
        {
            InitializeComponent();
        }

        public WCliente(Objeto.Cliente cliente)
        {
            InitializeComponent();

            _Cliente = new Objeto.Cliente();
            _Cliente.Codigo = txtCodigo.Conteudo = cliente.Codigo;
            _Cliente.Nome = txtNome.Conteudo = cliente.Nome;
        }

        private void ConfigurarControles()
        {
            this.Title = Comum.Util.UsuarioLogado != null ? Comum.Util.UsuarioLogado.Empresa.Nome : this.Title;
            this.txtCodigo.txtBox.Focus();
        }

        private void ValidarPermissao()
        {
            // Permissão módulos operacionais sistema
            btnNovo.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CRIAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnBuscar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnSelecionar.Visibility = _Cliente == null ? Visibility.Hidden : Visibility.Visible;

            // Permissão Selecionar DataGrid
            DataGridColumn dgColuna = null;
            dgColuna = dgClientes.Columns[0];
            dgColuna.Visibility = dgColuna != null && _Cliente != null ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        private void ListarClientes()
        {
            ListarClientes(false);
        }

        private void ListarClientes(bool mostrarMsgVazio)
        {
            Contrato.EntradaCliente entradaCliente = new Contrato.EntradaCliente();            
            entradaCliente.Chave = Comum.Util.Chave;
            entradaCliente.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaCliente.EmpresaLogada = Comum.Util.UsuarioLogado.Empresa;
            entradaCliente.Cliente = new Contrato.Cliente();

            PreencherFiltro(entradaCliente.Cliente);

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoCliente retCliente = servBrasilDidaticos.ClienteListar(entradaCliente);
            servBrasilDidaticos.Close();

            if (retCliente.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
                dgClientes.ItemsSource = (from c in retCliente.Clientes select new Objeto.Cliente 
                { 
                    Selecionado = false, 
                    Id = c.Id, 
                    Codigo = c.Codigo, 
                    Nome = c.Nome, 
                    CaixaEscolar = c.CaixaEscolar,
                    Tipo = c.Tipo, 
                    Cpf_Cnpj = c.Cpf_Cnpj,
                    Ativo = c.Ativo,
                    Email = c.Email,
                    Telefone = c.Telefone,
                    Celular = c.Celular,
                    InscricaoEstadual = c.InscricaoEstadual,
                    Endereco = c.Endereco,
                    Numero = c.Numero,
                    Complemento = c.Complemento,
                    Cep = c.Cep,
                    Bairro = c.Bairro,
                    Cidade = c.Cidade,
                    Uf = c.Uf,
                    ClienteMatriz = c.ClienteMatriz
                }).OrderBy(o => o.Nome);

            if (mostrarMsgVazio && retCliente.Codigo == Contrato.Constantes.COD_RETORNO_VAZIO)
                MessageBox.Show(retCliente.Mensagem, "Cliente", MessageBoxButton.OK, MessageBoxImage.Information);                              
        }

        private void PreencherFiltro(Contrato.Cliente Cliente)
        {
            Cliente.Codigo = txtCodigo.Conteudo;
            Cliente.Nome = txtNome.Conteudo;
            Cliente.CaixaEscolar = txtCaixaEscolar.Conteudo;
            if (rlbPessoa.ListBox.SelectedValue != null)
                Cliente.Tipo = (Contrato.Enumeradores.Pessoa)rlbPessoa.ListBox.SelectedValue;
            Cliente.Cpf_Cnpj = txtCPFCNP.Valor != null ? txtCPFCNP.Valor.ToString() : string.Empty;
        }

        private void CadastrarCliente()
        {
            WClienteCadastro clienteCadastro = new WClienteCadastro();
            clienteCadastro.Owner = this;
            clienteCadastro.ShowDialog();

            if (!clienteCadastro.Cancelou)
                ListarClientes();
        }

        private void EditarCliente(Contrato.Cliente cliente)
        {
            WClienteCadastro clienteCadastro = new WClienteCadastro();
            clienteCadastro.Cliente = cliente;
            clienteCadastro.ShowDialog();

            if (!clienteCadastro.Cancelou)
            {
                this.ListarClientes();
            }
        }

        private void PreencherPessoa()
        {
            rlbPessoa.ListBox.Items.Add(new ListBoxItem() { Content = "Física", Tag = Contrato.Enumeradores.Pessoa.Fisica });
            rlbPessoa.ListBox.Items.Add(new ListBoxItem() { Content = "Jurídica", Tag = Contrato.Enumeradores.Pessoa.Juridica });
        }

        private void Limpar()
        {
            txtCodigo.Conteudo = string.Empty;
            txtNome.Conteudo = string.Empty;
            txtCaixaEscolar.Conteudo = string.Empty;
            txtCPFCNP.Conteudo = string.Empty;
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
                this.ValidarPermissao();
                this.PreencherPessoa();
                this.ListarClientes();
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

        private void btnSelecionar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                // Verifica se o grid está selecionado
                if (dgClientes.SelectedItem != null)
                {
                    // verifica se existe algum item selecionado da edição
                    if (dgClientes.SelectedItem.GetType() == typeof(Objeto.Cliente))
                    {
                        // salva as alterações
                        _Cliente = (Objeto.Cliente)dgClientes.SelectedItem;
                        this.Close();
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

        private void btnNovo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                CadastrarCliente();
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

        private void btnLimpar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                Limpar();
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

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                ListarClientes(true);
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
                _Cliente = null;
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

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Verifica se o botão esquerdo foi pressionado
                if (((DataGridRow)sender).IsSelected && e.ChangedButton == MouseButton.Left)
                {
                    this.Cursor = Cursors.Wait;

                    // verifica se existe algum item selecionado da edição
                    if (((DataGridRow)sender).Item != null && ((DataGridRow)sender).Item.GetType() == typeof(Objeto.Cliente))
                    {
                        // salva as alterações
                        EditarCliente((Contrato.Cliente)((DataGridRow)sender).Item);
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
        
        private void rlbPessoa_SelectionChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;

                switch ((Contrato.Enumeradores.Pessoa)rlbPessoa.ListBox.SelectedValue)
                {
                    case Contrato.Enumeradores.Pessoa.Fisica:
                        txtCPFCNP.Tipo = Comum.Enumeradores.TipoMascara.CPF;
                        break;
                    case Contrato.Enumeradores.Pessoa.Juridica:
                        txtCPFCNP.Tipo = Comum.Enumeradores.TipoMascara.CNPJ;
                        break;
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

        #endregion                 
        
    }
}
