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
    /// Interaction logic for Principal.xaml
    /// </summary>
    public partial class WPrincipal : Window
    {
        #region "Atributos"

        bool _BuscarProduto = true;

        #endregion

        #region "[Metodos]"

        public WPrincipal()
        {
            InitializeComponent();            
        }

        private void ValidarPermissao()
        {
            // Permissão módulos operacionais sistema
            Orcamento.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_CONSULTAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            Cliente.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CONSULTAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            Fornecedor.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_CONSULTAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            Produto.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_CONSULTAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            Taxa.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_CONSULTAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            // Permissão módulos administrativos
            Usuario.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_USUARIO, Comum.Constantes.PERMISSAO_CONSULTAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            Perfil.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PERFIL, Comum.Constantes.PERMISSAO_CONSULTAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            Parametro.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PARAMETRO, Comum.Constantes.PERMISSAO_CONSULTAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            // Permissão Relatórios
            Atacado.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.RELATORIO_ATACADO, Comum.Constantes.PERMISSAO_CONSULTAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            Varejo.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.RELATORIO_VAREJO, Comum.Constantes.PERMISSAO_CONSULTAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            // Permissão Valor Custo DataGrid
            DataGridColumn dgColuna = null;
            dgColuna = (from c in dgProdutos.Columns where c.Header.ToString() == "Custo" select c).FirstOrDefault();
            if (dgColuna != null) dgColuna.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRINCIPAL, Comum.Constantes.VER_CUSTO) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private void PreencherDadosFornecedores()
        {
            Contrato.EntradaFornecedor entradaFornecedor = new Contrato.EntradaFornecedor();
            entradaFornecedor.Chave = Comum.Util.Chave;
            entradaFornecedor.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaFornecedor.Fornecedor = new Contrato.Fornecedor() { Ativo = true };

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
            Contrato.RetornoFornecedor retFornecedor = servBrasilDidaticos.FornecedorListar(entradaFornecedor);
            servBrasilDidaticos.Close();

            if (retFornecedor.Fornecedores != null)
            {
                cmbFornecedor.ComboBox.Items.Clear();
                cmbFornecedor.ComboBox.Items.Add(new ComboBoxItem() { Uid = Guid.Empty.ToString(), Content = "Todos" });
                foreach (Contrato.Fornecedor fornecedor in retFornecedor.Fornecedores)
                {
                    cmbFornecedor.ComboBox.Items.Add(new ComboBoxItem() { Uid = fornecedor.Id.ToString(), Content = fornecedor.Nome, Tag = fornecedor });
                }
            }
        }        

        private void PreencherDadosTela()
        {
            PreencherDadosProdutos();
        }

        private void PreencherDadosProdutos()
        {
            PreencherDadosFornecedores();
            PreencherDadosProdutos(false);
        }

        private void PreencherDadosProdutos(bool mostrarMsgVazio)
        {
            dgProdutos.Items.Clear();

            Contrato.EntradaProduto entradaProduto = new Contrato.EntradaProduto();
            entradaProduto.Chave = Comum.Util.Chave;
            entradaProduto.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaProduto.Produto = new Contrato.Produto() { Ativo = true };
            entradaProduto.Paginar = true;
            entradaProduto.PosicaoUltimoItem = 0;
            entradaProduto.CantidadeItens = Comum.Parametros.QuantidadeItensPagina;

            PreencherFiltro(entradaProduto.Produto);

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
            Contrato.RetornoProduto retProduto = servBrasilDidaticos.ProdutoListar(entradaProduto);
            servBrasilDidaticos.Close();

            if (retProduto.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                List<Objeto.Produto> lstProdutos = (from p in retProduto.Produtos select new Objeto.Produto { Codigo = p.Codigo, Nome = p.Nome, CodigoFornecedor = p.CodigoFornecedor, Fornecedor = p.Fornecedor, Taxas = p.Taxas, ValorBase = p.ValorBase }).ToList();
                foreach (Objeto.Produto p in lstProdutos)
                    dgProdutos.Items.Add(p);
            }

            if (mostrarMsgVazio && retProduto.Codigo == Contrato.Constantes.COD_RETORNO_VAZIO)
                MessageBox.Show(retProduto.Mensagem, "Produto", MessageBoxButton.OK, MessageBoxImage.Information);

            txtCodigo.txtBox.Focus();
        }

        private void PreencherFiltro(Contrato.Produto Produto)
        {
            Produto.Codigo = txtCodigo.Conteudo;
            Produto.Nome = txtNome.Conteudo;
            Produto.CodigoFornecedor = txtCodigoFornecedor.Conteudo;
            if (cmbFornecedor.ValorSelecionado != null)
                Produto.Fornecedor = (Contrato.Fornecedor)cmbFornecedor.ValorSelecionado;
        }

        /// <summary>
        /// Fecha a aplicação
        /// </summary>
        private void Fechar()
        {
            // Chama o serviço para apagar a sessão do usuário da aplicação
            if (Comum.Util.UsuarioLogado != null)
            {
                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
                Contrato.RetornoSessao retSessao = servBrasilDidaticos.SessaoExcluir(new Contrato.Sessao() { Login = Comum.Util.UsuarioLogado.Login, Chave = Comum.Util.Chave });
                servBrasilDidaticos.Close();
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
                PreencherDadosTela();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Brasil Didáticos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }            
        }

        private void Orcamento_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WOrcamento wOrcamento = new WOrcamento();
                wOrcamento.Owner = this;
                wOrcamento.ShowDialog();
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

        private void Cliente_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WCliente wCliente = new WCliente();
                wCliente.Owner = this;
                wCliente.ShowDialog();
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

        private void Fornecedor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WFornecedor wFornecedor = new WFornecedor();
                wFornecedor.Owner = this;
                wFornecedor.ShowDialog();
                if (wFornecedor.Alterou)
                    PreencherDadosProdutos();
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

        private void Produto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WProduto wProduto = new WProduto();
                wProduto.Owner = this;
                wProduto.ShowDialog();
                if (wProduto.Alterou)
                    PreencherDadosProdutos();            
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Produto", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void wProduto_WindowClosingEvent(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (((WProduto)sender).Alterou)
                    PreencherDadosProdutos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Produto", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }            
        }

        private void Varejo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WFiltroRelatorioVarejo wRelatorioVarejo = new WFiltroRelatorioVarejo();
                wRelatorioVarejo.Owner = this;
                wRelatorioVarejo.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Relatório Varejo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void Atacado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WFiltroRelatorioAtacado wRelatorioAtacado = new WFiltroRelatorioAtacado();
                wRelatorioAtacado.Owner = this;
                wRelatorioAtacado.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Relatório Atacado", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void Taxa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WTaxa wTaxa = new WTaxa();
                wTaxa.Owner = this;
                wTaxa.ShowDialog();
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

        private void Usuario_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WUsuario wUsuario = new WUsuario();
                wUsuario.Owner = this;
                wUsuario.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Usuario", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void Perfil_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WPerfil wPerfil = new WPerfil();
                wPerfil.Owner = this;
                wPerfil.ShowDialog();
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

        private void Parametro_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WParametro wParametro = new WParametro();
                wParametro.Owner = this;
                wParametro.ShowDialog();
                if (!wParametro.Cancelou)
                    PreencherDadosProdutos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Parâmetro", MessageBoxButton.OK, MessageBoxImage.Error);
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
                _BuscarProduto = true;
                PreencherDadosProdutos(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Brasil Didáticos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void dgProdutos_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            try
            {
                if (e.VerticalChange != 0)
                {
                    if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight && _BuscarProduto)
                    {
                        Contrato.EntradaProduto entradaProduto = new Contrato.EntradaProduto();
                        entradaProduto.Chave = Comum.Util.Chave;
                        entradaProduto.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
                        entradaProduto.Produto = new Contrato.Produto() { Ativo = true };
                        entradaProduto.Paginar = true;
                        entradaProduto.PosicaoUltimoItem = int.Parse(e.ExtentHeight.ToString());
                        entradaProduto.CantidadeItens = int.Parse(e.ViewportHeight.ToString());
                    
                        PreencherFiltro(entradaProduto.Produto);

                        Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
                        Contrato.RetornoProduto retProduto = servBrasilDidaticos.ProdutoListar(entradaProduto);
                        servBrasilDidaticos.Close();

                        if (retProduto.Codigo == 0)
                        {
                            // Verifica se será necessário buscar mais produtos
                            _BuscarProduto = retProduto.Produtos.Count == e.ViewportHeight;
                            // Se existem produtos preenche o grid
                            if (retProduto.Produtos.Count > 0)
                            {
                                List<Objeto.Produto> lstProdutos = (from p in retProduto.Produtos select new Objeto.Produto { Codigo = p.Codigo, Nome = p.Nome, Fornecedor = p.Fornecedor, Taxas = p.Taxas, ValorBase = p.ValorBase }).ToList();
                                foreach (Objeto.Produto p in lstProdutos)
                                    dgProdutos.Items.Add(p);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Brasil Didáticos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                //Fechar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Brasil Didáticos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }                   

        #endregion                                
    }
}