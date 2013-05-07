using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Contrato
{
    [DataContract]
    public class Perfil
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
        public string Descricao
        {
            get;
            set;
        }

        [DataMember]
        public bool Ativo
        {
            get;
            set;
        }

        [DataMember]
        public List<Permissao> Permissoes
        {
            get;
            set;
        }
    }
}
