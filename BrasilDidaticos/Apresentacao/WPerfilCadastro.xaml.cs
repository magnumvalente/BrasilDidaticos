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
    /// Interaction logic for Perfil.xaml
    /// </summary>
    public partial class WPerfilCadastro : Window
    {
        #region "[Atributos]"

        private Contrato.Perfil _perfil = null;
        private bool _cancelou = false;
        private StringBuilder _strValidacao = new StringBuilder();

        #endregion

        #region "[Propriedades]"

        public Contrato.Perfil Perfil
        {
            get 
            {
                return _perfil;
            }
            set 
            {
                _perfil = value;
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

        public WPerfilCadastro()
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
            btnSalvar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PERFIL, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PERFIL, Comum.Constantes.PERMISSAO_MODIFICAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            txtCodigo.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PERFIL, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PERFIL, Comum.Constantes.PERMISSAO_MODIFICAR);
            txtNome.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PERFIL, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PERFIL, Comum.Constantes.PERMISSAO_MODIFICAR);
            chkAtivo.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PERFIL, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PERFIL, Comum.Constantes.PERMISSAO_MODIFICAR);
            dgPermissoes.IsEnabled = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PERFIL, Comum.Constantes.PERMISSAO_CRIAR) == true || Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PERFIL, Comum.Constantes.PERMISSAO_MODIFICAR);
        }

        private bool SalvarPerfil()
        {
            bool salvou = true;

            _strValidacao = ValidarCampos();

            // Verifica se as informações do usuário são válidas
            if (_strValidacao.Length > 0)
            {
                MessageBox.Show(_strValidacao.ToString(), "Perfil", MessageBoxButton.OK, MessageBoxImage.Information);
                salvou = false;
            }
            else
            {
                Contrato.EntradaPerfil entPerfil = new Contrato.EntradaPerfil();
                entPerfil.Chave = Comum.Util.Chave;
                entPerfil.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
                entPerfil.EmpresaLogada = Comum.Util.UsuarioLogado.Empresa;
                if (_perfil == null) entPerfil.Novo = true;
                entPerfil.Perfil = new Contrato.Perfil();                

                PreencherDadosPerfil(entPerfil.Perfil);

                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
                Contrato.RetornoPerfil retPerfil = servBrasilDidaticos.PerfilSalvar(entPerfil);
                servBrasilDidaticos.Close();

                if (retPerfil.Codigo != Contrato.Constantes.COD_RETORNO_SUCESSO)
                {
                    MessageBox.Show(retPerfil.Mensagem, "Perfil", MessageBoxButton.OK, MessageBoxImage.Error);
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

            // Verifica se o Login foi informado
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

        private void PreencherDadosPerfil(Contrato.Perfil Perfil)
        {
            // Preenche os dados do usuário
            Perfil.Id = _perfil == null ? Guid.Empty : _perfil.Id;
            Perfil.Codigo = txtCodigo.Conteudo;
            Perfil.Nome = txtNome.Conteudo;
            Perfil.Ativo = (bool)chkAtivo.Selecionado;

            // Preenche as permissões do perfil
            PreencherDadosPermissoes(Perfil);
        }

        private void PreencherDadosPermissoes(Contrato.Perfil Perfil)
        {
            foreach (var item in dgPermissoes.Items)
            {
                if (item.GetType() == typeof(Objeto.Permissao) && ((Objeto.Permissao)item).Selecionado == true)
                {
                    if (Perfil.Permissoes == null)
                        Perfil.Permissoes = new List<Contrato.Permissao>();

                    Perfil.Permissoes.Add(new Contrato.Permissao() 
                    {
                        Id = ((Objeto.Permissao)item).Id,
                        Nome = ((Objeto.Permissao)item).Nome,
                        Ativo = ((Objeto.Permissao)item).Ativo
                    });
                }
            }            
        }

        private void PreencherDadosTela()
        {
            if (_perfil != null)
            {
                Item.Header = Comum.Util.GroupHeader("Edição", "/BrasilDidaticos;component/Imagens/ico_editar.png");

                txtCodigo.Conteudo = _perfil.Codigo;
                txtNome.Conteudo = _perfil.Nome;
                chkAtivo.Selecionado = _perfil.Ativo;
            }
        }

        private void ListarPermissoes()
        {
            Contrato.EntradaPermissao entPermissao = new Contrato.EntradaPermissao();
            entPermissao.Chave = Comum.Util.Chave;
            entPermissao.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entPermissao.Permissao = new Contrato.Permissao();
            entPermissao.Permissao.Ativo = true;            

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoPermissao retPermissao = servBrasilDidaticos.PermissaoListar(entPermissao);
            servBrasilDidaticos.Close();

            if (retPermissao.Codigo != Contrato.Constantes.COD_RETORNO_VAZIO)
            {

                List<Objeto.Permissao> objPermissoes = null;
                
                if (_perfil != null && _perfil.Permissoes != null)
                   objPermissoes = (from p in retPermissao.Permissoes
                                    select new Objeto.Permissao { Selecionado = (from pm in _perfil.Permissoes where pm.Nome == p.Nome select pm).Count() > 0, Id = p.Id, Nome = p.Nome, Ativo = p.Ativo, }).ToList();
                else
                    objPermissoes = (from p in retPermissao.Permissoes
                                     select new Objeto.Permissao { Selecionado = false, Id = p.Id, Nome = p.Nome, Ativo = p.Ativo}).ToList();

                dgPermissoes.ItemsSource = objPermissoes;

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
                this.ConfigurarControles();
                this.ValidarPermissao();
                this.PreencherDadosTela();
                this.ListarPermissoes();
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

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                if (SalvarPerfil())
                    this.Fechar();
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

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                _cancelou = true;
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

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
            {
                if (!cell.IsFocused)
                {
                    cell.Focus();
                }
                DataGrid dataGrid = Comum.Util.FindVisualParent<DataGrid>(cell);
                if (dataGrid != null)
                {
                    if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                    {
                        if (!cell.IsSelected)
                            cell.IsSelected = true;
                    }
                    else
                    {
                        DataGridRow row = Comum.Util.FindVisualParent<DataGridRow>(cell);
                        if (row != null && !row.IsSelected)
                        {
                            row.IsSelected = true;
                        }
                    }
                }
            }
        }
        
        #endregion                      
    }
}
