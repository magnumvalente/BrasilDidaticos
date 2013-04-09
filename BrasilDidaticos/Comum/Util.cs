using BrasilDidaticos.Apresentacao.Controler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BrasilDidaticos.Comum
{
    static class Util
    {
        private static Contrato.Usuario _UsuarioLogado = null;
        private static List<Contrato.UnidadeFederativa> _UnidadesFederativas = null;

        public static string CriptografiaMD5(string Valor)
        {
            string strResultado = "";    
            byte[] bytMensagem = System.Text.Encoding.Default.GetBytes(Valor);                    

            // Cria o Hash MD5 hash
            System.Security.Cryptography.MD5CryptoServiceProvider oMD5Provider = new System.Security.Cryptography.MD5CryptoServiceProvider();
    
            // Gera o Hash Code
            byte[] bytHashCode = oMD5Provider.ComputeHash(bytMensagem);
            for(int iItem = 0; iItem < bytHashCode.Length; iItem ++)
            {
               strResultado += bytHashCode[iItem].ToString("x2"); 
            }

            return strResultado;
        }

        public static string RecuperarIp()
        {
            string hostName = Dns.GetHostName();
            string ipAddress = string.Empty;

            IPHostEntry local = Dns.GetHostEntry(hostName);
            foreach (IPAddress ipaddress in local.AddressList)
            {
                if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    ipAddress = ipaddress.ToString() ;
            }

            return ipAddress;
        }

        public static Contrato.Usuario UsuarioLogado
        {
            set
            {
                _UsuarioLogado = value;
            }
            get
            {
                return _UsuarioLogado;
            }
        }

        public static string Chave
        {            
            get
            {
                return CriptografiaMD5(RecuperarIp());
            }
        }

        public static List<Contrato.UnidadeFederativa> UnidadesFederativas
        {    
            get
            {
                if (_UnidadesFederativas == null)
                {
                    Servico.BrasilDidaticosClient servBrasilDidaticos = new Servico.BrasilDidaticosClient();
                    Contrato.RetornoUnidadeFederativa retUnidadeFederativa = servBrasilDidaticos.UnidadeFederativaListar();
                    servBrasilDidaticos.Close();

                    if (retUnidadeFederativa.Codigo == Contrato.Constantes.COD_RETORNO_SUCESSO)
                    {
                        _UnidadesFederativas = retUnidadeFederativa.UnidadesFederativas;
                    }
                }

                return _UnidadesFederativas;
            }
        }

        public static UserControl CriarControler(string Titulo, string Valor, Contrato.Enumeradores.TipoParametro tipoParametro, object tag)
        {
            return CriarControler(Titulo, Valor, double.MinValue, tipoParametro, tag);
        }

        public static UserControl CriarControler(string Titulo, string Valor, double WidthTitulo, Contrato.Enumeradores.TipoParametro tipoParametro, object tag)
        {
            return CriarControler(Titulo, Valor, double.MinValue, double.MinValue, tipoParametro, tag);
        }

        public static UserControl CriarControler(string Titulo, string Valor, double WidthTitulo, double WidthConteudo, Contrato.Enumeradores.TipoParametro tipoParametro, object tag)
        {
            UserControl controler = null;

            switch (tipoParametro)
            {
                case Contrato.Enumeradores.TipoParametro.Texto:
                    controler = new MTextBox();
                    controler.Tag = tag;
                    ((MTextBox)controler).Titulo = string.Format("{0}:", Titulo);
                    ((MTextBox)controler).Conteudo = Valor;
                    break;
                case Contrato.Enumeradores.TipoParametro.Decimal:
                    controler = new MDecimalTextBox();
                    controler.Tag = tag;
                    ((MDecimalTextBox)controler).Titulo = string.Format("{0}:", Titulo);
                    ((MDecimalTextBox)controler).FormatString = "C2";
                    ((MDecimalTextBox)controler).Valor = decimal.Parse(Valor);                    
                    break;
                case Contrato.Enumeradores.TipoParametro.Percentagem:
                    controler = new MDecimalTextBox();
                    controler.Tag = tag;
                    ((MDecimalTextBox)controler).Titulo = string.Format("{0}:", Titulo);                    
                    ((MDecimalTextBox)controler).Valor = decimal.Parse(Valor) / 100;
                    ((MDecimalTextBox)controler).FormatString = "P";
                    break;
                case Contrato.Enumeradores.TipoParametro.Inteiro:
                    controler = new MDecimalTextBox();
                    controler.Tag = tag;
                    ((MDecimalTextBox)controler).Titulo = string.Format("{0}:", Titulo);
                    ((MDecimalTextBox)controler).FormatString = "F0";
                    ((MDecimalTextBox)controler).Valor = int.Parse(Valor);
                    if (WidthTitulo != double.MinValue) ((MDecimalTextBox)controler).WidthTitulo = WidthTitulo;
                    if (WidthConteudo != double.MinValue) ((MDecimalTextBox)controler).WidthConteudo = WidthConteudo;
                    break;
            }
            return controler;
        }

        public static bool ValidarPermissao(string nomeTela, string nomePermissao)
        { 
            return ValidarPermissao(UsuarioLogado, nomeTela, nomePermissao);
        }

        public static bool ValidarPermissao(Contrato.Usuario usuarioLogado, string nomeTela, string nomePermissao)
        {
            string nome = string.Format("{0}_{1}", nomeTela, nomePermissao);

            if (usuarioLogado.Perfis != null && usuarioLogado.Perfis.Count > 0)
            {
                foreach (Contrato.Perfil perfil in usuarioLogado.Perfis)
                {
                    Contrato.Permissao objPermissao = (from p in perfil.Permissoes
                                                       where p.Nome == nome
                                                      select p).FirstOrDefault();
                    if (objPermissao != null)
                        return true;
                }
            }
            return false;
        }

        public static StackPanel GroupHeader(string titulo, string caminhoImagem)
        {
            //Create stackpanel + children
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.VerticalAlignment = VerticalAlignment.Center;
            sp.Height = 25;

            //Add image to stackpanel
            sp.Children.Add(CreateImage(caminhoImagem));

            //Add TextBlock to stackpanel 
            TextBlock txtBlock = new TextBlock();
            txtBlock.Margin = new Thickness(5,5,0,0);
            txtBlock.Height = 25;
            txtBlock.Text = titulo;
            
            sp.Children.Add(txtBlock);
            
            return sp;
        }

        private static Image CreateImage(string caminho)
        {
            Image img = new Image();
            img.Height = 16;
            img.Width = 16;

            // Create source.
            BitmapImage bi = new BitmapImage();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();
            //add you image source in here 
            bi.UriSource = new Uri(caminho, UriKind.RelativeOrAbsolute);
            bi.EndInit();

            img.Source = bi;

            return img;
        }

        public static bool IsTextNumeric(string str)
        {
            
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[^0-9]");
            return reg.IsMatch(str);
        }

        public static bool IsInteger(string str)
        {
            int outParse;
            return int.TryParse(str, out outParse);
        }

        public static bool IsTextNumericFloat(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[^,0-9]");
            return reg.IsMatch(str);
        }

        public static bool IsDecimal(string str)
        {
            decimal outParse;
            return decimal.TryParse(str, out outParse) || (str.Length > 1 && (from s in str where s == ',' select s).Count() == 1);
        }

        public static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }

        public static T FindVisualChild<T>(UIElement element) where T : UIElement
        {
            UIElement child = element;
            while (child != null)
            {
                T correctlyTyped = child as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                child = VisualTreeHelper.GetParent(child) as UIElement;
            }
            return null;
        } 
    }
}
