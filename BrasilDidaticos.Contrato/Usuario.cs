using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Contrato
{
    [DataContract]
    public class Usuario
    {   
        [DataMember]
        public Guid Id
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
        public string Login
        {
            get;
            set;
        }

        [DataMember]
        public string Senha
        {
            get;
            set;
        }

        [DataMember]
        public List<BrasilDidaticos.Contrato.Perfil> Perfis
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
    }
}
