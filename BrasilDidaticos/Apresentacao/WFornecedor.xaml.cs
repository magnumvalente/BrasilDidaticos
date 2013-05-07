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
    /// Interaction logic for WFornecedor.xaml
    /// </summary>
    public partial class WFornecedor : Window
    {
        #region "[Atributos]"

        private bool _alterou = false;        

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

        public WFornecedor()
        {
            InitializeComponent();
        }

        private void ValidarPermissao()
        {
            // Permissão módulos operacionais sistema
            btnNovo.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_CRIAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnBuscar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private void ConfigurarControles()
        {
            this.Title = Comum.Util.UsuarioLogado != null ? Comum.Util.UsuarioLogado.Empresa.Nome : this.Title;
            this.txtNome.txtBox.Focus();
        }

        private void ListarFornecedores()
        {
            ListarFornecedores(false);
        }

        private void ListarFornecedores(bool mostrarMsgVazio)
        {
            Contrato.EntradaFornecedor entradaFornecedor = new Contrato.EntradaFornecedor();            
            entradaFornecedor.Chave = Comum.Util.Chave;
            entradaFornecedor.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaFornecedor.EmpresaLogada = Comum.Util.UsuarioLogado.Empresa;
            entradaFornecedor.Fornecedor = new Contrato.Fornecedor();

            PreencherFiltro(entradaFornecedor.Fornecedor);

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoFornecedor retFornecedor = servBrasilDidaticos.FornecedorListar(entradaFornecedor);
            servBrasilDidaticos.Close();

            dgFornecedores.ItemsSource = retFornecedor.Fornecedores;      

            if (mostrarMsgVazio && retFornecedor.Codigo == Contrato.Constantes.COD_RETORNO_VAZIO)
                MessageBox.Show(retFornecedor.Mensagem, "Fornecedor", MessageBoxButton.OK, MessageBoxImage.Information);                              
        }

        private void PreencherFiltro(Contrato.Fornecedor Fornecedor)
        {
            Fornecedor.Codigo = txtCodigo.Conteudo;
            Fornecedor.Nome = txtNome.Conteudo;
            if (rlbPessoa.ListBox.SelectedValue != null)
                Fornecedor.Tipo = (Contrato.Enumeradores.Pessoa)rlbPessoa.ListBox.SelectedValue;
            Fornecedor.Cpf_Cnpj = txtCPFCNP.Valor != null ? txtCPFCNP.Valor.ToString() : string.Empty;
            Fornecedor.Ativo = (bool)chkAtivo.Selecionado;
        }

        private void CadastrarFornecedor()
        {
            WFornecedorCadastro fornecedorCadastro = new WFornecedorCadastro();
            fornecedorCadastro.Owner = this;
            fornecedorCadastro.ShowDialog();

            if (!fornecedorCadastro.Cancelou)
                ListarFornecedores();
        }

        private void EditarFornecedor(Contrato.Fornecedor fornecedor)
        {
            WFornecedorCadastro fornecedorCadastro = new WFornecedorCadastro();
            fornecedorCadastro.Owner = this;
            fornecedorCadastro.Fornecedor = fornecedor;
            fornecedorCadastro.ShowDialog();

            if (!fornecedorCadastro.Cancelou)
            {
                this._alterou = true;
                this.ListarFornecedores();
            }
        }

        private void Limpar()
        {
            txtCodigo.Conteudo = string.Empty;
            txtNome.Conteudo = string.Empty;
            txtCPFCNP.Conteudo = string.Empty;
            txtCodigo.txtBox.Focus();
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
                this.ConfigurarControles();
                this.ValidarPermissao();
                this.PreencherPessoa();
                this.ListarFornecedores();
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

        private void btnNovo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                CadastrarFornecedor();
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

        private void btnLimpar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                Limpar();
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

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                ListarFornecedores(true);
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

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Verifica se o botão esquerdo foi pressionado
                if (((DataGridRow)sender).IsSelected && e.ChangedButton == MouseButton.Left)
                {
                    this.Cursor = Cursors.Wait;

                    // verifica se existe algum item selecionado da edição
                    if (((DataGridRow)sender).Item != null && ((DataGridRow)sender).Item.GetType() == typeof(Contrato.Fornecedor))
                    {
                        // salva as alterações
                        EditarFornecedor((Contrato.Fornecedor)((DataGridRow)sender).Item);
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
                 MessageBox.Show(ex.ToString(), "Fornecedor", MessageBoxButton.OK, MessageBoxImage.Error);
             }
             finally
             {
                 this.Cursor = Cursors.Arrow;
             } 
         }

        #endregion        
         
    }
}
