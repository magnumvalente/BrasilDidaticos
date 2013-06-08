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

        private void ConfigurarControles()
        {
            this.Title = Comum.Util.UsuarioLogado != null ? Comum.Util.UsuarioLogado.Empresa.Nome : this.Title;
            this.txtCodigo.txtBox.Focus();
        }

        private void ValidarPermissao()
        {
            // Permissão módulos operacionais sistema
            btnNovo.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_CRIAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnBuscar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            // Permissão Valor Custo DataGrid
            DataGridColumn dgColuna = null;
            dgColuna = dgOrcamentos.Columns[7];
            if (dgColuna != null) dgColuna.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.VER_CUSTO) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

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
            entradaOrcamento.EmpresaLogada = Comum.Util.UsuarioLogado.Empresa;
            entradaOrcamento.Orcamento = new Contrato.Orcamento();
            entradaOrcamento.Paginar = true;
            entradaOrcamento.PosicaoUltimoItem = 0;
            entradaOrcamento.CantidadeItens = Comum.Parametros.QuantidadeItensPagina;

            PreencherFiltro(entradaOrcamento.Orcamento);

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoOrcamento retOrcamento = servBrasilDidaticos.OrcamentoListar(entradaOrcamento);
            servBrasilDidaticos.Close();

            if (retOrcamento.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                foreach (Contrato.Orcamento c in retOrcamento.Orcamentos)
                    dgOrcamentos.Items.Add(c);
            }    

            if (mostrarMsgVazio && retOrcamento.Codigo == Contrato.Constantes.COD_RETORNO_VAZIO)
                MessageBox.Show(retOrcamento.Mensagem, "Orcamento", MessageBoxButton.OK, MessageBoxImage.Information);                              
        }

        private void BuscarClientePorCodigo()
        {
            if (!string.IsNullOrWhiteSpace(cmbCliente.Codigo))
            {
                Contrato.EntradaCliente entradaCliente = new Contrato.EntradaCliente();
                entradaCliente.Chave = Comum.Util.Chave;
                entradaCliente.PreencherListaSelecao = true;
                entradaCliente.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
                entradaCliente.EmpresaLogada = Comum.Util.UsuarioLogado.Empresa;
                entradaCliente.Cliente = new Contrato.Cliente();
                entradaCliente.Cliente.Ativo = true;
                entradaCliente.Cliente.Codigo = cmbCliente.Codigo;

                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
                Contrato.RetornoCliente retCliente = servBrasilDidaticos.ClienteListar(entradaCliente);
                servBrasilDidaticos.Close();

                if (retCliente.Clientes != null)
                {
                    if (retCliente.Clientes.Count == 1)
                    {
                        cmbCliente.Id = retCliente.Clientes.First().Id;
                        cmbCliente.Codigo = retCliente.Clientes.First().Codigo;
                        cmbCliente.Nome = retCliente.Clientes.First().Nome;
                        cmbResponsavel.cmbComboBox.Focus();
                    }
                }
                else
                {
                    cmbCliente.Limpar();
                }
            }
        }

        private void BuscarClientePorNome()
        {
            if (!string.IsNullOrWhiteSpace(cmbCliente.Nome))
            {
                Contrato.EntradaCliente entradaCliente = new Contrato.EntradaCliente();
                entradaCliente.Chave = Comum.Util.Chave;
                entradaCliente.PreencherListaSelecao = true;
                entradaCliente.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
                entradaCliente.Cliente = new Contrato.Cliente();
                entradaCliente.Cliente.Ativo = true;
                entradaCliente.Cliente.Nome = cmbCliente.Nome;

                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
                Contrato.RetornoCliente retCliente = servBrasilDidaticos.ClienteListar(entradaCliente);
                servBrasilDidaticos.Close();

                if (retCliente.Clientes != null)
                {
                    if (retCliente.Clientes.Count == 1)
                    {
                        cmbCliente.Id = retCliente.Clientes.First().Id;
                        cmbCliente.Codigo = retCliente.Clientes.First().Codigo;
                        cmbCliente.Nome = retCliente.Clientes.First().Nome;
                        cmbResponsavel.cmbComboBox.Focus();
                    }
                }
                else
                {
                    cmbCliente.Limpar();
                }
            }
        }

        private void BuscarCliente()
        {
            
            Objeto.Cliente cliente = new Objeto.Cliente();                
            cliente.Codigo = cmbCliente.Codigo;
            cliente.Nome = cmbCliente.Nome;

            WCliente wCliente = new WCliente(cliente);
            wCliente.Owner = this;
            wCliente.ShowDialog();

            if (wCliente.Cliente != null)
            {
                cmbCliente.Id = wCliente.Cliente.Id;
                cmbCliente.Codigo = wCliente.Cliente.Codigo;
                cmbCliente.Nome = wCliente.Cliente.Nome;
                cmbResponsavel.cmbComboBox.Focus();
            }
            else
            {
                cmbCliente.Limpar();
            }
        }

        private void PreencherDadosTela()
        {
            PreencherVendedor();
            PreencherResponsavel();
            PreencherEstadosOrcamento();
            ListarOrcamentos();
        }

        private void PreencherEstadosOrcamento()
        {
            Contrato.EntradaEstadoOrcamento entradaEstadoOrcamento = new Contrato.EntradaEstadoOrcamento();
            entradaEstadoOrcamento.Chave = Comum.Util.Chave;
            entradaEstadoOrcamento.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaEstadoOrcamento.EmpresaLogada = Comum.Util.UsuarioLogado.Empresa;
            entradaEstadoOrcamento.EstadoOrcamento = new Contrato.EstadoOrcamento() { Ativo = true };

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoEstadoOrcamento retEstadoOrcamento = servBrasilDidaticos.EstadoOrcamentoListar(entradaEstadoOrcamento);
            servBrasilDidaticos.Close();

            if (retEstadoOrcamento.EstadosOrcamento != null)
            {
                cmbEstadoOrcamento.ComboBox.Items.Clear();
                cmbEstadoOrcamento.ComboBox.Items.Add(new ComboBoxItem() { Uid = Guid.Empty.ToString(), Content = "Todos" });
                foreach (Contrato.EstadoOrcamento estadoOrcamento in retEstadoOrcamento.EstadosOrcamento)
                {
                    cmbEstadoOrcamento.ComboBox.Items.Add(new ComboBoxItem() { Uid = estadoOrcamento.Id.ToString(), Content = estadoOrcamento.Nome, Tag = estadoOrcamento });
                }
            }
        }

        private void PreencherVendedor()
        {
            Contrato.EntradaUsuario entradaUsuario = new Contrato.EntradaUsuario();
            entradaUsuario.Chave = Comum.Util.Chave;
            entradaUsuario.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaUsuario.EmpresaLogada = Comum.Util.UsuarioLogado.Empresa;
            entradaUsuario.PreencherListaSelecao = true;
            entradaUsuario.Usuario = new Contrato.Usuario() { Ativo = true };
            
            // Se o perfil para vendedor está definido
            if (Comum.Parametros.CodigoPerfilVendedor != null)
            {
                entradaUsuario.Usuario.Perfis = new List<Contrato.Perfil>();
                entradaUsuario.Usuario.Perfis.Add(new Contrato.Perfil() { Codigo = Comum.Parametros.CodigoPerfilVendedor });

                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
                Contrato.RetornoUsuario retUsuario = servBrasilDidaticos.UsuarioListar(entradaUsuario);
                servBrasilDidaticos.Close();

                if (retUsuario.Usuarios != null)
                {
                    cmbVendedor.ComboBox.Items.Clear();
                    cmbVendedor.ComboBox.Items.Add(new ComboBoxItem() { Uid = Guid.Empty.ToString(), Content = "Todos" });
                    foreach (Contrato.Usuario usuario in retUsuario.Usuarios.OrderBy(u => u.Nome))
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

        private void PreencherFiltro(Contrato.Orcamento Orcamento)
        {
            Orcamento.Codigo = txtCodigo.Conteudo;            
            if (dtpData.Conteudo != string.Empty)
                Orcamento.Data = DateTime.Parse(dtpData.Conteudo);
            if (!string.IsNullOrWhiteSpace(cmbCliente.Codigo) || !string.IsNullOrWhiteSpace(cmbCliente.Nome))
                Orcamento.Cliente = new Contrato.Cliente() { Id = cmbCliente.Id, Codigo = cmbCliente.Codigo, Nome = cmbCliente.Nome };
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

        private void Limpar()
        {
            txtCodigo.Conteudo = string.Empty;
            dtpData.Conteudo = string.Empty;
            cmbCliente.Id = Guid.Empty;
            cmbCliente.Codigo = string.Empty;
            cmbCliente.Nome = string.Empty;
            cmbResponsavel.ValorSelecionado = null;
            cmbVendedor.ValorSelecionado = null;
            cmbEstadoOrcamento.ValorSelecionado = null;
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

        private void btnLimpar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                Limpar();
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

        private void cmbCliente_CodigoGotFocusEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                BuscarClientePorCodigo();
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

        private void cmbCliente_NomeGotFocusEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                BuscarClientePorNome();
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
                         entradaOrcamento.EmpresaLogada = Comum.Util.UsuarioLogado.Empresa;
                         entradaOrcamento.Orcamento = new Contrato.Orcamento();
                         entradaOrcamento.Paginar = true;
                         entradaOrcamento.PosicaoUltimoItem = int.Parse(e.ExtentHeight.ToString());
                         entradaOrcamento.CantidadeItens = int.Parse(e.ViewportHeight.ToString());

                         PreencherFiltro(entradaOrcamento.Orcamento);

                         Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
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

        private void cmbCliente_BuscaClickEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                this.BuscarCliente();
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

        private void btnExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                
                List<string> lstItensOrcamento = new List<string>();
                foreach (Contrato.Item item in ((Contrato.Orcamento)((Button)e.Source).DataContext).Itens)
                {
                    lstItensOrcamento.Add(string.Format("{0};{1};{2};{3};{4}", 
                                          item.Descricao,
                                          item.Quantidade,
                                          Comum.Util.Encriptar(item.ValorCusto.ToString()),
                                          item.ValorUnitario,
                                          item.ValorDesconto));
                }

                SaveFileDialog salvaOrcamento = new SaveFileDialog();
                salvaOrcamento.FileName = ((Contrato.Orcamento)((Button)e.Source).DataContext).Codigo;
                salvaOrcamento.Filter = "Documento texto (.csv)|*.csv";
                salvaOrcamento.FilterIndex = 2;
                salvaOrcamento.RestoreDirectory = true;

                if (salvaOrcamento.ShowDialog().Value)
                {
                    System.IO.File.WriteAllLines(salvaOrcamento.FileName, lstItensOrcamento, Encoding.Default);
                }
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
