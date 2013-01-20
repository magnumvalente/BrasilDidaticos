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
    public partial class MRadioListBox : UserControl
    {
        public delegate void SelectionChanged(object sender, SelectionChangedEventArgs e);
        public event SelectionChanged SelectionChangedEvent;

        #region "[Propriedades]"

        public string Titulo
        {
            set
            {
                lblRadioListBox.Content = value;
            }
        }

        public Visibility Erro
        {
            set
            {
                lblRadioListBoxErro.Visibility = value;
            }
            get
            {
                return lblRadioListBoxErro.Visibility;
            }
        }

        public ListBox ListBox
        {
            set 
            {
                lstRadioListBox = value;
            }
            get 
            {
                return lstRadioListBox;
            }
        }

        public object ValorSelecionado
        {
            set
            {
                lstRadioListBox.SelectedValue = value;
            }
            get
            {
               return lstRadioListBox.SelectedValue;
            }
        }
        
        #endregion

        #region "[Metodos]"

        public MRadioListBox()
        {
            InitializeComponent();
        }

        #endregion

        #region "[Eventos]"

        private void lstRadioListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (SelectionChangedEvent != null)
                    SelectionChangedEvent(sender, e);
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
