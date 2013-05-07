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
using System.Collections.ObjectModel;

namespace BrasilDidaticos.Apresentacao
{
    /// <summary>
    /// Interaction logic for WImportarItens.xaml
    /// </summary>
    public partial class WImportarItens : Window
    {
        #region "[Constantes]"

        const int MAX_TAM_DESCRICAO = 300;
        const int MAX_TAM_QUANTIDADE = 9;
        const int MAX_TAM_VALOR_CUSTO = 14;
        const int MAX_TAM_VALOR_UNITARIO = 14;
        const int MAX_TAM_VALOR_DESCONTO = 14;

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

        public WImportarItens()
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

            if (conteudo[0].Trim().Length > MAX_TAM_DESCRICAO)
            {
                novaLinha += string.Format(" - [O tamanho '{0}' da descrição do item ultrapassa o máximo '{1}' permitido.]", conteudo[1].Trim().Length, MAX_TAM_DESCRICAO);
                retorno = false;
            }

            if (conteudo[1].Trim().Length > MAX_TAM_QUANTIDADE)
            {
                novaLinha += string.Format(" - [O tamanho '{0}' da quantidade do item ultrapassa o máximo '{1}' permitido.]", conteudo[0].Trim().Length, MAX_TAM_QUANTIDADE);
                retorno = false;
            }            

            //if (conteudo[2].Trim().Length > MAX_TAM_VALOR_CUSTO)
            //{
            //    novaLinha += string.Format(" - [O tamanho '{0}' do valor de custo do item ultrapassa o máximo '{1}' permitido.]", conteudo[1].Trim().Length, MAX_TAM_VALOR_CUSTO);
            //    retorno = false;
            //}

            if (conteudo[3].Trim().Length > MAX_TAM_VALOR_UNITARIO)
            {
                novaLinha += string.Format(" - [O tamanho '{0}' do valor unitário do item ultrapassa o máximo '{1}' permitido.]", conteudo[1].Trim().Length, MAX_TAM_VALOR_UNITARIO);
                retorno = false;
            }

            if (conteudo[4].Trim().Length > MAX_TAM_VALOR_DESCONTO)
            {
                novaLinha += string.Format(" - [O tamanho '{0}' do valor do desconto do item ultrapassa o máximo '{1}' permitido.]", conteudo[1].Trim().Length, MAX_TAM_VALOR_DESCONTO);
                retorno = false;
            }

            double result = double.MinValue;
            if (!double.TryParse(Comum.Util.Decriptar(conteudo[2].Trim()), out result))
            {
                novaLinha += string.Format(" - [O conteudo '{0}' do valor do custo não é numerico.]", Comum.Util.Decriptar(conteudo[2].Trim()));
                retorno = false;
            }

            if (!double.TryParse(conteudo[3].Trim(), out result))
            {
                novaLinha += string.Format(" - [O conteudo '{0}' do valor do unitário não é numerico.]", conteudo[3].Trim());
                retorno = false;
            }

            if (conteudo[4].Trim() != string.Empty && !double.TryParse(conteudo[4].Trim(), out result))
            {
                novaLinha += string.Format(" - [O conteudo '{0}' do valor do desconto não é numerico.]", conteudo[4].Trim());
                retorno = false;
            }

            novaLinha = (retorno ? "OK - " : "NOK - ") + novaLinha;

            return retorno;
        }

        public ObservableCollection<Contrato.Item> LerItensArquivo()
        {
            // Lista com os produtos
            ObservableCollection<Contrato.Item> itens = new ObservableCollection<Contrato.Item>();

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

                        // Verifica se existe pelo menos '5' informações
                        if (conteudo.Length >= 5)
                        {
                            // Se a linha está válida
                            if (ValidarLinha(conteudo, linha, out novaLinha))
                            {
                                // Adiciona o produto a lista
                                itens.Add(new Contrato.Item
                                {
                                    Descricao = conteudo[0].Trim(),
                                    Quantidade  = int.Parse(conteudo[1].Trim()),
                                    ValorCusto = decimal.Parse(Comum.Util.Decriptar(conteudo[2].Trim())),
                                    ValorUnitario = decimal.Parse(conteudo[3].Trim())
                                });
                                
                                if (conteudo[4].Trim() != string.Empty)
                                    itens.Last().ValorDesconto = decimal.Parse(conteudo[4].Trim());
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
                        MessageBox.Show(msgErro, "Orçamento", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }

            return itens;
        }

        private void ProcurarArquivo()
        {
            // Cria um OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Define o filtro para a extensão do arquivo 
            dlg.DefaultExt = ".csv";
            dlg.Filter = "Documento texto (.csv)|*.csv";

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
                MessageBox.Show(ex.ToString(), "Orçamento", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show(strValidacao.ToString(), "Orçamento", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Orçamento", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.ToString(), "Orçamento", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        #endregion
                
    }
}