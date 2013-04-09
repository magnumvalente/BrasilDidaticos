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
    /// Interaction logic for WPerfil.xaml
    /// </summary>
    public partial class WPerfil : Window
    {
        #region "[Metodos]"

        public WPerfil()
        {
            InitializeComponent();
        }

        private void ValidarPermissao()
        {
            // Permissão módulos operacionais sistema
            btnNovo.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PERFIL, Comum.Constantes.PERMISSAO_CRIAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnBuscar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PERFIL, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private void ListarPerfils()
        {
            ListarPerfils(false);
        }

        private void ListarPerfils(bool mostrarMsgVazio)
        {
            Contrato.EntradaPerfil entPerfil = new Contrato.EntradaPerfil();
            entPerfil.Chave = Comum.Util.Chave;
            entPerfil.Perfil = new Contrato.Perfil();
            entPerfil.UsuarioLogado = Comum.Util.UsuarioLogado.Login;

            PreencherFiltro(entPerfil.Perfil);

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
            Contrato.RetornoPerfil retPerfil = servBrasilDidaticos.PerfilListar(entPerfil);
            servBrasilDidaticos.Close();

            dgPerfis.ItemsSource = retPerfil.Perfis;      

            if (mostrarMsgVazio && retPerfil.Codigo == Contrato.Constantes.COD_RETORNO_VAZIO)
                MessageBox.Show(retPerfil.Mensagem, "Perfil", MessageBoxButton.OK, MessageBoxImage.Information);                              
        }

        private void PreencherFiltro(Contrato.Perfil Perfil)
        {
            Perfil.Codigo = txtCodigo.Conteudo;
            Perfil.Nome = txtNome.Conteudo;            
            Perfil.Ativo = (bool)chkAtivo.Selecionado;
        }

        private void CadastrarPerfil()
        {
            WPerfilCadastro perfilCadastro = new WPerfilCadastro();
            perfilCadastro.Owner = this;
            perfilCadastro.ShowDialog();

            if (!perfilCadastro.Cancelou)
                ListarPerfils();
        }

        private void EditarPerfil(Contrato.Perfil perfil)
        {
            WPerfilCadastro perfilCadastro = new WPerfilCadastro();
            perfilCadastro.Perfil = perfil;
            perfilCadastro.ShowDialog();

            if (!perfilCadastro.Cancelou)
                ListarPerfils();
        }

        private void Limpar()
        {
            txtCodigo.Conteudo = string.Empty;
            txtNome.Conteudo = string.Empty;
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
                ListarPerfils();
                txtCodigo.txtBox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Perfil", MessageBoxButton.OK, MessageBoxImage.Error);
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
                CadastrarPerfil();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Perfil", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), "Perfil", MessageBoxButton.OK, MessageBoxImage.Error);
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
                ListarPerfils(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Perfil", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    if (((DataGridRow)sender).Item != null && ((DataGridRow)sender).Item.GetType() == typeof(Contrato.Perfil))
                    {
                        // salva as alterações
                        EditarPerfil((Contrato.Perfil)((DataGridRow)sender).Item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Perfil", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        #endregion        
    }
}
