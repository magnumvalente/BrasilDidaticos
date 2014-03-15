using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BrasilDidaticos.Contrato
{

    [DataContract]
    [Serializable]
    public class Produto
    {
        private decimal _ValorCusto;
        private int _Quantidade;
        private List<Taxa> _Taxas;

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
        public string Ncm
        {
            get;
            set;
        }

        [DataMember]
        public string CodigoBarras
        {
            get;
            set;
        }

        [DataMember]
        public string CodigoFornecedor
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
        public int Quantidade
        {
            get 
            {
                if (UnidadeMedidas != null && UnidadeMedidas.Count > 0)
                {
                    return UnidadeMedidas.Sum(um => um.Quantidade * um.QuantidadeItens);
                }
                return _Quantidade;
            }
            set
            {
                _Quantidade = value;
            }
            
        }

        [DataMember]
        public decimal ValorBase
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

        public IEnumerable<Taxa> eTaxas
        {
            get;
            set;
        }

        [DataMember]
        public List<Taxa> Taxas
        {
            get 
            {
                if (eTaxas != null)
                {
                    _Taxas = eTaxas.ToList();
                    eTaxas = null;                    
                }
                return _Taxas;
            }
            set
            {
                _Taxas = value;
            }
        }

        [DataMember]
        public List<UnidadeMedida> UnidadeMedidas
        {
            get;
            set;
        }

        [DataMember]
        public UnidadeMedida UnidadeMedidaSelecionada
        {
            get;
            set;
        }

        [DataMember]
        public Fornecedor Fornecedor
        {
            get;
            set;
        }

        [DataMember]
        public List<Fornecedor> Fornecedores
        {
            get;
            set;
        }

        public decimal ValorCusto
        {
            get
            {
                _ValorCusto = ValorBase;

                if (Taxas != null && Taxas.Count > 0 && Taxas.FirstOrDefault() != null)
                {

                    var taxas = from t in
                                    (from tx in Taxas select new { Prioridade = tx.Prioridade, Percentagem = tx.Desconto.HasValue ? -1 * tx.Percentagem : tx.Percentagem }).ToList()
                                group t by t.Prioridade into p
                                select new { Prioridade = p.Key, Percentagem = p.Sum(v => v.Percentagem) };

                    if (taxas != null && taxas.Count() > 0)
                    {
                        foreach (var taxa in taxas)
                        {
                            _ValorCusto += _ValorCusto * taxa.Percentagem;
                        }
                    }
                }

                return _ValorCusto;
            }
        }
    }
}