using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace BrasilDidaticos.Apresentacao
{
    /// <summary>
    /// Interaction logic for WImportarProduto.xaml
    /// </summary>
    public partial class WImportarProduto : Window
    {
        #region "[Constantes]"

        const int MAX_TAM_CODIGO = 20;
        const int MAX_TAM_DESCRICAO = 300;

        #endregion

        #region "[Atributos]"

        private bool _cancelou = false;

        #endregion

        #region "[Propriedades]"
        
        public bool Cancelou
        {
            get
            {
                return _cancelou;
            }
        }
        
        #endregion

        #region "[Metodos]"

        public WImportarProduto()
        {
            InitializeComponent();
        }

        private StringBuilder ValidarCampos()
        {
            StringBuilder strValidacao = new StringBuilder();
            
            // Verifica se o Código foi informado
            if (string.IsNullOrWhiteSpace(txtCaminhoArquivo.Conteudo))
            {
                txtCaminhoArquivo.Erro = System.Windows.Visibility.Visible;
                strValidacao.Append("O 'Caminho do Arquivo' não foi informado!\n");
            }
            else
                txtCaminhoArquivo.Erro = System.Windows.Visibility.Hidden;            
                        
            return strValidacao;
        }

        private bool ValidarLinha(string[] conteudo, string linha, out string novaLinha)
        {
            bool retorno = true;
            novaLinha = linha;

            if (conteudo[0].Trim().Length > MAX_TAM_CODIGO)
            {
                novaLinha += string.Format(" - [O tamanho '{0}' do código do produto ultrapassa o máximo '{1}' permitido.]", conteudo[0].Trim().Length, MAX_TAM_CODIGO);
                retorno = false;
            }

            if (conteudo[1].Trim().Length > MAX_TAM_DESCRICAO)
            {
                novaLinha += string.Format(" - [O tamanho '{0}' da descrição do produto ultrapassa o máximo '{1}' permitido.]", conteudo[1].Trim().Length, MAX_TAM_DESCRICAO);
                retorno = false;
            }

            double result = double.MinValue;
            if (!double.TryParse(conteudo[2].Trim(), out result))
            {
                novaLinha += string.Format(" - [O conteudo '{0}' do valor não é numerico.]", conteudo[2].Trim());
                retorno = false;
            }

            novaLinha = (retorno ? "OK - " : "NOK - ") + novaLinha;

            return retorno;
        }

        public List<Contrato.Produto> LerProdutosArquivo()
        {
            // Lista com os produtos
            List<Contrato.Produto> produtos = new List<Contrato.Produto>();

            // Verifica se encontrou algum erro no arquivo
            bool encontrouErro = false;

            // Se o arquivo foi informado
            if (!string.IsNullOrWhiteSpace(txtCaminhoArquivo.Conteudo))
            {
                // Lê as linhas do arquivo
                string[] strLinhas = File.ReadAllLines(txtCaminhoArquivo.Conteudo, Encoding.Default);

                // Se existe linhas
                if (strLinhas.Length > 0)
                {
                    // Retorno
                    List<string> strLinhasRetorno = new List<string>();

                    // Para cada linha existente
                    foreach (string linha in strLinhas)
                    {
                        // Nova Linha
                        string novaLinha = string.Empty;

                        // Recupera os dados separados por ';'
                        string[] conteudo = linha.Split(';');

                        // Verifica se existe pelo menos '3' informações
                        if (conteudo.Length >= 3)
                        {
                            // Se a linha está válida
                            if (ValidarLinha(conteudo, linha, out novaLinha))
                            {
                                // Adiciona o produto a lista
                                produtos.Add(new Contrato.Produto
                                {
                                    CodigoFornecedor = conteudo[0].Trim(),
                                    Nome = conteudo[1].Trim(),
                                    ValorBase = decimal.Parse(conteudo[2].Trim()),
                                    Ativo = true
                                });

                                // Verifica se vai existir a coluna de NCM do produto
                                if (chkNCM.Selecionado != null && chkNCM.Selecionado == true)
                                    produtos.Last().Ncm = conteudo[3].Trim();
                            }
                            else
                                encontrouErro = true;
                        }
                        // Adiciona um nova linha no arquivo de retorno
                        strLinhasRetorno.Add(novaLinha);
                    }
                    
                    // Verifica se algum erro foi encontrado
                    if (encontrouErro)
                    {
                        string nomeArquivo = txtCaminhoArquivo.Conteudo + ".LOG";

                        // Grava um arquivo de log
                        File.WriteAllLines(nomeArquivo, strLinhasRetorno);

                        // Mensagem de retorno
                        string msgErro = string.Format("Foram encontrados erros no arquivo '{0}'!\n Verifique no arquivo de log '{1}' as linhas com problemas.\nCaso elas não sejam corregidas, os produtos existentes nas mesmas não serão importados!", txtCaminhoArquivo.Conteudo, nomeArquivo);

                        // Exibi uma mensagem informando que foram encontrados erros no arquivo
                        MessageBox.Show(msgErro, "Fornecedor", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }

            return produtos;
        }

        private void ProcurarArquivo()
        {
            // Cria um OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Define o filtro para a extensão do arquivo 
            dlg.DefaultExt = ".csv";
            dlg.Filter = "Text documents (.csv)|*.csv";

            // Exibe o OpenFileDialog 
            if (dlg.ShowDialog() == true)
            {
                // obtem o nome do arquivo selecionado
                txtCaminhoArquivo.Conteudo = dlg.FileName;
            }

            // Se selecionou o arquivo
            if (!string.IsNullOrWhiteSpace(txtCaminhoArquivo.Conteudo))
            {
                // define um objeto Paragraph e lê o arquivo para as linhas do parágrafo
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(System.IO.File.ReadAllText(txtCaminhoArquivo.Conteudo, Encoding.Default));

                // exibe o documento no controle
                FlowDocument documento = new FlowDocument(paragraph);
                documento.Background = Brushes.White;
                FlowDocReader.Document = documento;
            }
        }

        #endregion

        #region "[Eventos]"

        private void btnProcurar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                ProcurarArquivo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Fornecedor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void btnGravar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;                
                StringBuilder strValidacao = ValidarCampos();

                // Verifica se as informações do usuário são válidas
                if (strValidacao.Length > 0)
                    MessageBox.Show(strValidacao.ToString(), "Fornecedor", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Fornecedor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                this._cancelou = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Fornecedor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        #endregion
                
    }
}