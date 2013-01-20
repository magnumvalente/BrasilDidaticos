using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Objeto
{
    [DataContract]
    public class Taxa : Contrato.Taxa
    {
        [DataMember]
        public bool Selecionado
        {
            get;
            set;
        }

        [DataMember]
        public string PercentagemToString
        {
            get
            {
                return string.Format("{0}%", Valor.ToString("N2"));
            }
            set
            {
                Valor = decimal.Parse(value.Replace("%",""));
            }
        }
    }
}
