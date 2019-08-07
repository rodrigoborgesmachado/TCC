using System;
using System.Collections.Generic;
using System.Text;

namespace TCC.Util
{
    public class Global
    {
        // Caminho principal da aplicação
        public static string app_main_directoty = System.IO.Directory.GetCurrentDirectory() + "\\";

        // Caminho da pasta de logs do sistema
        public static string app_logs_directoty = app_main_directoty + "Log\\";

        // Caminho da pasta de arquivos temporários
        public static string app_temp_directory = app_main_directoty + "TEMP\\";

        // Nome do diretório de saída
        public static string app_out_directory = app_main_directoty + "OUT\\";

        // Nome do diretório de configuração
        public static string app_conf_directory = app_main_directoty + "CONF\\";

        // Nome do arquivo de saída de extensão JSON
        public static string app_out_file_json = app_out_directory + "saida.json";

        // Nome do arquivo de saída de extensão XML
        public static string app_out_file_xml = app_out_directory + "saida.xml";

        // Nome do arquivo de saída de extensão CSV
        public static string app_out_file_csv = app_out_directory + "saida.csv";

        // Nome do arquivo sqlite de configuração
        public static string app_base_file = app_out_directory + "pckdb.db3";

        // Nome do arquivo html temporário
        public static string app_conf_file = app_conf_directory + "config.xml";

        /// <summary>
        /// Enumerador referente ao tipo de log que o sistema irá persistir
        /// </summary>
        public enum TipoLog
        {
            DETALHADO = 0,
            SIMPLES = 1
        }

        public static TipoLog log_system = TipoLog.SIMPLES;
    }
}
