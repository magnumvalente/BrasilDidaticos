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
    /// Interaction logic for MComboBox.xaml
    /// </summary>
    public partial class MComboBox : UserControl
    {
        public delegate void SelectionChanged(object sender, SelectionChangedEventArgs e);
        public event SelectionChanged SelectionChangedEvent;

        #region "[Propriedades]"

        public string Titulo
        {
            set
            {
                lblComboBox.Content = value;
            }
        }

        public Visibility Erro
        {
            set
            {
                lblComboBoxErro.Visibility = value;
            }
            get
            {
                return lblComboBoxErro.Visibility;
            }
        }

        public ComboBox ComboBox
        {
            set
            {
                cmbComboBox = (ComboBox)value;
            }
            get
            {
                return cmbComboBox;
            }
        }

        public double WidthTitulo
        {
            set
            {
                lblComboBox.Width = value;
            }
            get
            {
                return lblComboBox.Width;
            }
        }

        public double WidthConteudo
        {
            set
            {
                cmbComboBox.Width = value;
            }
            get
            {
                return cmbComboBox.Width;
            }
        }

        public object ValorSelecionado
        {
            set
            {
                cmbComboBox.SelectedValue = value;
            }
            get
            {
                return cmbComboBox.SelectedValue;
            }
        }

        #endregion

        #region "[Metodos]"

        public MComboBox()
        {
            InitializeComponent();
        }

        #endregion

        #region "[Eventos]"

        private void cmbComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectionChangedEvent != null)
                SelectionChangedEvent(sender, e);
        }

        #endregion
    }
}