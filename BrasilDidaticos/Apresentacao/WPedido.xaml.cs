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
using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using System.IO;

namespace BrasilDidaticos.Apresentacao
{
    /// <summary>
    /// Interaction logic for WPedido.xaml
    /// </summary>
    public partial class WPedido : Window
    {
        #region "[Atributos]"

        private bool _alterou = false;
        bool _BuscarPedido = true;

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

        public WPedido()
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
            btnNovo.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PEDIDO, Comum.Constantes.PERMISSAO_CRIAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnBuscar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PEDIDO, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;           
        }

        private void ListarPedidos()
        {            
            ListarPedidos(false);
        }

        private void ListarPedidos(bool mostrarMsgVazio)
        {
            dgPedidos.Items.Clear();

            Contrato.EntradaPedido entradaPedido = new Contrato.EntradaPedido();            
            entradaPedido.Chave = Comum.Util.Chave;
            entradaPedido.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaPedido.EmpresaLogada = Comum.Util.UsuarioLogado.Empresa;
            entradaPedido.Pedido = new Contrato.Pedido();
            entradaPedido.Paginar = true;
            entradaPedido.PosicaoUltimoItem = 0;
            entradaPedido.CantidadeItens = Comum.Parametros.QuantidadeItensPagina;

            PreencherFiltro(entradaPedido.Pedido);

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoPedido retPedido = servBrasilDidaticos.PedidoListar(entradaPedido);
            servBrasilDidaticos.Close();

            if (retPedido.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                foreach (Contrato.Pedido c in retPedido.Pedidos)
                    dgPedidos.Items.Add(c);
            }    

            if (mostrarMsgVazio && retPedido.Codigo == Contrato.Constantes.COD_RETORNO_VAZIO)
                MessageBox.Show(retPedido.Mensagem, "Pedido", MessageBoxButton.OK, MessageBoxImage.Information);                              
        }

        private void PreencherDadosTela()
        {
            PreencherResponsavel();
            PreencherEstadosPedido();
            ListarPedidos();
        }

        private void PreencherEstadosPedido()
        {
            Contrato.EntradaEstadoPedido entradaEstadoPedido = new Contrato.EntradaEstadoPedido();
            entradaEstadoPedido.Chave = Comum.Util.Chave;
            entradaEstadoPedido.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaEstadoPedido.EmpresaLogada = Comum.Util.UsuarioLogado.Empresa;
            entradaEstadoPedido.EstadoPedido = new Contrato.EstadoPedido() { Ativo = true };

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoEstadoPedido retEstadoPedido = servBrasilDidaticos.EstadoPedidoListar(entradaEstadoPedido);
            servBrasilDidaticos.Close();

            if (retEstadoPedido.EstadosPedido != null)
            {
                cmbEstadoPedido.ComboBox.Items.Clear();
                cmbEstadoPedido.ComboBox.Items.Add(new ComboBoxItem() { Uid = Guid.Empty.ToString(), Content = "Todos" });
                foreach (Contrato.EstadoPedido estadoPedido in retEstadoPedido.EstadosPedido)
                {
                    cmbEstadoPedido.ComboBox.Items.Add(new ComboBoxItem() { Uid = estadoPedido.Id.ToString(), Content = estadoPedido.Nome, Tag = estadoPedido });
                }
            }
        }

