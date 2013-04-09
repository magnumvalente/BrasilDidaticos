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

namespace BrasilDidaticos.Apresentacao.Controler
{
    /// <summary>
    /// Interaction logic for MBusca.xaml
    /// </summary>
    public partial class MBusca : UserControl
    {
        public delegate void BuscaClick(object sender, RoutedEventArgs e);
        public event BuscaClick BuscaClickEvent;

        public delegate void CodigoLostFocus(object sender, RoutedEventArgs e);
        public event CodigoLostFocus CodigoLostFocusEvent;

        public delegate void NomeLostFocus(object sender, RoutedEventArgs e);
        public event NomeLostFocus NomeGotFocusEvent;
                
        #region "[Propriedades]"

        public string Titulo
        {
            set
            {
                lblTitulo.Content = value;
            }
        }

        public string Codigo
        {
            set
            {
                txtCodigo.Text = value;
            }
            get
            {
                return txtCodigo.Text;
            }
        }

        public string Nome
        {
            set 
            { 
                txtNome.Text =  value;
            }
            get
            {
                return txtNome.Text;
            }
        }

        public Visibility Erro
        {
            set
            {
                lblCheckBoxErro.Visibility = value;
            }
            get
            {
                return lblCheckBoxErro.Visibility;
            }
        }

        public double WidthTitulo
        {
            set
            {
                gdControle.ColumnDefinitions[0].Width = new GridLength(value);
            }
            get
            {
                return gdControle.ColumnDefinitions[0].Width.Value;
            }
        }

        public double WidthConteudo
        {
            set
            {
                gdControle.ColumnDefinitions[1].Width = new GridLength(value);
            }
            get
            {
                return gdControle.ColumnDefinitions[1].Width.Value;
            }
        }

        public int TamanhoTexto
        {
            set
            { 
                txtNome.MaxLength = value; 
            }
            get 
            {
                return txtNome.MaxLength;
            }
        }

        public Guid Id
        {
            get;
            set;
        }

        #endregion

        #region "[Metodos]"

        public MBusca()
        {
            InitializeComponent();
        }

        public void Limpar()
        {
            Codigo = string.Empty;
            Nome = string.Empty;
        }

        #endregion

        #region "[Eventos]"

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (BuscaClickEvent != null)
                BuscaClickEvent(sender, e);
        }        

        private void txtCodigo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CodigoLostFocusEvent != null)
                CodigoLostFocusEvent(sender, e);
        }

        private void txtNome_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NomeGotFocusEvent != null)
                NomeGotFocusEvent(sender, e);
        }

        #endregion        
    }
}
