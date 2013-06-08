using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Contrato
{

    [DataContract]
    public class Pedido
    {
        [DataMember]
        public Guid Id
        {
            get;
            set;
        }

        [DataMember]
        public string Codigo
        {
            get;
            set;
        }

        [DataMember]
        public EstadoPedido Estado
        {
            get;
            set;
        }

        [DataMember]
        public Usuario Responsavel
        {
            get;
            set;
        }

        [DataMember]
        public decimal? ValorDesconto
        {
            get;
            set;
        }

        [DataMember]
        public DateTime Data
        {
            get;
            set;
        }

        [DataMember]
        public List<ItemPedido> ItensPedido
        {
            get;
            set;
        }
    }
}
