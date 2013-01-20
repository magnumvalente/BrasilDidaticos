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
    /// Interaction logic for MMaskTextBox.xaml
    /// </summary>
    public partial class MMaskTextBox : UserControl
    {
        #region "[Atributos]"

        private Comum.Enumeradores.TipoMascara _Tipo;

        #endregion

        #region "[Propriedades]"

        public string Titulo
        {
            set
            {
                lblMaskTextBox.Content = value;
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
                txtMaskTextBox.Text = value;
            }
            get
            {
                return txtMaskTextBox.Text;
            }
        }

        public double WidthTitulo
        {
            set
            {
                lblMaskTextBox.Width = value;
            }
            get
            {
                return lblMaskTextBox.Width;
            }
        }

        public double WidthConteudo
        {
            set
            {
                txtMaskTextBox.Width = value;
            }
            get
            {
                return txtMaskTextBox.Width;
            }
        }

        public object Valor
        {
            set
            {
                txtMaskTextBox.Value = value;
            }
            get
            {
                return txtMaskTextBox.Value;
            }
        }      

        public int TamanhoTexto
        {
            set
            {
                txtMaskTextBox.MaxLength = value;
            }
            get
            {
                return txtMaskTextBox.MaxLength;
            }
        }

        public Comum.Enumeradores.TipoMascara Tipo
        {
            set
            {
                _Tipo = value;
                DefinirMascara();
            }
            get 
            {
                return _Tipo;
            }
        }

        #endregion

        #region "[Metodos]"

        public MMaskTextBox()
        {
            InitializeComponent();
        }

        private void DefinirMascara()
        {
            switch (_Tipo)
            { 
                case Comum.Enumeradores.TipoMascara.CNPJ:
                    txtMaskTextBox.Mask = "00,000,000/0000-00";
                    break;
                case Comum.Enumeradores.TipoMascara.CPF:
                    txtMaskTextBox.Mask = "000,000,000-00";
                    break;
                case Comum.Enumeradores.TipoMascara.Telefone:
                    txtMaskTextBox.Mask = "(000) 0000-0000";
                    break;
                case Comum.Enumeradores.TipoMascara.Celular:
                    txtMaskTextBox.Mask = "(000) 0000-0000";
                    break;
                case Comum.Enumeradores.TipoMascara.Cep:
                    txtMaskTextBox.Mask = "00000-000";
                    break;
                default:
                    break;
            }

        }

        #endregion
    }
}
