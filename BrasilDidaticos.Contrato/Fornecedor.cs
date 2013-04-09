using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Contrato
{
    [DataContract]
    public class Fornecedor
    {
        private bool _PessoaFisica; 

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
        public string Codigo
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
        public bool PessoaFisica
        {
            get
            {
                return _PessoaFisica;
            }
            set
            {
                _PessoaFisica = value;
                Tipo = _PessoaFisica ? Contrato.Enumeradores.Pessoa.Fisica : Contrato.Enumeradores.Pessoa.Juridica;
            }
        }

        [DataMember]
        public string Cpf_Cnpj
        {
            get;
            set;
        }

        [DataMember]
        public decimal? ValorPercentagemVarejo
        {
            get;
            set;
        }

        [DataMember]
        public decimal? ValorPercentagemAtacado
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
        public List<Taxa> Taxas
        {
            get;
            set;
        }
    }    
}
