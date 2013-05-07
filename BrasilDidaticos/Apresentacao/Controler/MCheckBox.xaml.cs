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
    /// Interaction logic for MCheckBox.xaml
    /// </summary>
    public partial class MCheckBox : UserControl
    {
        public delegate void Click(object sender, RoutedEventArgs e);
        public event Click ClickEvent;

        #region "[Propriedades]"

        public string Titulo
        {
            set
            {
                lblCheckBox.Content = value;
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

        public bool? Selecionado
        {
            set 
            { 
                checkBox.IsChecked =  value;
            }
            get
            {
                return checkBox.IsChecked;
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

        #endregion

        #region "[Metodos]"

        public MCheckBox()
        {
            InitializeComponent();
        }

        #endregion

        #region "[Eventos]"

        private void checkBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ClickEvent != null)
                    ClickEvent(sender, e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        #endregion

    }
}
