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
    /// Interaction logic for MDatePicker.xaml
    /// </summary>
    public partial class MDatePicker : UserControl
    {
        #region "[Propriedades]"

        public string Titulo
        {
            set
            {
                lblDatePicker.Content = value;
            }
        }

        public Visibility Erro
        {
            set
            {
                lblDatePickerErro.Visibility = value;
            }
            get
            {
                return lblDatePickerErro.Visibility;
            }
        }

        public string Conteudo
        {
            set 
            {
                dtpDatePicker.Text = value;
            }
            get
            {
                return dtpDatePicker.Text;
            }
        }

        public double WidthTitulo
        {
            set
            {
                lblDatePicker.Width = value;
            }
            get
            {
                return lblDatePicker.Width;
            }
        }

        public double WidthConteudo
        {
            set
            {
                dtpDatePicker.Width = value;
            }
            get
            {
                return dtpDatePicker.Width;
            }
        }

        #endregion

        #region "[Metodos]"

        public MDatePicker()
        {
            InitializeComponent();
            dtpDatePicker.SelectedDate = DateTime.Today;
        }

        #endregion
    }
}
