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
    /// Interaction logic for Usuario.xaml
    /// </summary>
    public partial class WUsuarioCadastro : Window
    {
        #region "[Atributos]"

        private Contrato.Usuario _usuario = null;
        private bool _cancelou = false;
        private StringBuilder _strValidacao = new StringBuilder();

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

        public bool Cancelou
        {
            get 
            {
                return _cancelou;
            }
        }

        #endregion

        #region "[Metodos]"

        public WUsuarioCadastro()
        {
            InitializeComponent();
        }

        private void ValidarPermissao()
        {
            btnSalvar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_USUARIO, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_USUARIO, Comum.Constantes.PERMISSAO_MODIFICAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            txtNome.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_USUARIO, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_USUARIO, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtLogin.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_USUARIO, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_USUARIO, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtConfirmarSenha.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_USUARIO, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_USUARIO, Comum.Constantes.PERMISSAO_MODIFICAR);
            chkAtivo.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_USUARIO, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_USUARIO, Comum.Constantes.PERMISSAO_MODIFICAR);
            dgPerfis.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_USUARIO, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_USUARIO, Comum.Constantes.PERMISSAO_MODIFICAR);
        }

        private bool SalvarUsuario()
        {
            bool salvou = true;

            _strValidacao = ValidarCampos();

            // Verifica se as informações do usuário são válidas
            if (_strValidacao.Length > 0)
            {
                MessageBox.Show(_strValidacao.ToString(), "Usuário", MessageBoxButton.OK, MessageBoxImage.Information);
                salvou = false;
            }
            else
            {
                Contrato.EntradaUsuario entUsuario = new Contrato.EntradaUsuario();
                entUsuario.Chave = Comum.Util.Chave;
                entUsuario.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
                entUsuario.Usuario = new Contrato.Usuario();  

                if (_usuario == null)
                    entUsuario.Novo = true;
                else
                    entUsuario.Usuario.Id = _usuario.Id;                              

                PreencherDadosUsuario(entUsuario.Usuario);

                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
                Contrato.RetornoUsuario retUsuario = servBrasilDidaticos.UsuarioSalvar(entUsuario);
                servBrasilDidaticos.Close();

                if (retUsuario.Codigo != Contrato.Constantes.COD_RETORNO_SUCESSO)
                {
                    MessageBox.Show(retUsuario.Mensagem, "Usuario", MessageBoxButton.OK, MessageBoxImage.Error);
                    salvou = false;
                }
            }

            return salvou;
        }

        /// <summary>
        /// Valida os campos do formulário
        /// </summary>
        private StringBuilder ValidarCampos()
        {
            StringBuilder strValidacao = new StringBuilder();

            // Verifica se o Nome foi informado
            if (string.IsNullOrWhiteSpace(txtNome.Conteudo.ToString()))
            {
                txtNome.Erro = Visibility.Visible;
                strValidacao.Append("O campo 'Nome' não foi informado!\n");
            }
            else
                txtNome.Erro = Visibility.Hidden;

            // Verifica se o Login foi informado
            if (string.IsNullOrWhiteSpace(txtLogin.Conteudo.ToString()))
            {
                txtLogin.Erro = Visibility.Visible;
                strValidacao.Append("O campo 'Login' não foi informado!\n");
            }
            else
                txtLogin.Erro = Visibility.Hidden;

            // Verifica se a senha foi informada
            if ((_usuario != null && string.IsNullOrWhiteSpace(_usuario.Senha)) || (_usuario == null && string.IsNullOrWhiteSpace(txtSenha.Conteudo.ToString())))
            {
                txtSenha.Erro = Visibility.Visible;
                strValidacao.Append("O campo 'Senha' não foi informado!\n");
            }
            else
                txtSenha.Erro = Visibility.Hidden;

            // Verifica se a confirmação da senha foi informada
            if ((_usuario != null && string.IsNullOrWhiteSpace(_usuario.Senha)) || (_usuario == null && string.IsNullOrWhiteSpace(txtConfirmarSenha.Conteudo.ToString())))
            {
                txtConfirmarSenha.Erro = Visibility.Visible;
                strValidacao.Append("O campo 'ConfirmarSenha' não foi informado!\n");
            }
            else
                txtSenha.Erro = Visibility.Hidden;

            // Verifica se as senhas são diferentes
            if (!string.IsNullOrWhiteSpace(txtSenha.Conteudo.ToString()) &&
                !string.IsNullOrWhiteSpace(txtConfirmarSenha.Conteudo.ToString()) &&
                txtSenha.Conteudo != txtConfirmarSenha.Conteudo)
            {
                strValidacao.Append("Por favor, o informe o mesmo valor no campo 'Senha'!\n");
            }

            return strValidacao;
        }

        private void PreencherDadosUsuario(Contrato.Usuario Usuario)
        {
            // Preenche os dados do usuário
            Usuario.Nome = txtNome.Conteudo;
            Usuario.Login = txtLogin.Conteudo;
            if (!string.IsNullOrWhiteSpace(txtSenha.Conteudo))
                Usuario.Senha = Comum.Util.CriptografiaMD5(txtSenha.Conteudo);
            else
                Usuario.Senha = _usuario.Senha;
            Usuario.Ativo = (bool)chkAtivo.Selecionado;

            // Preenche os perfis do usuário
            PreencherDadosPerfis(Usuario);
        }

        private void PreencherDadosPerfis(Contrato.Usuario Usuario)
        {
            foreach (var item in dgPerfis.Items)
            {
                if (item.GetType() == typeof(Objeto.Perfil) && ((Objeto.Perfil)item).Selecionado == true)
                {
                    if (Usuario.Perfis == null)
                        Usuario.Perfis = new List<Contrato.Perfil>();

                    Usuario.Perfis.Add(new Contrato.Perfil() 
                    {
                        Id = ((Objeto.Perfil)item).Id,
                        Nome = ((Objeto.Perfil)item).Nome,
                        Codigo = ((Objeto.Perfil)item).Codigo,
                        Ativo = ((Objeto.Perfil)item).Ativo,
                        Permissoes = ((Objeto.Perfil)item).Permissoes,
                    });
                }
            }            
        }

        private void PreencherDadosTela()
        {
            if (_usuario != null)
            {
                Item.Header = Comum.Util.GroupHeader("Edição", "/BrasilDidaticos;component/Imagens/ico_editar.png");

                txtNome.Conteudo = _usuario.Nome;
                txtLogin.Conteudo = _usuario.Login;
                chkAtivo.Selecionado = _usuario.Ativo;
            }
        }

        private void ListarPerfis()
        {
            Contrato.EntradaPerfil entPerfil = new Contrato.EntradaPerfil();
            entPerfil.Chave = Comum.Util.Chave;
            entPerfil.Perfil = new Contrato.Perfil();
            entPerfil.Perfil.Ativo = true;
            entPerfil.UsuarioLogado = Comum.Util.UsuarioLogado.Login;

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
            Contrato.RetornoPerfil retPerfil = servBrasilDidaticos.PerfilListar(entPerfil);
            servBrasilDidaticos.Close();

            if (retPerfil.Codigo != Contrato.Constantes.COD_RETORNO_VAZIO)
            {

                List<Objeto.Perfil> objPerfis = null;
                
                if (_usuario != null && _usuario.Perfis != null)
                   objPerfis = (from p in retPerfil.Perfis
                                 select new Objeto.Perfil { Selecionado = (from pf in _usuario.Perfis where pf.Codigo == p.Codigo select pf).Count() > 0, Id = p.Id, Codigo = p.Codigo, Nome = p.Nome, Ativo = p.Ativo, Permissoes = p.Permissoes }).ToList();
                else
                    objPerfis = (from p in retPerfil.Perfis
                                 select new Objeto.Perfil { Selecionado = false, Id = p.Id, Codigo = p.Codigo, Nome = p.Nome, Ativo = p.Ativo, Permissoes = p.Permissoes }).ToList();

                dgPerfis.ItemsSource = objPerfis;

            }
        }

        private void Fechar()
        {
            if (_strValidacao.Length == 0)
                this.Close();
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
                ListarPerfis();
                txtNome.txtBox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Usuário", MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (SalvarUsuario())
                    this.Fechar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Usuário", MessageBoxButton.OK, MessageBoxImage.Error);
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
                this.Cursor = Cursors.Wait;
                _cancelou = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Usuário", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            } 
        }
        
        #endregion
                      
    }
}
