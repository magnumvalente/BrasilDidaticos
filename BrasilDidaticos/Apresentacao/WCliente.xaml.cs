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
        #region "[Metodos]"

        public WCliente()
        {
            InitializeComponent();
        }

        private void ConfigurarControles()
        {
            txtCodigo.txtBox.Focus();
        }

        private void ValidarPermissao()
        {
            // Permissão módulos operacionais sistema
            btnNovo.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CRIAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnBuscar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CONSULTAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
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
            entradaCliente.Cliente = new Contrato.Cliente();

            PreencherFiltro(entradaCliente.Cliente);

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
            Contrato.RetornoCliente retCliente = servBrasilDidaticos.ClienteListar(entradaCliente);
            servBrasilDidaticos.Close();

            dgClientes.ItemsSource = retCliente.Clientes;      

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

        #endregion

        #region "[Eventos]"

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                ValidarPermissao();
                PreencherPessoa();
                ListarClientes();
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

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Verifica se o botão esquerdo foi pressionado
                if (((DataGridRow)sender).IsSelected && e.ChangedButton == MouseButton.Left)
                {
                    this.Cursor = Cursors.Wait;

                    // verifica se existe algum item selecionado da edição
                    if (((DataGridRow)sender).Item != null && ((DataGridRow)sender).Item.GetType() == typeof(Contrato.Cliente))
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
