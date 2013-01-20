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
    /// Interaction logic for Taxa.xaml
    /// </summary>
    public partial class WTaxaCadastro : Window
    {
        #region "[Atributos]"

        private Contrato.Taxa _taxa = null;
        private bool _cancelou = false;

        #endregion

        #region "[Propriedades]"

        public Contrato.Taxa Taxa
        {
            get 
            {
                return _taxa;
            }
            set 
            {
                _taxa = value;
            }
        }

        public bool Cancelou
        {
            get 
            {
                return _cancelou;
            }
        }

        #endregion

        #region "[Metodos]"

        public WTaxaCadastro()
        {
            InitializeComponent();
        }

        private void ValidarPermissao()
        {
            btnSalvar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_MODIFICAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            txtNome.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_MODIFICAR);
            chkFornecedor.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_MODIFICAR);
            chkProduto.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_MODIFICAR);
            chkDesconto.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_MODIFICAR);
            chkAtivo.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_MODIFICAR);
        }

        /// <summary>
        /// Valida os campos do formulário
        /// </summary>
        private StringBuilder ValidarCampos()
        {
            StringBuilder strValidacao = new StringBuilder();                       

            // Verifica se a Nome foi informada
            if (string.IsNullOrWhiteSpace(txtNome.Conteudo.ToString()))
            {
                txtNome.Erro = Visibility.Visible;
                strValidacao.Append("O campo 'Nome' não foi informado!\n");
            }
            else
                txtNome.Erro = Visibility.Hidden;           

            return strValidacao;
        }

        private bool SalvarTaxa()
        {
            bool salvou = true;

            StringBuilder strValidacao = ValidarCampos();

            // Verifica se as informações do usuário são válidas
            if (strValidacao.Length > 0)
            {
                MessageBox.Show(strValidacao.ToString(), "Produto", MessageBoxButton.OK, MessageBoxImage.Information);
                salvou = false;
            }
            else
            {
                Contrato.EntradaTaxa entradaTaxa = new Contrato.EntradaTaxa();
                entradaTaxa.Chave = Comum.Util.Chave;
                entradaTaxa.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
                if (_taxa == null) entradaTaxa.Novo = true;
                entradaTaxa.Taxa = new Contrato.Taxa();

                PreencherDados(entradaTaxa.Taxa);

                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
                Contrato.RetornoTaxa retTaxa = servBrasilDidaticos.TaxaSalvar(entradaTaxa);
                servBrasilDidaticos.Close();

                if (retTaxa.Codigo != Contrato.Constantes.COD_RETORNO_SUCESSO)
                {
                    MessageBox.Show(retTaxa.Mensagem, "Taxa", MessageBoxButton.OK, MessageBoxImage.Error);
                    salvou = false;
                }
            }

            return salvou;
        }

        private void PreencherDados(Contrato.Taxa Taxa)
        {            
            Taxa.Nome = txtNome.Conteudo;
            Taxa.Fornecedor = (bool)chkFornecedor.Selecionado;
            Taxa.Produto = (bool)chkProduto.Selecionado;
            Taxa.Desconto = (bool)chkDesconto.Selecionado;
            Taxa.Ativo = (bool)chkAtivo.Selecionado;
        }

        private void PreencherDadosTela()
        {
            if (_taxa != null)
            {
                Item.Header = Comum.Util.GroupHeader("Edição", "/BrasilDidaticos;component/Imagens/ico_editar.png");

                txtNome.Conteudo = _taxa.Nome;
                chkFornecedor.Selecionado = _taxa.Fornecedor;
                chkProduto.Selecionado = _taxa.Produto;
                chkDesconto.Selecionado = _taxa.Desconto;
                chkAtivo.Selecionado = _taxa.Ativo;
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
                txtNome.txtBox.Focus();
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

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                if (SalvarTaxa())
                    this.Close();
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

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _cancelou = true;
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