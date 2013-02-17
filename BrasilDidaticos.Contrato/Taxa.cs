using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.ComponentModel;

namespace BrasilDidaticos.Contrato
{
    [DataContract]
    public class Taxa : INotifyPropertyChanged
    {
        private decimal _Valor = 0;

        [DataMember]
        public Guid Id
        {
            get;
            set;
        }

        [DataMember]
        public string Nome
        {
            get;
            set;
        }

        [DataMember]
        public bool? Fornecedor
        {
            get;
            set;
        }

        [DataMember]
        public bool? Produto
        {
            get;
            set;
        }

        [DataMember]
        public bool? Desconto
        {
            get;
            set;
        }

        [DataMember]
        public decimal Valor
        {
            get
            {
                return _Valor;
            }
            set
            {
                _Valor = value;
                OnPropertyChanged("Percentagem");
            }
        }

        [DataMember]
        public decimal Percentagem
        {
            get
            {
                return (Valor / 100);
            }
            set
            {
                Valor = (value * 100);
                OnPropertyChanged("Percentagem");
                OnPropertyChanged("Valor");
            }
        }

        [DataMember]
        public short Prioridade
        {
            get;
            set;
        }

        [DataMember]
        public bool Ativo
        {
            get;
            set;
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
