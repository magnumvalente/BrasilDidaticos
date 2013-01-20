using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Contrato
{
    [DataContract]
    public class EntradaTaxa : Entrada
    {        
        [DataMember]
        public Taxa Taxa
        {
            get;
            set;
        }        
    }
}
