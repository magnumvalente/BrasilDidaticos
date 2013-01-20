using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Contrato
{
    [DataContract]
    public class Cliente
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
        public bool Ativo
        {
            get;
            set;
        }

        [DataMember]
        public string CaixaEscolar
        {
            get;
            set;
        }

        [DataMember]
        public Enumeradores.Pessoa? Tipo
        {
            get;
            set;
        }

        [DataMember]
        public string Cpf_Cnpj
        {
            get;
            set;
        }

        [DataMember]
        public string Email
        {
            get;
            set;
        }

        [DataMember]
        public string Telefone
        {
            get;
            set;
        }

        [DataMember]
        public string Celular
        {
            get;
            set;
        }

        [DataMember]
        public string InscricaoEstadual
        {
            get;
            set;
        }

        [DataMember]
        public string Endereco
        {
            get;
            set;
        }

        [DataMember]
        public int? Numero
        {
            get;
            set;
        }

        [DataMember]
        public string Complemento
        {
            get;
            set;
        }

        [DataMember]
        public string Cep
        {
            get;
            set;
        }

        [DataMember]
        public string Bairro
        {
            get;
            set;
        }

        [DataMember]
        public string Cidade
        {
            get;
            set;
        }

        [DataMember]
        public UnidadeFederativa Uf
        {
            get;
            set;
        }

        public Cliente ClienteMatriz
        {
            get;
            set;
        }
    }    
}