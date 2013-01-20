using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Objeto
{
    [DataContract]
    public class Permissao : Contrato.Permissao
    {
        [DataMember]
        public bool Selecionado
        {
            get;
            set;
        }
    }
}
