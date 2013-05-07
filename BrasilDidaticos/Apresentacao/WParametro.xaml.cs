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
    /// Interaction logic for WParametro.xaml
    /// </summary>
    public partial class WParametro : Window
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

        public WParametro()
        {
            InitializeComponent();
        }

        private void ConfigurarControles()
        {
            this.Title = Comum.Util.UsuarioLogado != null ? Comum.Util.UsuarioLogado.Empresa.Nome : this.Title;
        }

        private void ValidarPermissao()
        {
            // Permissão módulos operacionais sistema
            btnGravar.Visibility = Comum.Util.ValidarPermissao(Comum.Constantes.TELA_PARAMETRO, Comum.Constantes.PERMISSAO_MODIFICAR) == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;            
        }

        private void ListarParametros()
        {
            ListarParametros(false);
        }

        private void ListarParametros(bool mostrarMsgVazio)
        {
            Contrato.EntradaParametro entParametro = new Contrato.EntradaParametro();
            entParametro.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entParametro.Chave = Comum.Util.Chave;
            entParametro.EmpresaLogada = Comum.Util.UsuarioLogado.Empresa;
            entParametro.Parametro = new Contrato.Parametro();            

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoParametro retParametro = servBrasilDidaticos.ParametroListar(entParametro);
            servBrasilDidaticos.Close();

            if (retParametro.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO && retParametro.Parametros.Count > 0)
            {
                // Create Columns
                ColumnDefinition gridCol1 = new ColumnDefinition();
                dgParametros.ColumnDefinitions.Add(gridCol1);

                bool definiuFoco = false;

                foreach (Contrato.Parametro p in retParametro.Parametros.OrderBy(p => p.Ordem))
                {
                    // Create Rows
                    RowDefinition gridRow = new RowDefinition();
                    gridRow.Height = new GridLength(30);
                    dgParametros.RowDefinitions.Add(gridRow);
                    UserControl controler = null;
                    controler = Comum.Util.CriarControler(p.Nome, p.Valor, 200, p.TipoParametro, p);
                    Grid.SetRow(controler, dgParametros.RowDefinitions.Count - 1);
                    Grid.SetColumn(controler, 0);
                    if (!definiuFoco)
                    {
                        controler.Focus();
                        definiuFoco = true;
                    }
                    dgParametros.Children.Add(controler);
                }
            }
            else
            {
                MessageBox.Show("Não existem parâmetros cadastros!", "Parâmetros", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }

            if (mostrarMsgVazio && retParametro.Codigo == Contrato.Constantes.COD_RETORNO_VAZIO)
                MessageBox.Show(retParametro.Mensagem, "Parâmetros", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private void SalvarParametros()
        {
            Contrato.EntradaParametros entradaParametros = new Contrato.EntradaParametros();            
            entradaParametros.Chave = Comum.Util.Chave;
            entradaParametros.UsuarioLogado = Comum.Util.UsuarioLogado.Login;
            entradaParametros.EmpresaLogada = Comum.Util.UsuarioLogado.Empresa;
            entradaParametros.Parametros = new List<Contrato.Parametro>();

            PreencherParametros(entradaParametros.Parametros);

            Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient(Comum.Util.RecuperarNomeEndPoint());
            Contrato.RetornoParametro retParametro = servBrasilDidaticos.ParametrosSalvar(entradaParametros);
            servBrasilDidaticos.Close();

            Comum.Parametros.CarregarParametros();

            if (retParametro.Codigo != Contrato.Constantes.COD_RETORNO_SUCESSO || retParametro.Mensagem != null)
                MessageBox.Show(retParametro.Mensagem, "Parâmetro", MessageBoxButton.OK, MessageBoxImage.Error);               
        }
        
        private void PreencherParametros(List<Contrato.Parametro> Parametros)
        {
            foreach (var item in dgParametros.Children)
            {
                if (item != null)
                {
                    if (Parametros == null)
                        Parametros = new List<Contrato.Parametro>();

                    Contrato.Parametro parametro = ((Contrato.Parametro)((UserControl)item).Tag);

                    switch (parametro.TipoParametro)
                    { 
                        case Contrato.Enumeradores.TipoParametro.Texto:                        
                            parametro.Valor = ((Controler.MTextBox)item).Conteudo;
                            break;
                        case Contrato.Enumeradores.TipoParametro.Percentagem:
                            decimal d = (decimal)((Controler.MDecimalTextBox)item).Valor * 100;
                            parametro.Valor = d.ToString();
                            break;
                        case Contrato.Enumeradores.TipoParametro.Inteiro:
                            int i = (int)((Controler.MDecimalTextBox)item).Valor;
                            parametro.Valor = i.ToString();
                            break;
                        case Contrato.Enumeradores.TipoParametro.Cor:
                            parametro.Valor = ((Controler.MColorPicker)item).Conteudo;
                            break;
                    }

                    Parametros.Add(parametro);
                }
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
                this.ListarParametros();
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

        private void btnGravar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                SalvarParametros();
                this.Close();
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

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this._cancelou = true;
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

        #endregion
        
    }
}
