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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class WLogin : Window
    {

        #region "[Metodos]"
        
        /// <summary>
        /// Instancia a página de Login
        /// </summary>
        public WLogin()
        {
            InitializeComponent();
            txtLogin.txtBox.Focus();

#if (DEBUG)
            txtLogin.Conteudo = "Magnum";
            txtSenha.Conteudo = "1";
#endif

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
        private void Logar()
        { 
            StringBuilder strValidacao = ValidarCampos();

            // Verifica se as informações do usuário são válidas
            if (strValidacao.Length > 0)
            {
                MessageBox.Show(strValidacao.ToString(), "Login", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Define se a sessão foi desbloqueada. Por padrão sempre é verdadeiro
                bool sessaoDesbloqueda = false;
                
                // Define os dados do Login
                Contrato.EntradaUsuario usuario = new Contrato.EntradaUsuario();
                usuario.Usuario = new Contrato.Usuario();
                usuario.Usuario.Login = txtLogin.Conteudo;
                usuario.Usuario.Senha = Comum.Util.CriptografiaMD5(txtSenha.Conteudo);
                usuario.Chave = Comum.Util.Chave;

                // Chama o serviço para logar na aplicação
                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
                Contrato.RetornoUsuario retUsuario = servBrasilDidaticos.UsuarioLogar(usuario);
                servBrasilDidaticos.Close();

                // Verifica se o usuário já está logado
                if (retUsuario.Codigo == Contrato.Constantes.COD_REGISTRO_DUPLICADO)
                {
                    MessageBox.Show(string.Format("Sessão Bloqueada!\nPor favor, solicite ao responsável o desbloqueio de sua sessão!", txtLogin.Conteudo), "Login", MessageBoxButton.OK, MessageBoxImage.Information);

                    servBrasilDidaticos = new Servico.BrasilDidaticosClient();
                    Contrato.RetornoSessao retSessao = servBrasilDidaticos.SessaoListar(new Contrato.Sessao() { Login = usuario.Usuario.Login });
                    servBrasilDidaticos.Close();

                    WSessao wSessao = new WSessao();
                    wSessao.Sessao = retSessao.Sessoes.FirstOrDefault();
                    wSessao.ShowDialog();
                    sessaoDesbloqueda = wSessao.SessaoDesbloqueada;
                }

                // Verifica se o usuário logou com sucesso
                if (retUsuario.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO || sessaoDesbloqueda)
                {
                    // Guarda os dados do usuário Logado
                    Comum.Util.UsuarioLogado = retUsuario.Usuarios.First();
                    // Esconde a tela de login
                    this.Visibility = System.Windows.Visibility.Hidden;
                    // Recupera os parâmetros
                    Comum.Parametros.CarregarParametros();
                    // Entra na tela principal
                    WPrincipal wPrincipal = new WPrincipal();
                    wPrincipal.ShowDialog();
                }
                else if (retUsuario.Codigo == Contrato.Constantes.COD_RETORNO_VAZIO)
                {
                    MessageBox.Show(string.Format("Não foi possível entrar na aplicação!\nPor favor, verifique o usuário ou a senha informada!", txtLogin.Conteudo), "Login", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
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

            this.Close();
        }

        #endregion

        #region "[Eventos]"

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                this.Fechar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Login", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }
                
        private void btnEntrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;                                
                this.Logar();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Login", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (Comum.Util.UsuarioLogado != null)
                    this.Fechar();

                this.Cursor = Cursors.Arrow;                
            }
        }

        #endregion
    }
}
