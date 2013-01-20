using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Contrato
{
    [DataContract]
    public class EntradaUsuario : Entrada
    {        
        [DataMember]
        public Usuario Usuario
        {
            get;
            set;
        }        
    }
}
