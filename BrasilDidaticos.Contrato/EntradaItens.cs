using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Contrato
{
    [DataContract]
    public class EntradaItens : Entrada
    {        
        [DataMember]
        public List<Item> Itens
        {
            get;
            set;
        }

        [DataMember]
        public Contrato.Orcamento Orcamento
        {
            get;
            set;
        }
    }
}
