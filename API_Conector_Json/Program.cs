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
            CL_Files.WriteOnTheLog("-------------------------Iniciando", Global.TipoLog.SIMPLES);
            string command = "";
            string saida = "";
#if !Debug
            if (args.Length < 2)
            {
                CL_Files.WriteOnTheLog("There are no suficient arguments", Global.TipoLog.SIMPLES);
                CL_Files.WriteOnTheLog("There are no suficient arguments. Need a -i 'file in' and -o 'directory out'", Global.TipoLog.SIMPLES);
                Console.WriteLine("There are no suficient arguments.\n-i 'file in (.json)' \n-o 'directory out' \n-t 'type of file out (.xml;.sql;.csv;.json)'\n-l for logs", Global.TipoLog.SIMPLES);

                //return;
            }

            Document.Saida out_files = new Document.Saida();
            out_files.sqlite = true;
            out_files.csv    = true;
            out_files.json   = true;
            out_files.xml    = true;

            if (!CarregaParametros(args, ref command, ref saida, ref out_files))
            {
                Console.WriteLine("Erro ao carregar os parâmetros. Log:" + Global.app_logs_directoty);

                TCC.Util.CL_Files.WriteOnTheLog("Erro ao carregar os parâmetros.", Global.TipoLog.SIMPLES);
                return;
            }
            CL_Files.WriteOnTheLog("Arquivo de Entrada: " + command, Global.TipoLog.DETALHADO);
#else
            CarregaComando("C:\\Users\\rodri\\OneDrive\\Área de Trabalho\\TCC\\python\\teste.json", ref command);
#endif

            DataBase.OpenConnection();
            Document document = new Document(command, out_files);
            string mensagemErro = "";

            if (!document.Criar(ref mensagemErro))
            {
                Console.WriteLine(mensagemErro);
            }
            CopiaSaida(saida);

            CL_Files.WriteOnTheLog("-------------------------Finalizado", Global.TipoLog.SIMPLES);
        }

        /// <summary>
        /// Método que carrega os parâmetros de entrada
        /// </summary>
        /// <param name="args"></param>
        /// <param name="command"></param>
        static bool CarregaParametros(string[] args, ref string command, ref string caminho_saida, ref Document.Saida out_files)
        {
            CL_Files.WriteOnTheLog("Program.CarregaParametros", Global.TipoLog.DETALHADO);

            command = "";
            bool entrada = false, saida = false, types = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToUpper().Equals("-I"))
                {
                    entrada = true;
                }
                else if (args[i].ToUpper().Equals("-O"))
                {
                    saida = true;
                }
                else if (args[i].ToUpper().Equals("-T"))
                {
                    types = true;
                    out_files.csv = out_files.json = out_files.sqlite = out_files.xml = false;
                }
                else if (args[i].ToUpper().Equals("-L"))
                {
                    // Seta o tipo de log para detalhado (averiguação de problemas)
                    Global.log_system = Global.TipoLog.DETALHADO;
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
                else if (types)
                {
                    types = false;
                    string lista = args[i].ToString();
                    foreach(string typ in lista.Split(';'))
                    {
                        switch (typ)
                        {
                            case ".json":
                                out_files.json = true;
                                break;
                            case ".xml":
                                out_files.xml = true;
                                break;
                            case ".csv":
                                out_files.csv = true;
                                break;
                            case ".sql":
                                out_files.sqlite = true;
                                break;
                            default:
                                break;
                        }
                    }
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
