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
    /// Interaction logic for WOrcamento.xaml
    /// </summary>
    public partial class WOrcamento : Window
    {
        #region "[Atributos]"

        private bool _alterou = false;
        bool _BuscarOrcamento = true;

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

        public WOrcamento()
        {
            InitializeComponent();
        }

        private void ValidarPermissao()
        {
            // Permissão módulos operacionais sistema
            btnNovo.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_CRIAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnBuscar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_CONSULTAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            // Permissão Valor Custo DataGrid
            DataGridColumn dgColuna = null;
            dgColuna = dgOrcamentos.Columns[6];
            if (dgColuna != null) dgColuna.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.VER_CUSTO) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

        }

        private void ListarOrcamentos()
        {            
            ListarOrcamentos(false);
        }

        private void ListarOrcamentos(bool mostrarMsgVazio)
        {
            dgOrcamentos.Items.Clear();

            Contrato.EntradaOrcamento entradaOrcamento = new Contrato.EntradaOrcamento();            
            entradaOrcamento.Chave = Comum.Util.Chave;
            entradaOrcamento.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaOrcamento.Orcamento = new Contrato.Orcamento();
            entradaOrcamento.Paginar = true;
            entradaOrcamento.PosicaoUltimoItem = 0;
            entradaOrcamento.CantidadeItens = Comum.Parametros.QuantidadeItensPagina;

            PreencherFiltro(entradaOrcamento.Orcamento);

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
            Contrato.RetornoOrcamento retOrcamento = servBrasilDidaticos.OrcamentoListar(entradaOrcamento);
            servBrasilDidaticos.Close();

            if (retOrcamento.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {                
                foreach (Contrato.Orcamento p in retOrcamento.Orcamentos)
                    dgOrcamentos.Items.Add(p);
            }    

            if (mostrarMsgVazio && retOrcamento.Codigo == Contrato.Constantes.COD_RETORNO_VAZIO)
                MessageBox.Show(retOrcamento.Mensagem, "Orcamento", MessageBoxButton.OK, MessageBoxImage.Information);                              
        }

        private void PreencherEstadosOrcamento()
        {
            Contrato.EntradaEstadoOrcamento entradaEstadoOrcamento = new Contrato.EntradaEstadoOrcamento();
            entradaEstadoOrcamento.Chave = Comum.Util.Chave;
            entradaEstadoOrcamento.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaEstadoOrcamento.EstadoOrcamento = new Contrato.EstadoOrcamento() { Ativo = true };

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
            Contrato.RetornoEstadoOrcamento retFornecedor = servBrasilDidaticos.EstadoOrcamentoListar(entradaEstadoOrcamento);
            servBrasilDidaticos.Close();

            if (retFornecedor.EstadosOrcamento != null)
            {
                foreach (Contrato.EstadoOrcamento estadoOrcamento in retFornecedor.EstadosOrcamento)
                {
                    cmbEstadoOrcamento.ComboBox.Items.Add(new ComboBoxItem() { Uid = estadoOrcamento.Id.ToString(), Content = estadoOrcamento.Nome, Tag = estadoOrcamento });
                }
            }
        }

        private void PreencherCliente()
        {
            Contrato.EntradaCliente entradaCliente = new Contrato.EntradaCliente();
            entradaCliente.Chave = Comum.Util.Chave;
            entradaCliente.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaCliente.Cliente = new Contrato.Cliente();
            entradaCliente.Cliente.Ativo = true;

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
            Contrato.RetornoCliente retCliente = servBrasilDidaticos.ClienteListar(entradaCliente);
            servBrasilDidaticos.Close();

            if (retCliente.Clientes != null)
            {
                foreach (Contrato.Cliente cliente in retCliente.Clientes)
                {
                    cmbCliente.ComboBox.Items.Add(new ComboBoxItem()
                    {
                        Uid = cliente.Id.ToString(),
                        Content = cliente.Nome,
                        Tag = cliente
                    });
                }
            }
        }

        private void PreencherVendedor()
        {
            Contrato.EntradaUsuario entradaUsuario = new Contrato.EntradaUsuario();
            entradaUsuario.Chave = Comum.Util.Chave;
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
                    foreach (Contrato.Usuario usuario in retUsuario.Usuarios)
                    {
                        cmbVendedor.ComboBox.Items.Add(new ComboBoxItem() { Uid = usuario.Id.ToString(), Content = usuario.Nome, Tag = usuario });
                    }
                }
            }
        }

        private void PreencherResponsavel()
        {
            Contrato.EntradaUsuario entradaUsuario = new Contrato.EntradaUsuario();
            entradaUsuario.Chave = Comum.Util.Chave;
            entradaUsuario.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaUsuario.Usuario = new Contrato.Usuario() { Ativo = true };
            entradaUsuario.Usuario.Perfis = new List<Contrato.Perfil>();
            entradaUsuario.Usuario.Perfis.Add(new Contrato.Perfil() { Codigo = Comum.Parametros.CodigoPerfilOrcamentista });

            // Se o perfil para vendedor está definido
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
                foreach (Contrato.Usuario usuario in retUsuario.Usuarios)
                {
                    cmbResponsavel.ComboBox.Items.Add(new ComboBoxItem()
                    {
                        Uid = usuario.Id.ToString(),
                        Content = usuario.Nome,
                        Tag = usuario
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
                    Tag = Comum.Util.UsuarioLogado
                });
            }
        }

        private void PreencherFiltro(Contrato.Orcamento Orcamento)
        {
            Orcamento.Codigo = txtCodigo.Conteudo;            
            if (dtpData.Conteudo != string.Empty)
                Orcamento.Data = DateTime.Parse(dtpData.Conteudo);
            if (cmbCliente.ValorSelecionado != null)
                Orcamento.Cliente = (Contrato.Cliente)cmbCliente.ValorSelecionado;
            if (cmbResponsavel.ValorSelecionado != null)
                Orcamento.Responsavel = (Contrato.Usuario)cmbResponsavel.ValorSelecionado;
            if (cmbVendedor.ValorSelecionado != null)
                Orcamento.Vendedor = (Contrato.Usuario)cmbVendedor.ValorSelecionado;
            if (cmbEstadoOrcamento.ValorSelecionado != null)
                Orcamento.Estado = (Contrato.EstadoOrcamento)cmbEstadoOrcamento.ValorSelecionado;
        }

        private void CadastrarOrcamento()
        {
            WOrcamentoCadastro orcamentoCadastro = new WOrcamentoCadastro();
            orcamentoCadastro.Owner = this;
            orcamentoCadastro.ShowDialog();

            if (!orcamentoCadastro.Cancelou)
                ListarOrcamentos();
        }

        private void EditarOrcamento(Contrato.Orcamento produto)
        {
            WOrcamentoCadastro orcamentoCadastro = new WOrcamentoCadastro();
            orcamentoCadastro.Orcamento = produto;
            orcamentoCadastro.Owner = this;
            orcamentoCadastro.ShowDialog();

            if (!orcamentoCadastro.Cancelou)
            {
                this._alterou = true;
                this.ListarOrcamentos();
            }
        }       

        #endregion

        #region "[Eventos]"

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                ValidarPermissao();
                PreencherCliente();
                PreencherVendedor();
                PreencherResponsavel();
                PreencherEstadosOrcamento();                
                ListarOrcamentos();
                txtCodigo.txtBox.Focus();
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

        private void btnNovo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                CadastrarOrcamento();
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

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;                
                _BuscarOrcamento = true;
                ListarOrcamentos(true);
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

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Verifica se o botão esquerdo foi pressionado
                if (((DataGridRow)sender).IsSelected && e.ChangedButton == MouseButton.Left)
                {
                    this.Cursor = Cursors.Wait;

                    // verifica se existe algum item selecionado da edição
                    if (((DataGridRow)sender).Item != null && ((DataGridRow)sender).Item.GetType() == typeof(Contrato.Orcamento))
                    {
                        // salva as alterações
                        EditarOrcamento((Contrato.Orcamento)((DataGridRow)sender).Item);
                    }
                }
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

        private void dgOrcamentos_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            try
            {
                if (e.VerticalChange != 0)
                {
                    if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight && _BuscarOrcamento)
                    {
                         Contrato.EntradaOrcamento entradaOrcamento = new Contrato.EntradaOrcamento();
                         entradaOrcamento.Chave = Comum.Util.Chave;
                         entradaOrcamento.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
                         entradaOrcamento.Orcamento = new Contrato.Orcamento();
                         entradaOrcamento.Paginar = true;
                         entradaOrcamento.PosicaoUltimoItem = int.Parse(e.ExtentHeight.ToString());
                         entradaOrcamento.CantidadeItens = int.Parse(e.ViewportHeight.ToString());

                         PreencherFiltro(entradaOrcamento.Orcamento);

                         Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
                         Contrato.RetornoOrcamento retOrcamento = servBrasilDidaticos.OrcamentoListar(entradaOrcamento);
                         servBrasilDidaticos.Close();

                         if (retOrcamento.Codigo == 0)
                         {
                             // Verifica se será necessário buscar mais produtos
                             _BuscarOrcamento = retOrcamento.Orcamentos.Count == e.ViewportHeight;
                             // Se existem produtos preenche o grid
                             if (retOrcamento.Orcamentos.Count > 0)
                             {
                                 foreach (Contrato.Orcamento p in retOrcamento.Orcamentos)
                                     dgOrcamentos.Items.Add(p);
                             }
                         }
                     }
                 }             
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
                 this.Close();
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

         private void btnVer_Click(object sender, RoutedEventArgs e)
         {
             try
             {
                 WRelatorioOrcamento relatorioOrcamento = new WRelatorioOrcamento();
                 relatorioOrcamento.Orcamento = (Contrato.Orcamento)((Button)e.Source).DataContext;
                 relatorioOrcamento.ShowDialog();
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

         private void btnCusto_Click(object sender, RoutedEventArgs e)
         {
             try
             {
                 WRelatorioOrcamentoCusto relatorioOrcamentoCusto = new WRelatorioOrcamentoCusto();
                 relatorioOrcamentoCusto.Orcamento = (Contrato.Orcamento)((Button)e.Source).DataContext;
                 relatorioOrcamentoCusto.ShowDialog();
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
        
        #endregion                 
    }
}