        private void PreencherResponsavel()
        {
            Contrato.EntradaUsuario entradaUsuario = new Contrato.EntradaUsuario();
            entradaUsuario.Chave = Comum.Util.Chave;
            entradaUsuario.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaUsuario.EmpresaLogada = Comum.Util.UsuarioLogado.Empresa;
            entradaUsuario.PreencherListaSelecao = true;
            entradaUsuario.Usuario = new Contrato.Usuario() { Ativo = true };
            entradaUsuario.Usuario.Perfis = new List<Contrato.Perfil>();
            entradaUsuario.Usuario.Perfis.Add(new Contrato.Perfil() { Codigo = Comum.Parametros.CodigoPerfilOrcamentista });

            // Se o perfil para vendedor está definido
            if (Comum.Parametros.CodigoPerfilOrcamentista != null)
            {
                entradaUsuario.Usuario.Perfis = new List<Contrato.Perfil>();
                entradaUsuario.Usuario.Perfis.Add(new Contrato.Perfil() { Codigo = Comum.Parametros.CodigoPerfilOrcamentista });
            }

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoUsuario retUsuario = servBrasilDidaticos.UsuarioListar(entradaUsuario);
            servBrasilDidaticos.Close();

            bool incluiuLogado = false;

            if (retUsuario.Usuarios != null)
            {
                foreach (Contrato.Usuario usuario in retUsuario.Usuarios.OrderBy(u => u.Nome))
                {
                    cmbResponsavel.ComboBox.Items.Clear();
                    cmbResponsavel.ComboBox.Items.Add(new ComboBoxItem() { Uid = Guid.Empty.ToString(), Content = "Todos" });
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

        private void PreencherFiltro(Contrato.Pedido Pedido)
        {
            Pedido.Codigo = txtCodigo.Conteudo;
            if (dtpData.Conteudo != string.Empty)
                Pedido.Data = DateTime.Parse(dtpData.Conteudo);
            if (cmbResponsavel.ValorSelecionado != null)
                Pedido.Responsavel = (Contrato.Usuario)cmbResponsavel.ValorSelecionado;
            if (cmbEstadoPedido.ValorSelecionado != null)
                Pedido.Estado = (Contrato.EstadoPedido)cmbEstadoPedido.ValorSelecionado;
        }

        private void CadastrarPedido()
        {
            WPedidoCadastro pedidoCadastro = new WPedidoCadastro();
            pedidoCadastro.Owner = this;
            pedidoCadastro.ShowDialog();

            if (!pedidoCadastro.Cancelou)
                ListarPedidos();
        }

        private void EditarPedido(Contrato.Pedido produto)
        {
            WPedidoCadastro pedidoCadastro = new WPedidoCadastro();
            pedidoCadastro.Pedido = produto;
            pedidoCadastro.Owner = this;
            pedidoCadastro.ShowDialog();

            if (!pedidoCadastro.Cancelou)
            {
                this._alterou = true;
                this.ListarPedidos();
            }
        }

        private void Limpar()
        {
            txtCodigo.Conteudo = string.Empty;
            dtpData.Conteudo = string.Empty;
            cmbResponsavel.ValorSelecionado = null;
            cmbEstadoPedido.ValorSelecionado = null;
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
                this.PreencherDadosTela();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Pedido", MessageBoxButton.OK, MessageBoxImage.Error);
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
                CadastrarPedido();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Pedido", MessageBoxButton.OK, MessageBoxImage.Error);
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
                _BuscarPedido = true;
                ListarPedidos(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Pedido", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), "Pedido", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    if (((DataGridRow)sender).Item != null && ((DataGridRow)sender).Item.GetType() == typeof(Contrato.Pedido))
                    {
                        // salva as alterações
                        EditarPedido((Contrato.Pedido)((DataGridRow)sender).Item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Pedido", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void dgPedidos_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            try
            {
                if (e.VerticalChange != 0)
                {
                    if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight && _BuscarPedido)
                    {
                         Contrato.EntradaPedido entradaPedido = new Contrato.EntradaPedido();
                         entradaPedido.Chave = Comum.Util.Chave;
                         entradaPedido.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
                         entradaPedido.EmpresaLogada = Comum.Util.UsuarioLogado.Empresa;
                         entradaPedido.Pedido = new Contrato.Pedido();
                         entradaPedido.Paginar = true;
                         entradaPedido.PosicaoUltimoItem = int.Parse(e.ExtentHeight.ToString());
                         entradaPedido.CantidadeItens = int.Parse(e.ViewportHeight.ToString());

                         PreencherFiltro(entradaPedido.Pedido);

                         Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
                         Contrato.RetornoPedido retPedido = servBrasilDidaticos.PedidoListar(entradaPedido);
                         servBrasilDidaticos.Close();

                         if (retPedido.Codigo == 0)
                         {
                             // Verifica se será necessário buscar mais produtos
                             _BuscarPedido = retPedido.Pedidos.Count == e.ViewportHeight;
                             // Se existem produtos preenche o grid
                             if (retPedido.Pedidos.Count > 0)
                             {
                                 foreach (Contrato.Pedido p in retPedido.Pedidos)
                                     dgPedidos.Items.Add(p);
                             }
                         }
                     }
                 }             
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Pedido", MessageBoxButton.OK, MessageBoxImage.Error);
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
                 WRelatorioPedido relatorioPedido = new WRelatorioPedido();
                 relatorioPedido.Pedido = (Contrato.Pedido)((Button)e.Source).DataContext;
                 relatorioPedido.ShowDialog();
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
