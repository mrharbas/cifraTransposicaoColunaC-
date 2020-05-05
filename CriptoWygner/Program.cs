using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CriptoWygner
{
    class Program
    {
        // variáveis globais
        private static string escolha;

        private static string chave;
        private static char[] chaveOrdenada;

        private static string texto;
        private static string textoSomenteLetras;
        private static string textoFinal;
        private static string textoTransposicaoColuna;
        private static string textoCifrado;

        private static string complementoTexto;

        private static int quantidadeCaracteresChave;

        private static List<string> linhas;
        private static List<string> colunas;

        private static bool utilizarCaracteresEspeciaisNaChave;
        private static List<char> caracteresPossiveisChave;
        private static List<char> caracteresPossiveisComplemento;
        private static int maxCaracChave;

        static void Main(string[] args)
        {
            Cabecalho();

            while (escolha != "0")
            {
                bool escolhaValida = false;

                Menu();
                Console.Clear();

                if (escolha != "0" && escolha != "4")
                {
                    UtilizaCaracteresEspeciais();
                    caracteresPossiveisChave = ObterListaCaracteresPossiveisChave(utilizarCaracteresEspeciaisNaChave, true);
                    caracteresPossiveisComplemento = ObterListaCaracteresPossiveisChave(false, false);
                }

                switch (escolha)
                {
                    case "0":
                        escolhaValida = true;
                        break;
                    case "1":
                        escolhaValida = true;
                        LerTamanhoDaChave();
                        chave = GerarStringAleatoria(quantidadeCaracteresChave, true);
                        break;
                    case "2":
                        escolhaValida = true;
                        LerChave();
                        break;
                    case "3":
                        Console.WriteLine($"\n\tCaracteres Possíveis: {new string(caracteresPossiveisChave.ToArray())}");
                        break;
                    case "4":
                        utilizarCaracteresEspeciaisNaChave = true;
                        caracteresPossiveisChave = ObterListaCaracteresPossiveisChave(true, true);
                        LerChave();
                        LerTextoCifrado();
                        DecifrarTexto();
                        break;
                    default:
                        Console.WriteLine("\n\tOpção Inválida. Escolha uma das opções do Menu:");
                        break;
                }

                if (escolha == "0")
                    break;

                if (escolhaValida)
                {
                    chaveOrdenada = chave.OrderBy(c => c).ToArray();
                    complementoTexto = GerarStringAleatoria(quantidadeCaracteresChave - 1, false);

                    LerTexto();
                    GerarLinhas();
                    GerarColunas();

                    ExibirMatriz();

                    ExibirResultado();
                }
            }
        }

        static void Cabecalho()
        {
            Console.WriteLine("\n\tCifra de Transposição de Coluna");
        }

        static void Menu()
        {
            Console.WriteLine("\n\tEscolha uma opção:");
            Console.WriteLine("\t1 - Gerar Chave Aleatoriamente;");
            Console.WriteLine("\t2 - Digitar Chave;");
            Console.WriteLine("\t3 - Ver Lista dos Cacacteres Possíveis;");
            //Console.WriteLine("\t4 - Decifrar Texto;");
            Console.WriteLine("\t0 - Sair.");
            Console.Write("\n\tOPÇÃO: ");

            escolha = Console.ReadLine();
        }

        static void UtilizaCaracteresEspeciais()
        {
            string opcao = "";

            while (opcao.Trim().ToLower() != "s" && opcao.Trim().ToLower() != "n")
            {
                Console.Write("\n\tDeseja utilizar caracteres especiais na chave [s/n]?: ");
                opcao = Console.ReadLine();

                switch (opcao.Trim().ToLower())
                {
                    case "s":
                        utilizarCaracteresEspeciaisNaChave = true;
                        break;
                    case "n":
                        utilizarCaracteresEspeciaisNaChave = false;
                        break;
                    default:
                        Console.WriteLine("\n\tOPÇÃO INVÁLIDA!");
                        break;
                }
            }
        }

        static void LerChave()
        {
            bool chaveValida = false;

            while (!chaveValida)
            {
                bool mensagemExibida = false;
                Console.Write("\n\tDigite a Chave: ");
                chave = Console.ReadLine();

                chave = ObtemApenasCaracteresPermitidos(RemoverAcentos(chave.ToUpper()));

                // verifica se foi digitado algum caractere repetido
                string temp = "";

                foreach (char c in chave)
                {
                    if (temp.IndexOf(c) == -1)
                        temp += c;
                    else if (!mensagemExibida)
                    {
                        mensagemExibida = true;
                        Console.WriteLine("\n\tChave inválida! Não é possível inserir a mesma letra mais de uma vez na chave.");
                    }
                }

                // se a varável temporária for igual à chave digitada é porque não houve caracteres duplicados
                if (temp == chave)
                {
                    if (temp.Length > 12)
                        Console.WriteLine("\n\tChave inválida! Máximo de 12 caracteres.");
                    else
                    {
                        chaveValida = true;
                        quantidadeCaracteresChave = chave.Length;
                        Console.WriteLine($"\n\tChave que será utilizada: {chave}");
                    }
                }
            }
        }

        static void LerTamanhoDaChave()
        {
            bool isNumber = false;
            string output;

            while (!isNumber)
            {
                Console.Write($"\n\tDigite a quantidade de caracteres que a chave deve possuir (Max. {maxCaracChave}): ");
                output = Console.ReadLine();

                try
                {
                    quantidadeCaracteresChave = int.Parse(output);

                    if (quantidadeCaracteresChave <= 0)
                        throw new Exception();
                    else if (quantidadeCaracteresChave <= maxCaracChave)
                        isNumber = true;
                    else
                        Console.WriteLine($"\n\tVALOR EXCEDE TAMANHO MÁXIMO DE {maxCaracChave} CARACTERES.");

                }
                catch { Console.WriteLine("\n\tVALOR INVÁLIDO!!!"); }
            }
        }

        static void LerTexto()
        {
            texto = "";

            while (texto.Length <= 1)
            {
                Console.Write("\n\tDigite o texto: ");
                texto = Console.ReadLine();
            }

            textoSomenteLetras = ObtemApenasCaracteresPermitidos(RemoverAcentos(texto.ToUpper()));
            GerarTextoFinal();

            Console.WriteLine($"\n\tTexto Digitado: {texto}");
            Console.WriteLine($"\tTexto Para Transposição: {textoFinal}");
        }

        static void LerTextoCifrado()
        {
            textoCifrado = "";
            
            while (textoCifrado.Length <= 1)
            {
                Console.Write("\n\tDigite o texto cifrado: ");
                textoCifrado = Console.ReadLine();
            }
        }

        static void ExibirMatriz()
        {
            Console.Write("\n\tExibir Matriz [s/n]: ");
            escolha = Console.ReadLine();

            if (escolha.ToLower().Trim() == "s")
            {
                string retorno = $"\n\t{chave}\n";

                foreach (var linha in linhas)
                    retorno += $"\n\t{linha}";

                Console.WriteLine(retorno);
            }
        }

        static void ExibirResultado()
        {
            textoTransposicaoColuna = "";

            for (int i = 0; i < quantidadeCaracteresChave; i++)
            {
                int index = chave.IndexOf(chaveOrdenada[i]);

                textoTransposicaoColuna += $"{colunas[index]} ";
            }

            Console.WriteLine($"\n\tRESULTADO: {textoTransposicaoColuna}");
        }

        private static void DecifrarTexto()
        {
            try
            {
                string textoParaDecifrar = textoCifrado.Trim().Replace(" ", "");

                int qtdLinhas = textoParaDecifrar.Length / quantidadeCaracteresChave;

                List<string> linhaColuna = new List<string>();
                List<string> colunas = new List<string>();

                for (int i = 0; i < textoCifrado.Length; i += quantidadeCaracteresChave)
                {
                    string temp = textoParaDecifrar.Substring(i, quantidadeCaracteresChave);
                    linhaColuna.Add(temp);
                }


            }
            catch
            {
                Console.WriteLine("\n\tTexto não corresponde a chave informada!");
            }
        }

        private static void GerarTextoFinal()
        {
            textoFinal = textoSomenteLetras;
            int qtdCaracteresIncrementar = 0;

            while ((textoSomenteLetras.Length + qtdCaracteresIncrementar) % quantidadeCaracteresChave != 0)
                qtdCaracteresIncrementar++;

            if (qtdCaracteresIncrementar > 0)
                textoFinal += complementoTexto.Substring(0, qtdCaracteresIncrementar);
        }

        private static void GerarLinhas()
        {
            linhas = new List<string>();

            for (int i = 0; i < textoFinal.Length; i += quantidadeCaracteresChave)
            {
                string item = textoFinal.Substring(i, quantidadeCaracteresChave);

                linhas.Add(item);
            }
        }

        private static void GerarColunas()
        {
            colunas = new List<string>();

            for (int i = 0; i < quantidadeCaracteresChave; i++)
            {
                string coluna = "";
                foreach (string linha in linhas)
                {
                    coluna += linha[i];
                }

                colunas.Add(coluna);
            }
        }

        private static string GerarStringAleatoria(int quantidadeCaracteres, bool chave)
        {
            string result = "";

            List<char> caracPossiveis = new List<char>(chave ? caracteresPossiveisChave : caracteresPossiveisComplemento);

            while (result.Length < quantidadeCaracteres)
            {
                int randomIndex = new Random().Next(0, caracPossiveis.Count());

                result += caracPossiveis[randomIndex];

                if (chave)
                    caracPossiveis.Remove(caracPossiveis[randomIndex]);
            }

            Console.WriteLine($"\n\t{(chave ? "Chave Gerada" : "Complemento Gerado")}: {result}");

            return result;
        }

        private static string RemoverAcentos(string str)
        {
            StringBuilder sbReturn = new StringBuilder();
            var arrayText = str.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (char letter in arrayText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                    sbReturn.Append(letter);
            }

            return sbReturn.ToString();
        }

        private static string ObtemApenasCaracteresPermitidos(string str)
        {
            string retorno = "";

            foreach (char c in str)
                if (caracteresPossiveisChave.Contains(c))
                    retorno += c;

            return retorno;
        }

        private static List<char> ObterListaCaracteresPossiveisChave(bool caracteresEspeciais, bool registrarMaxCacacChave)
        {
            List<char> list = new List<char>();

            if (caracteresEspeciais)
            {
                for (int i = 33; i <= 96; i++)
                    list.Add(Convert.ToChar(i));
                for (int i = 123; i <= 126; i++)
                    list.Add(Convert.ToChar(i));
            }
            else
                for (int i = 65; i <= 90; i++)
                    list.Add(Convert.ToChar(i));

            if (registrarMaxCacacChave)
            {
                maxCaracChave = list.Count();
            }

            return list;
        }
    }
}
