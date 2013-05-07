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
    /// Interaction logic for WUnidadeMedidaCadastro.xaml
    /// </summary>
    public partial class WUnidadeMedidaCadastro : Window
    {
        #region "[Atributos]"

        private Contrato.UnidadeMedida _unidadeMedida = null;
        private bool _cancelou = false;

        #endregion

        #region "[Propriedades]"

        public Contrato.UnidadeMedida UnidadeMedida
        {
            get 
            {
                return _unidadeMedida;
            }
            set 
            {
                _unidadeMedida = value;
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

        public WUnidadeMedidaCadastro()
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
            btnSalvar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_UNIDADE_MEDIDA, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_MODIFICAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            txtCodigo.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_UNIDADE_MEDIDA, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtNome.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_UNIDADE_MEDIDA, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_MODIFICAR);
            chkAtivo.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_UNIDADE_MEDIDA, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_MODIFICAR);
        }

        /// <summary>
        /// Valida os campos do formulário
        /// </summary>
        private StringBuilder ValidarCampos()
        {
            StringBuilder strValidacao = new StringBuilder();

            // Verifica se o Código foi informado
            if (string.IsNullOrWhiteSpace(txtCodigo.Conteudo.ToString()))
            {
                txtCodigo.Erro = Visibility.Visible;
                strValidacao.Append("O campo 'Código' não foi informado!\n");
            }
            else
                txtCodigo.Erro = Visibility.Hidden;

            // Verifica se o Nome foi informado
            if (string.IsNullOrWhiteSpace(txtNome.Conteudo.ToString()))
            {
                txtNome.Erro = Visibility.Visible;
                strValidacao.Append("O campo 'Nome' não foi informado!\n");
            }
            else
                txtNome.Erro = Visibility.Hidden;           

            return strValidacao;
        }

        private bool SalvarUnidadeMedida()
        {
            bool salvou = true;

            StringBuilder strValidacao = ValidarCampos();

            // Verifica se as informações do usuário são válidas
            if (strValidacao.Length > 0)
            {
                MessageBox.Show(strValidacao.ToString(), "Unidade de Medida", MessageBoxButton.OK, MessageBoxImage.Information);
                salvou = false;
            }
            else
            {
                Contrato.EntradaUnidadeMedida entradaUnidadeMedida = new Contrato.EntradaUnidadeMedida();
                entradaUnidadeMedida.Chave = Comum.Util.Chave;
                entradaUnidadeMedida.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
                entradaUnidadeMedida.EmpresaLogada = Comum.Util.UsuarioLogado.Empresa;
                if (_unidadeMedida == null) entradaUnidadeMedida.Novo = true;
                entradaUnidadeMedida.UnidadeMedida = new Contrato.UnidadeMedida();

                PreencherDados(entradaUnidadeMedida.UnidadeMedida);

                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
                Contrato.RetornoUnidadeMedida retUnidadeMedida = servBrasilDidaticos.UnidadeMedidaSalvar(entradaUnidadeMedida);
                servBrasilDidaticos.Close();

                if (retUnidadeMedida.Codigo != Contrato.Constantes.COD_RETORNO_SUCESSO)
                {
                    MessageBox.Show(retUnidadeMedida.Mensagem, "UnidadeMedida", MessageBoxButton.OK, MessageBoxImage.Error);
                    salvou = false;
                }
            }

            return salvou;
        }

        private void PreencherDados(Contrato.UnidadeMedida UnidadeMedida)
        {
            UnidadeMedida.Codigo = txtCodigo.Conteudo;
            UnidadeMedida.Nome = txtNome.Conteudo;
            UnidadeMedida.Descricao = txtDescricao.Conteudo;
            UnidadeMedida.Ativo = (bool)chkAtivo.Selecionado;
        }

        private void PreencherDadosTela()
        {
            if (_unidadeMedida != null)
            {
                Item.Header = Comum.Util.GroupHeader("Edição", "/BrasilDidaticos;component/Imagens/ico_editar.png");

                txtCodigo.Conteudo = _unidadeMedida.Codigo;
                txtNome.Conteudo = _unidadeMedida.Nome;
                txtDescricao.Conteudo = _unidadeMedida.Descricao;
                chkAtivo.Selecionado = _unidadeMedida.Ativo;
            }
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
                MessageBox.Show(ex.ToString(), "Unidade de Medida", MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (SalvarUnidadeMedida())
                    this.Close();
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

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _cancelou = true;
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