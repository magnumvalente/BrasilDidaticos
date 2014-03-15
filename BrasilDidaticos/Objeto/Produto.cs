using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Objeto
{
    [DataContract]
    public class Produto : Contrato.Produto
    {
        [DataMember]
        public bool Selecionado
        {
            get;
            set;
        }

        [DataMember]
        public string FornecedorNome
        {
            get
            {
                return Fornecedor.Nome;
            }
        }

        [DataMember]
        public decimal ValorAtacado
        {
            get
            {
                if (ValorPercentagemAtacado.HasValue && ValorPercentagemAtacado.Value > 0)
                    return ValorCusto * (decimal)ValorPercentagemAtacado;

                if (Fornecedor != null && Fornecedor.ValorPercentagemAtacado.HasValue && Fornecedor.ValorPercentagemAtacado.Value > 0)
                    return ValorCusto * (decimal)Fornecedor.ValorPercentagemAtacado;

                return ValorCusto * Comum.Parametros.PercentagemAtacado;
            }
        }

        [DataMember]
        public decimal ValorVarejo
        {
            get
            {
                if (ValorPercentagemVarejo.HasValue && ValorPercentagemVarejo.Value > 0)
                    return ValorCusto * (decimal)ValorPercentagemVarejo;

                if (Fornecedor != null && Fornecedor.ValorPercentagemVarejo.HasValue && Fornecedor.ValorPercentagemVarejo.Value > 0)
                    return ValorCusto * (decimal)Fornecedor.ValorPercentagemVarejo;

                return ValorCusto * Comum.Parametros.PercentagemVarejo;
            }
        }
    }
}
