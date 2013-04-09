using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Contrato
{
    [DataContract]
    public class Entrada
    {
        [DataMember]
        public string UsuarioLogado
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

        [DataMember]
        public bool? Novo
        {
            get;
            set;
        }

        [DataMember]
        public bool Paginar
        {
            get;
            set;
        }

        [DataMember]
        public int CantidadeItens
        {
            get;
            set;
        }

        [DataMember]
        public int PosicaoUltimoItem
        {
            get;
            set;
        }

        [DataMember]
        public bool PreencherListaSelecao
        {
            get;
            set;
        }
    }
}

