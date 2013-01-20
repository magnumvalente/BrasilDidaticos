using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Contrato
{
    [DataContract]
    public class Sessao
    {

        [DataMember]
        public Guid Id
        {
            get;
            set;
        }


        [DataMember]
        public string Login
        {
            get;
            set;
        }


        [DataMember]
        public string Chave
        {
            get;
            set;
        }
    }
}
