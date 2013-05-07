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
        private long _Duracao = DateTime.Now.Ticks;

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

        [DataMember]
        public long Duracao
        {
            get
            {
                return _Duracao;
            }
            set 
            {
                _Duracao = value;
            }
        }
    }
}
