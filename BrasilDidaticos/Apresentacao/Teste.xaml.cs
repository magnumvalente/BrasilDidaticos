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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BrasilDidaticos.Apresentacao
{
    /// <summary>
    /// Interaction logic for Teste.xaml
    /// </summary>
    public partial class Teste : Window
    {
        private ObservableCollection<Numerico> Decimais = new ObservableCollection<Numerico>();

        public class Numerico : INotifyPropertyChanged
        {
            private decimal _ValorDecimal = 0;

            public decimal ValorDecimal
            {
                get
                {
                    return _ValorDecimal;
                }
                set
                {
                    _ValorDecimal = value;
                    OnPropertyChanged("ValorPorcentagem");
                }
            }

            public int ValorInteiro
            {
                get;
                set;
            }

            public decimal ValorPorcentagem
            {
                get
                {
                    return _ValorDecimal / 100;
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        public Teste()
        {
            InitializeComponent();            
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
                string ret = servBrasilDidaticos.TestarServico(System.Net.Dns.GetHostName());
                MessageBox.Show(ret);
                servBrasilDidaticos.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Decimais.Add(new Numerico () { ValorDecimal = 5.34M });
            dataGrid1.ItemsSource = Decimais;
        }

        private void txtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ;
        }

        private void txtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ;
        }

        private void NumericFloatOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            string valorDecimal = e.Text;

            if (sender != null && sender.GetType() == typeof(TextBox))
                valorDecimal = ((TextBox)sender).Text + e.Text;
            
            e.Handled = Comum.Util.IsTextNumericFloat(e.Text) || !Comum.Util.IsDecimal(valorDecimal);
        }

        private void DataGridCell_NumericFloatOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (((DataGridCell)sender).Content.GetType() == typeof(TextBlock))
            {   
                e.Handled = Comum.Util.IsTextNumericFloat(e.Text) || !Comum.Util.IsDecimal(e.Text);
            }
        }

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
            {
                if (!cell.IsFocused)
                {
                    cell.Focus();
                }
                DataGrid dataGrid = Comum.Util.FindVisualParent<DataGrid>(cell);
                if (dataGrid != null)
                {
                    if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                    {
                        if (!cell.IsSelected)
                            cell.IsSelected = true;
                    }
                    else
                    {
                        DataGridRow row = Comum.Util.FindVisualParent<DataGridRow>(cell);
                        if (row != null && !row.IsSelected)
                        {
                            row.IsSelected = true;
                        }
                    }
                }
            }
        }
    }
}
