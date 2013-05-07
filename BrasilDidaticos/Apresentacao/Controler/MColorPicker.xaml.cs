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
    /// Interaction logic for MColorPicker.xaml
    /// </summary>
    public partial class MColorPicker : UserControl
    {
        #region "[Propriedades]"

        public string Titulo
        {
            set
            {
                lblColorPicker.Content = value;
            }
        }

        public Visibility Erro
        {
            set
            {
                lblColorPickerErro.Visibility = value;
            }
            get
            {
                return lblColorPickerErro.Visibility;
            }
        }

        public string Conteudo
        {
            set 
            {
                colColorPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(value);
            }
            get
            {
                return colColorPicker.SelectedColorText;
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
                colColorPicker.Width = value;
            }
            get
            {
                return colColorPicker.Width;
            }
        }

        #endregion

        #region "[Metodos]"

        public MColorPicker()
        {
            InitializeComponent();
        }

        #endregion        
    }
}
