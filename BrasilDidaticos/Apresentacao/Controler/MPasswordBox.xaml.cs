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
    /// Interaction logic for MPasswordBox.xaml
    /// </summary>
    public partial class MPasswordBox : UserControl
    {
        #region "[Propriedades]"

        public string Titulo
        {
            set
            {
                lblPasswordBox.Content = value;
            }
        }

        public Visibility Erro
        {
            set
            {
                lblPasswordBoxErro.Visibility = value;
            }
            get
            {
                return lblPasswordBoxErro.Visibility;
            }
        }

        public string Conteudo
        {
            set 
            { 
                txtPasswordBox.Password =  value;
            }
            get
            {
                return txtPasswordBox.Password;
            }
        }

        public int TamanhoTexto
        {
            set
            {
                txtPasswordBox.MaxLength = value;
            }
            get
            {
                return txtPasswordBox.MaxLength;
            }
        }

        #endregion

        #region "[Metodos]"

        public MPasswordBox()
        {
            InitializeComponent();
        }

        #endregion
    }
}
