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
using System.Net.Mail;
using System.Net;

namespace BrasilDidaticos.Apresentacao
{
    /// <summary>
    /// Interaction logic for WEmailEnvio.xaml
    /// </summary>
    public partial class WEmailEnvio : Window
    {
        #region "[Atributos]"

        private bool _cancelou = false;

        #endregion

        #region "[Propriedades]"

        public bool Cancelou
        {
            get 
            {
                return _cancelou;
            }
        }

        #endregion

        #region "[Metodos]"

        public WEmailEnvio()
        {
            InitializeComponent();
        }

        private void ConfigurarControles()
        {
            this.Title = Comum.Util.UsuarioLogado != null ? Comum.Util.UsuarioLogado.Empresa.Nome : this.Title;
            this.txtAssunto.txtBox.Focus();
        }

        private void ValidarPermissao()
        {
            //btnEnviar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_UNIDADE_MEDIDA, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_MODIFICAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            //txtPara.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_UNIDADE_MEDIDA, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_MODIFICAR);
            //txtAssunto.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_UNIDADE_MEDIDA, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_MODIFICAR);
            //txtDescricao.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_UNIDADE_MEDIDA, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_TAXA, Comum.Constantes.PERMISSAO_MODIFICAR);
        }

        /// <summary>
        /// Valida os campos do formulário
        /// </summary>
        private StringBuilder ValidarCampos()
        {
            StringBuilder strValidacao = new StringBuilder();

            // Verifica se o Assunto foi informado
            if (string.IsNullOrWhiteSpace(txtAssunto.Conteudo.ToString()))
            {
                txtAssunto.Erro = Visibility.Visible;
                strValidacao.Append("O campo 'Assunto' não foi informado!\n");
            }
            else
                txtAssunto.Erro = Visibility.Hidden;

            // Verifica se o Para foi informado
            if (string.IsNullOrWhiteSpace(txtPara.Conteudo.ToString()))
            {
                txtPara.Erro = Visibility.Visible;
                strValidacao.Append("O campo 'Para' não foi informado!\n");
            }
            else
                txtPara.Erro = Visibility.Hidden;

            // Verifica se o Descrição foi informado
            if (string.IsNullOrWhiteSpace(txtDescricao.Conteudo.ToString()))
            {
                txtDescricao.Erro = Visibility.Visible;
                strValidacao.Append("O campo 'Descrição' não foi informado!\n");
            }
            else
                txtPara.Erro = Visibility.Hidden;       

            return strValidacao;
        }

        private bool EnviarEmail()
        {
            bool salvou = true;

            StringBuilder strValidacao = ValidarCampos();

            // Verifica se as informações do usuário são válidas
            if (strValidacao.Length > 0)
            {
                MessageBox.Show(strValidacao.ToString(), "Email", MessageBoxButton.OK, MessageBoxImage.Information);
                salvou = false;
            }
            else
            {
               SmtpClient cliente = new SmtpClient("smtp.gmail.com", 587 /* TLS */);
               cliente.EnableSsl = true;
         
               MailAddress remetente = new MailAddress("brasildidaticos@gmail.com", Comum.Util.UsuarioLogado.Empresa.Nome);
               MailAddress destinatario = new MailAddress(txtPara.Conteudo);
               MailMessage mensagem = new MailMessage(remetente, destinatario);

               mensagem.Subject = txtAssunto.Conteudo;
               mensagem.Body = txtDescricao.Conteudo;
                        
               NetworkCredential credenciais = new NetworkCredential(
                 "brasildidaticos@gmail.com", /* login */
                 "219021bd", /* senha */
                 "");
         
               cliente.Credentials = credenciais;         
               cliente.Send(mensagem);
            }

            return salvou;
        }

        private void PreencherDados()
        {
            
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Email", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }            
        }

        private void btnEnviar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                if (EnviarEmail())
                    this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Email", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), "Email", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            } 
        }

        #endregion                       
    }
}