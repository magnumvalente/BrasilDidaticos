using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Contrato
{

    [DataContract]
    public class Orcamento
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
        public EstadoOrcamento Estado
        {
            get;
            set;
        }

        [DataMember]
        public Cliente Cliente
        {
            get;
            set;
        }

        [DataMember]
        public Usuario Responsavel
        {
            get;
            set;
        }

        [DataMember]
        public Usuario Vendedor
        {
            get;
            set;
        }  

        [DataMember]
        public decimal? ValorDesconto
        {
            get;
            set;
        }

        [DataMember]
        public DateTime Data
        {
            get;
            set;
        }

        [DataMember]
        public int? ValidadeOrcamento
        {
            get;
            set;
        }

        [DataMember]
        public int? PrazoEntrega
        {
            get;
            set;
        }

        [DataMember]
        public List<Item> Itens
        {
            get;
            set;
        }

        public bool Licitacao
        {
            get
            {
                return !(PrazoEntrega.HasValue && ValidadeOrcamento.HasValue);
            }
        }
    }
}
