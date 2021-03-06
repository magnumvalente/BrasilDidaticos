﻿using System;
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
using System.Configuration;

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

        private void ConfigurarControles()
        {
            this.Title = Comum.Util.UsuarioLogado != null ? Comum.Util.UsuarioLogado.Empresa.Nome : this.Title;
            this.BrasilDidaticos.Header = Comum.Util.UsuarioLogado != null ? Comum.Util.UsuarioLogado.Empresa.Nome : BrasilDidaticos.Header;
            this.txtCodigo.txtBox.Focus();
            this.Background = Comum.Util.ConfigurarCorFundoTela(this.Background);

            if (ConfigurationManager.AppSettings["BolAmbienteHomologacao"] != null && ConfigurationManager.AppSettings["BolAmbienteHomologacao"].ToLower() == "true")
                stpCabecalho.Visibility = System.Windows.Visibility.Visible;
        }

        private void ValidarPermissao()
        {
            // Permissão módulos operacionais sistema
            Orcamento.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            Pedido.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PEDIDO, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            Estoque.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ESTOQUE, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            Cliente.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            Fornecedor.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            Produto.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            Taxa.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            UnidadMedida.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_UNIDADE_MEDIDA, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnOrcamento.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ORCAMENTO, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnPedido.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PEDIDO, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnEstoque.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_ESTOQUE, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnCliente.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_CLIENTE, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnFornecedor.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_FORNECEDOR, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnProduto.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PRODUTO, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnTaxa.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnUnidadeMedida.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_UNIDADE_MEDIDA, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            
            // Permissão módulos administrativos
            Usuario.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_USUARIO, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            Perfil.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PERFIL, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            Parametro.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PARAMETRO, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnUsuario.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_USUARIO, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnPerfil.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PERFIL, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnParametro.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PARAMETRO, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            // Permissão Relatórios
            Atacado.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.RELATORIO_ATACADO, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            Varejo.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.RELATORIO_VAREJO, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnRelatorioAtacado.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.RELATORIO_ATACADO, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            btnRelatorioVarejo.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.RELATORIO_VAREJO, Comum.Constantes.PERMISSAO_CONSULTAR) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

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
            entradaFornecedor.EmpresaLogada = Comum.Parametros.EmpresaProduto;
            entradaFornecedor.PreencherListaSelecao = true;
            entradaFornecedor.Fornecedor = new Contrato.Fornecedor() { Ativo = true };

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoFornecedor retFornecedor = servBrasilDidaticos.FornecedorListar(entradaFornecedor);
            servBrasilDidaticos.Close();

            if (retFornecedor.Fornecedores != null)
            {
                cmbFornecedor.ComboBox.Items.Clear();
                cmbFornecedor.ComboBox.Items.Add(new ComboBoxItem() { Uid = Guid.Empty.ToString(), Content = "Todos" });
                foreach (Contrato.Fornecedor fornecedor in retFornecedor.Fornecedores.OrderBy(f => f.Nome))
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
            entradaProduto.EmpresaLogada = Comum.Parametros.EmpresaProduto;
            entradaProduto.Produto = new Contrato.Produto() { Ativo = true };
            entradaProduto.Paginar = true;
            entradaProduto.PosicaoUltimoItem = 0;
            entradaProduto.CantidadeItens = Comum.Parametros.QuantidadeItensPagina;

            PreencherFiltro(entradaProduto.Produto);

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoProduto retProduto = servBrasilDidaticos.ProdutoListar(entradaProduto);
            servBrasilDidaticos.Close();

            if (retProduto.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
            {
                List<Objeto.Produto> lstProdutos = (from p in retProduto.Produtos select new Objeto.Produto { Codigo = p.Codigo, Nome = p.Nome, Ncm = p.Ncm, ValorPercentagemAtacado = p.ValorPercentagemAtacado, ValorPercentagemVarejo = p.ValorPercentagemVarejo, CodigoFornecedor = p.CodigoFornecedor, Fornecedor = p.Fornecedor, Taxas = p.Taxas, Quantidade = p.Quantidade, ValorBase = p.ValorBase }).ToList();
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
                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
                Contrato.RetornoSessao retSessao = servBrasilDidaticos.SessaoExcluir(new Contrato.Sessao() { Login = Comum.Util.UsuarioLogado.Login, Chave = Comum.Util.Chave });
                servBrasilDidaticos.Close();
            }
        }

        private void Limpar()
        {
            txtCodigo.Conteudo = string.Empty;
            txtNome.Conteudo = string.Empty;
            txtCodigoFornecedor.Conteudo = string.Empty;
            cmbFornecedor.ValorSelecionado = null;
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
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void Pedido_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WPedido wPedido = new WPedido();
                wPedido.Owner = this;
                wPedido.ShowDialog();
                if (wPedido.Alterou)
                    PreencherDadosProdutos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }           

        private void Estoque_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WEstoque wEstoque = new WEstoque();
                wEstoque.Owner = this;
                wEstoque.ShowDialog();
                if (wEstoque.Alterou)
                    PreencherDadosProdutos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void UnidadeMedida_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WUnidadeMedida wTaxa = new WUnidadeMedida();
                wTaxa.Owner = this;
                wTaxa.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void btnEmail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WEmailEnvio wEmailEnvio = new WEmailEnvio();
                wEmailEnvio.Owner = this;
                wEmailEnvio.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
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
                {
                    ConfigurarControles();
                    PreencherDadosProdutos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void Sobre_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WSobre wSobre = new WSobre(this);
                wSobre.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
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
                        entradaProduto.EmpresaLogada = Comum.Parametros.EmpresaProduto;
                        entradaProduto.Produto = new Contrato.Produto() { Ativo = true };
                        entradaProduto.Paginar = true;
                        entradaProduto.PosicaoUltimoItem = int.Parse(e.ExtentHeight.ToString());
                        entradaProduto.CantidadeItens = int.Parse(e.ViewportHeight.ToString());
                    
                        PreencherFiltro(entradaProduto.Produto);

                        Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
                        Contrato.RetornoProduto retProduto = servBrasilDidaticos.ProdutoListar(entradaProduto);
                        servBrasilDidaticos.Close();

                        if (retProduto.Codigo == 0)
                        {
                            // Verifica se será necessário buscar mais produtos
                            _BuscarProduto = retProduto.Produtos.Count == e.ViewportHeight;
                            // Se existem produtos preenche o grid
                            if (retProduto.Produtos.Count > 0)
                            {
                                List<Objeto.Produto> lstProdutos = (from p in retProduto.Produtos select new Objeto.Produto { Codigo = p.Codigo, Nome = p.Nome, Ncm = p.Ncm, ValorPercentagemAtacado = p.ValorPercentagemAtacado, ValorPercentagemVarejo = p.ValorPercentagemVarejo, CodigoFornecedor = p.CodigoFornecedor, Fornecedor = p.Fornecedor, Taxas = p.Taxas, Quantidade = p.Quantidade, ValorBase = p.ValorBase }).ToList();
                                foreach (Objeto.Produto p in lstProdutos)
                                    dgProdutos.Items.Add(p);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), Comum.Util.UsuarioLogado.Empresa.Nome, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }                   

        #endregion 
                      
    }
}