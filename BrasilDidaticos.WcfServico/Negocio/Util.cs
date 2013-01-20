using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace BrasilDidaticos.WcfServico.Negocio
{
    public static class Util
    {
        public static string Criptografar(string entrada)
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
    }
}