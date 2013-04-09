using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.ComponentModel;

namespace BrasilDidaticos.Objeto
{
    [DataContract]
    public class Fornecedor : Contrato.Fornecedor, INotifyPropertyChanged
    {
        private bool _Selecionado;

        [DataMember]
        public bool Selecionado
        {
            get
            {
                return _Selecionado;
            }
            set
            {
                _Selecionado = value;
                OnPropertyChanged("Selecionado");
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
}
