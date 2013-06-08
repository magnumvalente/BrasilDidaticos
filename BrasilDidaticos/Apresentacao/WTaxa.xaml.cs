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
    /// Interaction logic for WTaxa.xaml
    /// </summary>
    public partial class WTaxa : Window
    {
        #region "[Metodos]"

        public WTaxa()
        {
            InitializeComponent();
        }

        private void ConfigurarControles()
        {
            this.Title = Comum.Util.UsuarioLogado != null ? Comum.Util.UsuarioLogado.Empresa.Nome : this.Title;
            this.txtNome.txtBox.Focus();
        }

        private void ValidarPermissao()
        {
            // Permissão módulos operacionais sistema
            btnNovo.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_CRIAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnBuscar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private void ListarTaxas()
        {
            ListarTaxas(false);
        }

        private void ListarTaxas(bool mostrarMsgVazio)
        {
            Contrato.EntradaTaxa entTaxa = new Contrato.EntradaTaxa();
            entTaxa.Chave = Comum.Util.Chave;            
            entTaxa.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entTaxa.EmpresaLogada = Comum.Parametros.EmpresaProduto;
            entTaxa.Taxa = new Contrato.Taxa();

            PreencherFiltro(entTaxa.Taxa);

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoTaxa retTaxa = servBrasilDidaticos.TaxaListar(entTaxa);
            servBrasilDidaticos.Close();

            dgTaxas.ItemsSource = retTaxa.Taxas;      

            if (mostrarMsgVazio && retTaxa.Codigo == Contrato.Constantes.COD_RETORNO_VAZIO)
                MessageBox.Show(retTaxa.Mensagem, "Taxa", MessageBoxButton.OK, MessageBoxImage.Information);                              
        }

        private void PreencherFiltro(Contrato.Taxa Taxa)
        {
            Taxa.Nome = txtNome.Conteudo;
            Taxa.Ativo = (bool)chkAtivo.Selecionado;
        }

        private void CadastrarTaxa()
        {
            WTaxaCadastro taxaCadastro = new WTaxaCadastro();
            taxaCadastro.Owner = this;
            taxaCadastro.ShowDialog();

            if (!taxaCadastro.Cancelou)
                ListarTaxas();
        }

        private void EditarTaxa(Contrato.Taxa taxa)
        {
            WTaxaCadastro taxaCadastro = new WTaxaCadastro();
            taxaCadastro.Taxa = taxa;
            taxaCadastro.ShowDialog();

            if (!taxaCadastro.Cancelou)
                ListarTaxas();
        }

        private void Limpar()
        {
            txtNome.Conteudo = string.Empty;
            txtNome.txtBox.Focus();
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
                this.ListarTaxas();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Taxa", MessageBoxButton.OK, MessageBoxImage.Error);
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
                CadastrarTaxa();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Taxa", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), "Taxa", MessageBoxButton.OK, MessageBoxImage.Error);
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
                ListarTaxas(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Taxa", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    if (((DataGridRow)sender).Item != null && ((DataGridRow)sender).Item.GetType() == typeof(Contrato.Taxa))
                    {
                        // salva as alterações
                        EditarTaxa((Contrato.Taxa)((DataGridRow)sender).Item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Taxa", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }         

        #endregion        
    }
}
