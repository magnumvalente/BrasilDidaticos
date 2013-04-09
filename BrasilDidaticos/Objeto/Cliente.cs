using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Objeto
{
    [DataContract]
    public class Cliente : Contrato.Cliente
    {
        [DataMember]
        public bool Selecionado
        {
            get;
            set;
        }
    }
}
