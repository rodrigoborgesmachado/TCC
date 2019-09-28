using System;
using System.Collections.Generic;
using System.IO;
using TCC.Util;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace API_Conector_Json
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            CL_Files.CreateMainDirectories();
            string command = "";
            string saida = "";
#if !Debug
            if (args.Length < 4)
            {
                CL_Files.WriteOnTheLog("There are no suficient arguments", Global.TipoLog.SIMPLES);
                Console.WriteLine("There are no suficient arguments. Need a -i 'file in' and -o 'directory out'");

                return;
            }

            
            if (!CarregaParametros(args, ref command, ref saida))
            {
                Console.WriteLine("Erro ao carregar os parâmetros. Log:" + Global.app_logs_directoty);

                TCC.Util.CL_Files.WriteOnTheLog("Erro ao carregar os parâmetros.", Global.TipoLog.SIMPLES);
                return;
            }
#else
            CarregaComando("C:\\Users\\rodri\\OneDrive\\Área de Trabalho\\TCC\\python\\teste.json", ref command);
#endif

            DataBase.OpenConnection();
            Document document = new Document(command, true, true, true, true);
            string mensagemErro = "";

            if (!document.Criar(ref mensagemErro))
            {
                Console.WriteLine(mensagemErro);
            }
            CopiaSaida(saida);

        }

        /// <summary>
        /// Método que carrega os parâmetros de entrada
        /// </summary>
        /// <param name="args"></param>
        /// <param name="command"></param>
        static bool CarregaParametros(string[] args, ref string command, ref string caminho_saida)
        {
            command = "";
            bool entrada = false, saida = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("-i"))
                {
                    entrada = true;
                }
                else if (args[i].Equals("-o"))
                {
                    saida = true;
                }
                else if (entrada)
                {
                    if (!CarregaComando(args[i], ref command))
                    {
                        return false;
                    }
                    entrada = false;
                }
                else if (saida)
                {
                    caminho_saida = args[i];
                    saida = false;
                }
            }
            return true;
        }

        /// <summary>
        /// Método que carrega o comando no arquivo de entrada
        /// </summary>
        /// <param name="command"></param>
        static bool CarregaComando(string directory, ref string command)
        {
            try
            {
                StreamReader reader = new StreamReader(directory);
                command = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
            }
            catch (Exception e)
            {
                CL_Files.WriteOnTheLog("Erro ao abrir o arquivo de entrada. Erro: " + e.Message, Global.TipoLog.SIMPLES);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Método que copia os arquivos de saída
        /// </summary>
        /// <param name="saida">Diretório para os arquivos de saída</param>
        static void CopiaSaida(string saida)
        {
            try
            {
                if (string.IsNullOrEmpty(saida))
                {
                    return;
                }

                if (!Directory.Exists(saida))
                {
                    Directory.CreateDirectory(saida);
                }

                File.Copy(Global.app_base_file, saida +  "\\saida.db3");
                File.Copy(Global.app_out_file_csv, saida + "\\saida.csv");
                File.Copy(Global.app_out_file_json, saida + "\\saida.json");
                File.Copy(Global.app_out_file_xml, saida + "\\saida.xml");
            }
            catch (Exception e)
            {
                CL_Files.WriteOnTheLog("Erro ao copiar os arquivos de saída. Erro: " + e.Message, Global.TipoLog.SIMPLES);
            }
        }

    }
}
