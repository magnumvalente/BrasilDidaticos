using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.ComponentModel;

namespace BrasilDidaticos.Contrato
{
    [DataContract]
    public class Item : INotifyPropertyChanged
    {
        private int _Quantidade = 0;
        private decimal _ValorUnitario = 0;
        private decimal? _ValorDesconto = null;

        [DataMember]
        public Guid Id
        {
            get;
            set;
        }

        [DataMember]
        public string Descricao 
        {
            get;
            set;
        }

        [DataMember]
        public Orcamento Orcamento
        {
            get;
            set;
        }

        [DataMember]
        public Produto Produto
        {
            get;
            set;
        }
        
        [DataMember]
        public decimal ValorCusto
        {
            get;
            set;
        }

        [DataMember]
        public decimal ValorUnitario
        {
            get
            {
                return _ValorUnitario;
            }
            set
            {
                _ValorUnitario = value;
                OnPropertyChanged("ValorUnitario");
                OnPropertyChanged("Total");
            }
        }

        [DataMember]
        public decimal? ValorDesconto
        {
            get
            {
                return _ValorDesconto;
            }
            set
            {
                _ValorDesconto = value;
                OnPropertyChanged("PercentagemDesconto");
                OnPropertyChanged("Total");
            }
        }
        
        [DataMember]
        public int Quantidade
        {
            get
            {
                return _Quantidade;
            }
            set
            {
                _Quantidade = value;
                OnPropertyChanged("Total");
            }
        }

        public decimal? PercentagemDesconto
        {
            get
            {
                return (ValorDesconto / 100);
            }
        }

        public decimal? ValorDescontoOrcamento
        {
            get
            {
                if (Orcamento != null)
                    return Orcamento.ValorDesconto;
                return 0;
            }
        }

        public decimal Total
        {
            get
            {
                if (ValorDesconto != null)
                    return (ValorUnitario - ValorUnitario * (decimal)PercentagemDesconto) * Quantidade;

                return ValorUnitario * Quantidade;
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
