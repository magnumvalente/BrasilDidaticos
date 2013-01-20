using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Contrato
{
    [DataContract]
    public class EstadoOrcamento
    {
        [DataMember]
        public Guid Id
        {
            get;
            set;
        }

        [DataMember]
        public string Codigo
        {
            get;
            set;
        }

        [DataMember]
        public string Nome
        {
            get;
            set;
        }
        [DataMember]
        public bool? Ativo
        {
            get;
            set;
        }

        [DataMember]
        public EstadoOrcamento Anterior
        {
            get;
            set;
        }

        [DataMember]
        public EstadoOrcamento Sucessor
        {
            get;
            set;
        }
    }
}
