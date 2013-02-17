using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Objeto
{
    [DataContract]
    public class Fornecedor : Contrato.Fornecedor
    {
        [DataMember]
        public bool Selecionado
        {
            get;
            set;
        }
    }
}
