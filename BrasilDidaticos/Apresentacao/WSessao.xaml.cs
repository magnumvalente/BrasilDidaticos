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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BrasilDidaticos.Contrato;

namespace BrasilDidaticos.Apresentacao
{
    /// <summary>
    /// Interaction logic for Sessao.xaml
    /// </summary>
    public partial class WSessao : Window
    {
        #region "Atributos"

        // Usuário logado
        private Contrato.Usuario _usuario = null;
        // Sessão do usuário
        private Contrato.Sessao _sessao = null;
        private bool _sessaoDesbloqueada = false;

        #endregion

        #region "[Propriedades]"

        public Contrato.Usuario Usuario
        {
            get
            {
                return _usuario;
            }
            set
            {
                _usuario = value;
            }
        }
        public Contrato.Sessao Sessao
        {
            get
            {
                return _sessao;
            }
            set
            {
                _sessao = value;
            }
        }
        public bool SessaoDesbloqueada
        {
            get
            {
                return _sessaoDesbloqueada;
            }
            set
            {
                _sessaoDesbloqueada = value;
            }
        }

        #endregion 

        #region "[Metodos]"

        /// <summary>
        /// Instancia a página de Sessão
        /// </summary>
        public WSessao()
        {
            InitializeComponent();
            txtLogin.txtBox.Focus();
        }

        private void PreencherTela()
        {
            this.Title = _usuario != null ? _usuario.Empresa.Nome : this.Title;
        }

        /// <summary>
        /// Valida os campos do formulário
        /// </summary>
        private StringBuilder ValidarCampos() 
        {
            StringBuilder strValidacao = new StringBuilder();

            // Verifica se o Login foi informado
            if (string.IsNullOrWhiteSpace(txtLogin.Conteudo.ToString()))
            {
                txtLogin.Erro = Visibility.Visible;
                strValidacao.Append("O campo 'Login' não foi informado!\n" );
            }
            else
                txtLogin.Erro = Visibility.Hidden;

            // Verifica se a senha foi informada
            if (string.IsNullOrWhiteSpace(txtSenha.Conteudo.ToString()))
            {
                txtSenha.Erro = Visibility.Visible;
                strValidacao.Append("O campo 'Senha' não foi informado!\n");
            }
            else
                txtSenha.Erro = Visibility.Hidden;

            return strValidacao;
        }

        /// <summary>
        /// Loga o usuário na aplicação
        /// </summary>
        private void DesbloquearUsuario()
        { 
            StringBuilder strValidacao = ValidarCampos();

            // Verifica se as informações do usuário são válidas
            if (strValidacao.Length > 0)
            {
                MessageBox.Show(strValidacao.ToString(), "Sessão", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Define os dados do Login
                Contrato.EntradaUsuario usuario = new Contrato.EntradaUsuario();
                usuario.Usuario = new Contrato.Usuario();
                usuario.Usuario.Login = txtLogin.Conteudo;
                usuario.Usuario.Senha = Comum.Util.CriptografiaMD5(txtSenha.Conteudo);
                usuario.Chave = Comum.Util.Chave;

                // Chama o serviço para logar na aplicação
                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
                Contrato.RetornoUsuario retUsuario = servBrasilDidaticos.UsuarioLogar(usuario);
                servBrasilDidaticos.Close();                

                // Verifica se o usuário logou com sucesso
                if (retUsuario.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO || retUsuario.Codigo == Contrato.Constantes.COD_REGISTRO_DUPLICADO)
                {
                    // Se o usuáio possui permissão para desbloqueio
                    if (_sessao != null && 
                        Comum.Util.ValidarPermissao(retUsuario.Usuarios.First(), Comum.Constantes.TELA_SESSAO, Comum.Constantes.PERMISSAO_DESBLOQUEAR_USUARIO) && 
                        retUsuario.Usuarios.First().Empresa.Id == _usuario.Empresa.Id)
                    {
                        servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
                        servBrasilDidaticos.SessaoExcluir(new Contrato.Sessao() { Login = _sessao.Login, Chave = _sessao.Chave });
                        servBrasilDidaticos.Close();
                        SessaoDesbloqueada = true;

                        // Se está duplicado significa que o usuário já está logado, então não apaga a sua sessão
                        if (retUsuario.Codigo != Contrato.Constantes.COD_REGISTRO_DUPLICADO)
                        {
                            // Chama o serviço para apagar a sessão do usuário que possui permissão para o desbloqueio
                            servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
                            servBrasilDidaticos.SessaoExcluir(new Contrato.Sessao() { Login = usuario.Usuario.Login, Chave = usuario.Chave });
                            servBrasilDidaticos.Close();
                        }

                        // Fecha a tela de Sessão
                        this.Close();
                    }
                }

                if (!SessaoDesbloqueada)
                {
                    MessageBox.Show(string.Format("Não foi possível desbloquear a sessão do usuário!\nPor favor, verifique o usuário ou a senha informada!", txtLogin.Conteudo), "Sessão", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }        

        #endregion

        #region "[Eventos]"

        private void frmLogin_Loaded(object sender, RoutedEventArgs e)
        {
            this.PreencherTela();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Sessão", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }
                
        private void btnDesbloquear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;                                
                this.DesbloquearUsuario();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Sessão", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;                
            }
        }

        #endregion
        
    }
}
