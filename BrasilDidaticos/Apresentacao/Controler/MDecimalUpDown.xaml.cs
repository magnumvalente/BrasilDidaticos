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
    /// Interaction logic for MDecimalUpDown.xaml
    /// </summary>
    public partial class MDecimalUpDown : UserControl
    {
        #region "[Atributos]"

        

        #endregion

        #region "[Propriedades]"

        public string Titulo
        {
            set
            {
                lblDecimalUpDown.Content = value;
            }
        }
                
        public Visibility Erro
        {
            set
            {
                lblDecimalUpDownErro.Visibility = value;
            }
            get
            {
                return lblDecimalUpDownErro.Visibility;
            }
        }

        public string Conteudo
        {
            set 
            {
                txtDecimalUpDown.Text = value;
            }
            get
            {
                return txtDecimalUpDown.Text;
            }
        }

        public double WidthTitulo
        {
            set
            {
                lblDecimalUpDown.Width = value;
            }
            get
            {
                return lblDecimalUpDown.Width;
            }
        }

        public double WidthConteudo
        {
            set
            {
                txtDecimalUpDown.Width = value;
            }
            get
            {
                return txtDecimalUpDown.Width;
            }
        }

        public decimal? Valor
        {
            set
            {
                txtDecimalUpDown.Value = value;
            }
            get
            {
                return txtDecimalUpDown.Value;
            }
        }       

        #endregion

        #region "[Metodos]"

        public MDecimalUpDown()
        {
            InitializeComponent();
        }

        #endregion
    }
}
