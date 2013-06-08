﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Contrato
{
    [DataContract]
    public class RetornoPedido : Retorno
    {        
        [DataMember]
        public List<Contrato.Pedido> Pedidos
        {
            get;
            set;
        }        
    }
}
