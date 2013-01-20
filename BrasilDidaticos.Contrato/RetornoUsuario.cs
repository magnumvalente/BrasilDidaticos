using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Contrato
{
    [DataContract]
    public class RetornoUsuario : Retorno
    {        
        [DataMember]
        public List<Contrato.Usuario> Usuarios
        {
            get;
            set;
        }        
    }
}
