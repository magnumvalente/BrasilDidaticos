using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Contrato
{
    [DataContract]
    public class EntradaItemPedido : Entrada
    {        
        [DataMember]
        public ItemPedido ItemPedido
        {
            get;
            set;
        }
    }
}
