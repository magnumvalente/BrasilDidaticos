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
    /// Interaction logic for MDecimalTextBox.xaml
    /// </summary>
    public partial class MDecimalTextBox : UserControl
    {
        #region "[Atributos]"

        private string _formatString = Comum.Constantes.STRING_FORMAT_MOEDA;
        private decimal? _valor = null;

        #endregion

        #region "[Propriedades]"

        public string Titulo
        {
            set
            {
                lblDecimalUpDown.Content = value;
            }
        }

        public string FormatString
        {
            set
            {
                if (value.StartsWith(Comum.Constantes.STRING_FORMAT_DECIMAL.ElementAt(0).ToString()))
                    _formatString = value;
                else if (value.StartsWith(Comum.Constantes.STRING_FORMAT_MOEDA.ElementAt(0).ToString()))
                    _formatString = value;
                else if (value.StartsWith(Comum.Constantes.STRING_FORMAT_PORCENTAGEM.ElementAt(0).ToString()))
                    _formatString = value;

                txtDecimalTextBox.Text = _valor.HasValue ? _valor.Value.ToString(_formatString) : string.Empty;
            }
            get
            {
                return _formatString;
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
                if (!value.Equals(string.Empty))
                    _valor = decimal.Parse(value);
                else
                    _valor = null;
                txtDecimalTextBox.Text = _valor.HasValue ? _valor.Value.ToString(_formatString) : string.Empty;
            }
            get
            {
                return txtDecimalTextBox.Text;
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
                txtDecimalTextBox.MaxLength = value;
            }
            get
            {
                return txtDecimalTextBox.MaxLength;
            }
        }  

        public decimal? Valor
        {
            set
            {
                bool valorAlterado = value != _valor;
                
                if (value.HasValue)
                    _valor = value;
                else
                    _valor = null;

                if (valorAlterado && _formatString.StartsWith(Comum.Constantes.STRING_FORMAT_PORCENTAGEM.ElementAt(0).ToString()))
                    _valor = _valor / 100;

                txtDecimalTextBox.Text = _valor.HasValue ? _valor.Value.ToString(_formatString) : string.Empty;
            }
            get
            {                
                if (_valor.HasValue)
                    return _valor;
                return null;
            }
        }       

        #endregion

        #region "[Metodos]"

        public MDecimalTextBox()
        {
            InitializeComponent();
        }
        
        #endregion

        #region "[Eventos]"

        private void textBox_PreviewTextInput(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            string valorDecimal = e.Text;
            
            if (sender != null && sender.GetType() == typeof(TextBox))
                valorDecimal = ((TextBox)sender).Text + e.Text;

            e.Handled = Comum.Util.IsNumericFloat(e.Text) || !Comum.Util.IsDecimal(valorDecimal);
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(((TextBox)sender).Text))
            {
                if (_formatString.StartsWith(Comum.Constantes.STRING_FORMAT_PORCENTAGEM.ElementAt(0).ToString()))
                    Valor = decimal.Parse(((TextBox)sender).Text.Replace("%",string.Empty).Trim());
                else if (_formatString.StartsWith(Comum.Constantes.STRING_FORMAT_MOEDA.ElementAt(0).ToString()))
                    Valor = decimal.Parse(((TextBox)sender).Text.Replace("R$",string.Empty).Trim());
                else if (_formatString.StartsWith(Comum.Constantes.STRING_FORMAT_DECIMAL.ElementAt(0).ToString()))
                    Valor = decimal.Parse(((TextBox)sender).Text.Trim()); 
            }
            else
                Valor = null;
        }

        #endregion
    }
}
