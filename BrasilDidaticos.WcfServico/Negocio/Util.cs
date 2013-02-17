using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace BrasilDidaticos.WcfServico.Negocio
{
    internal static class Util
    {
        internal const string INI_COD_PRODUTO = "PD{0}";
        internal const string INI_COD_FORNECEDOR = "FN{0}";
        internal const string INI_COD_CLIENTE = "CL{0}";
        internal const string INI_COD_ORCAMENTO = "OR{0}";

        internal const int MAX_COD_PRODUTO = 6;
        internal const int MAX_COD_FORNECEDOR = 6;
        internal const int MAX_COD_CLIENTE = 6;
        internal const int MAX_COD_ORCAMENTO = 10;

        internal static string Criptografar(string entrada)
        {
            string txtResultado = "";
            byte[] txtMensagem = System.Text.Encoding.Default.GetBytes(entrada);// Criar o Hash Code
            System.Security.Cryptography.MD5CryptoServiceProvider txtMD5provider = new MD5CryptoServiceProvider();
            //Hash Code
            byte[] txthashcode = txtMD5provider.ComputeHash(txtMensagem);
            for (int i = 0; i < txthashcode.Length; i++)
            {
                txtResultado += (char)(txthashcode[i]);
            }

            return txtResultado;
        }

        internal static string RecuperaCodigo(int codigo, string tipoCodigo)
        {
            switch (tipoCodigo)
            {
                case Contrato.Constantes.TIPO_COD_PRODUTO:
                    return string.Format(INI_COD_PRODUTO, codigo.ToString().PadLeft(MAX_COD_PRODUTO, '0'));
                case Contrato.Constantes.TIPO_COD_FORNECEDOR:
                    return string.Format(INI_COD_FORNECEDOR, codigo.ToString().PadLeft(MAX_COD_FORNECEDOR, '0'));
                case Contrato.Constantes.TIPO_COD_CLIENTE:
                    return string.Format(INI_COD_CLIENTE, codigo.ToString().PadLeft(MAX_COD_CLIENTE, '0'));
                case Contrato.Constantes.TIPO_COD_ORCAMENTO:
                    return string.Format(INI_COD_ORCAMENTO, codigo.ToString().PadLeft(MAX_COD_ORCAMENTO, '0'));
                default:
                    return string.Empty;                
            }
        }
    }    
}