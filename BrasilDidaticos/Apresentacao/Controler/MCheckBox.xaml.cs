﻿using System;
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
        
        #endregion

        #region "[Metodos]"

        public MCheckBox()
        {
            InitializeComponent();
        }

        #endregion
    }
}