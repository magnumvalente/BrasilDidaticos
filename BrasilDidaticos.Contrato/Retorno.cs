using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Contrato
{
    [DataContract]
    public class Retorno
    {

        [DataMember]
        public int Codigo
        {
            get;
            set;
        }

        [DataMember]
        public string Mensagem
        {
            get;
            set;
        }
    }
}
