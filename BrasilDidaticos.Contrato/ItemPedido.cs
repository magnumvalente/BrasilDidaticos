using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.ComponentModel;

namespace BrasilDidaticos.Contrato
{
    [DataContract]
    public class ItemPedido : INotifyPropertyChanged
    {
        private int _Quantidade = 0;
        private decimal _Valor = 0;
        private decimal? _ValorDesconto = null;

        [DataMember]
        public Guid Id
        {
            get;
            set;
        }
                
        [DataMember]
        public Pedido Pedido
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
        public UnidadeMedida UnidadeMedida
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

        public decimal? ValorDescontoPedido
        {
            get
            {
                if (Pedido != null)
                    return Pedido.ValorDesconto;
                return 0;
            }
        }

        public decimal Total
        {
            get
            {
                if (ValorDesconto != null)
                    return (Valor - Valor * (decimal)PercentagemDesconto) * Quantidade;

                return Valor * Quantidade;
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
