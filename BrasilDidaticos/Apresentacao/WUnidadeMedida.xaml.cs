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
    /// Interaction logic for WUnidadeMedida.xaml
    /// </summary>
    public partial class WUnidadeMedida : Window
    {
        #region "[Metodos]"

        public WUnidadeMedida()
        {
            InitializeComponent();
        }

        private void ConfigurarControles()
        {
            this.Title = Comum.Util.UsuarioLogado != null ? Comum.Util.UsuarioLogado.Empresa.Nome : this.Title;
            this.txtCodigo.txtBox.Focus();
        }

        private void ValidarPermissao()
        {
            // Permissão módulos operacionais sistema
            btnNovo.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_UNIDADE_MEDIDA, Comum.Constantes.PERMISSAO_CRIAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnBuscar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_UNIDADE_MEDIDA, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private void ListarUnidadeMedidas()
        {
            ListarUnidadeMedidas(false);
        }

        private void ListarUnidadeMedidas(bool mostrarMsgVazio)
        {
            Contrato.EntradaUnidadeMedida entUnidadeMedida = new Contrato.EntradaUnidadeMedida();
            entUnidadeMedida.Chave = Comum.Util.Chave;            
            entUnidadeMedida.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entUnidadeMedida.EmpresaLogada = Comum.Parametros.EmpresaProduto;
            entUnidadeMedida.UnidadeMedida = new Contrato.UnidadeMedida();

            PreencherFiltro(entUnidadeMedida.UnidadeMedida);

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoUnidadeMedida retUnidadeMedida = servBrasilDidaticos.UnidadeMedidaListar(entUnidadeMedida);
            servBrasilDidaticos.Close();

            dgUnidadeMedidas.ItemsSource = retUnidadeMedida.UnidadeMedidas;      

            if (mostrarMsgVazio && retUnidadeMedida.Codigo == Contrato.Constantes.COD_RETORNO_VAZIO)
                MessageBox.Show(retUnidadeMedida.Mensagem, "Unidade de Medida", MessageBoxButton.OK, MessageBoxImage.Information);                              
        }

        private void PreencherFiltro(Contrato.UnidadeMedida UnidadeMedida)
        {
            UnidadeMedida.Codigo = txtCodigo.Conteudo;
            UnidadeMedida.Nome = txtNome.Conteudo;
            UnidadeMedida.Ativo = (bool)chkAtivo.Selecionado;
        }

        private void CadastrarUnidadeMedida()
        {
            WUnidadeMedidaCadastro unidadeMedidaCadastro = new WUnidadeMedidaCadastro();
            unidadeMedidaCadastro.Owner = this;
            unidadeMedidaCadastro.ShowDialog();

            if (!unidadeMedidaCadastro.Cancelou)
                ListarUnidadeMedidas();
        }

        private void EditarUnidadeMedida(Contrato.UnidadeMedida taxa)
        {
            WUnidadeMedidaCadastro unidadeMedidaCadastro = new WUnidadeMedidaCadastro();
            unidadeMedidaCadastro.UnidadeMedida = taxa;
            unidadeMedidaCadastro.ShowDialog();

            if (!unidadeMedidaCadastro.Cancelou)
                ListarUnidadeMedidas();
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
                this.ConfigurarControles();
                this.ValidarPermissao();
                this.ListarUnidadeMedidas();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Unidade de Medida", MessageBoxButton.OK, MessageBoxImage.Error);
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
                CadastrarUnidadeMedida();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Unidade de Medida", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), "Unidade de Medida", MessageBoxButton.OK, MessageBoxImage.Error);
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
                ListarUnidadeMedidas(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Unidade de Medida", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    if (((DataGridRow)sender).Item != null && ((DataGridRow)sender).Item.GetType() == typeof(Contrato.UnidadeMedida))
                    {
                        // salva as alterações
                        EditarUnidadeMedida((Contrato.UnidadeMedida)((DataGridRow)sender).Item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Unidade de Medida", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }         

        #endregion        
    }
}
