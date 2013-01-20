using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Contrato
{
    [DataContract]
    public class RetornoItem : Retorno
    {        
        [DataMember]
        public List<Contrato.Item> Itens
        {
            get;
            set;
        }        
    }
}
