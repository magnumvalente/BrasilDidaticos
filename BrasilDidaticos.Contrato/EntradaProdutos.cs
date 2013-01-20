using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Contrato
{
    [DataContract]
    public class EntradaProdutos : Entrada
    {        
        [DataMember]
        public List<Produto> Produtos
        {
            get;
            set;
        }

        [DataMember]
        public Contrato.Fornecedor Fornecedor
        {
            get;
            set;
        }
    }
}
