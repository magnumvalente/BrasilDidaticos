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
    /// Interaction logic for MTextBox.xaml
    /// </summary>
    public partial class MTextBox : UserControl
    {
        #region "[Propriedades]"

        public string Titulo
        {
            set
            {
                lblTextBox.Content = value;
            }
        }

        public Visibility Erro
        {
            set
            {
                lblTextBoxErro.Visibility = value;
            }
            get
            {
                return lblTextBoxErro.Visibility;
            }
        }

        public string Conteudo
        {
            set 
            { 
                txtBox.Text =  value;
            }
            get
            {
                return txtBox.Text;
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
                txtBox.MaxLength = value; 
            }
            get 
            {
                return txtBox.MaxLength;
            }
        }     

        #endregion

        #region "[Metodos]"

        public MTextBox()
        {
            InitializeComponent();
        }

        #endregion

        #region "[Eventos]"

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        #endregion

    }
}
