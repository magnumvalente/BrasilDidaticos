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
    /// Interaction logic for WUsuario.xaml
    /// </summary>
    public partial class WUsuario : Window
    {
        #region "[Metodos]"

        public WUsuario()
        {
            InitializeComponent();
        }

        private void ValidarPermissao()
        {
            // Permissão módulos operacionais sistema
            btnNovo.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_USUARIO, Comum.Constantes.PERMISSAO_CRIAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnBuscar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_USUARIO, Comum.Constantes.PERMISSAO_CONSULTAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private void ListarUsuarios()
        {
            ListarUsuarios(false);
        }

        private void ListarUsuarios(bool mostrarMsgVazio)
        {
            Contrato.EntradaUsuario entUsuario = new Contrato.EntradaUsuario();
            entUsuario.Chave = Comum.Util.Chave;
            entUsuario.Usuario = new Contrato.Usuario();
            entUsuario.UsuarioLogado = Comum.Util.UsuarioLogado.Login;

            PreencherFiltro(entUsuario.Usuario);

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
            Contrato.RetornoUsuario retUsuario = servBrasilDidaticos.UsuarioListar(entUsuario);
            servBrasilDidaticos.Close();

            dgUsuarios.ItemsSource = retUsuario.Usuarios;      

            if (mostrarMsgVazio && retUsuario.Codigo == Contrato.Constantes.COD_RETORNO_VAZIO)
                MessageBox.Show(retUsuario.Mensagem, "Usuario", MessageBoxButton.OK, MessageBoxImage.Information);                              
        }

        private void PreencherFiltro(Contrato.Usuario Usuario)
        {
            Usuario.Nome = txtNome.Conteudo;
            Usuario.Login = txtLogin.Conteudo;
            Usuario.Ativo = (bool)chkAtivo.Selecionado;
        }

        private void CadastrarUsuario()
        {
            WUsuarioCadastro usuarioCadastro = new WUsuarioCadastro();
            usuarioCadastro.Owner = this;
            usuarioCadastro.ShowDialog();

            if (!usuarioCadastro.Cancelou)
                ListarUsuarios();
        }

        private void EditarUsuario(Contrato.Usuario usuario)
        {
            WUsuarioCadastro usuarioCadastro = new WUsuarioCadastro();
            usuarioCadastro.Usuario = usuario;
            usuarioCadastro.ShowDialog();

            if (!usuarioCadastro.Cancelou)
                ListarUsuarios();
        }              

        #endregion

        #region "[Eventos]"

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                ValidarPermissao();
                ListarUsuarios();
                txtNome.txtBox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Usuário", MessageBoxButton.OK, MessageBoxImage.Error);
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
                CadastrarUsuario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Usuário", MessageBoxButton.OK, MessageBoxImage.Error);
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
                ListarUsuarios(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Usuário", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    if (((DataGridRow)sender).Item != null && ((DataGridRow)sender).Item.GetType() == typeof(Contrato.Usuario))
                    {
                        // salva as alterações
                        EditarUsuario((Contrato.Usuario)((DataGridRow)sender).Item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Usuário", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        #endregion        
    }
}
