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
    /// Interaction logic for MRadioButton.xaml
    /// </summary>
    public partial class MRadioButton : UserControl
    {
        #region "[Propriedades]"

        public string Titulo
        {
            set
            {
                lblRadioButton.Content = value;
            }
        }

        public Visibility Erro
        {
            set
            {
                lblRadioButtonErro.Visibility = value;
            }
            get
            {
                return lblRadioButtonErro.Visibility;
            }
        }

        public bool? Selecionado
        {
            set 
            { 
                rdbRadioButton.IsChecked =  value;
            }
            get
            {
                return rdbRadioButton.IsChecked;
            }
        }
        
        #endregion

        #region "[Metodos]"

        public MRadioButton()
        {
            InitializeComponent();
        }

        #endregion
    }
}
