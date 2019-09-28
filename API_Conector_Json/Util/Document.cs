using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using TCC.Model;

namespace TCC.Util
{
    public class Document
    {
        #region Atributos e Propriedades

        string requisicao = "";
        /// <summary>
        /// Requisição a ser feita
        /// </summary>
        public string Requisicao
        {
            get
            {
                return requisicao;
            }
        }

        bool save_as_csv = false;
        /// <summary>
        /// Identifica se irá salvar como csv
        /// </summary>
        public bool Save_as_csv
        {
            get
            {
                return save_as_csv;
            }
            set
            {
                save_as_csv = value;
            }
        }

        bool save_as_xml = false;
        /// <summary>
        /// Identifica se irá salvar como xml
        /// </summary>
        public bool Save_as_xml
        {
            get
            {
                return save_as_xml;
            }
            set
            {
                save_as_xml = value;
            }
        }

        bool save_as_json = false;
        /// <summary>
        /// Identifica se irá salvar como json
        /// </summary>
        public bool Save_as_json
        {
            get
            {
                return save_as_json;
            }
            set
            {
                save_as_json = value;
            }
        }

        bool save_on_bd = false;
        /// <summary>
        /// Identifica se irá salvar no banco de dados
        /// </summary>
        public bool Save_on_bd
        {
            get
            {
                return save_on_bd;
            }
            set
            {
                save_on_bd = value;
            }
        }

        #endregion Atributos e Propriedades

        #region Construtores

        /// <summary>
        /// Construtor principal da classe
        /// </summary>
        /// <param name="requisicao">Comando de requisição a ser feita</param>
        public Document(string requisicao)
        {
            this.requisicao = requisicao;
        }

        /// <summary>
        /// Construtor principal da classe
        /// </summary>
        /// <param name="requisicao">Comando de requisição a ser feita</param>
        public Document(string requisicao, bool save_result_as_xml, bool save_result_as_csv, bool save_result_as_json, bool save_result_on_dataBase)
        {
            this.requisicao = requisicao;
            save_as_xml = save_result_as_xml;
            save_as_json = save_result_as_json;
            save_as_csv = save_result_as_csv;
            save_on_bd = save_result_on_dataBase;
        }

        #endregion Construtores

        #region Métodos

        /// <summary>
        /// Método que cria o relatório final
        /// </summary>
        /// <returns></returns>
        public bool Criar(ref string mensagemErro)
        {
            mensagemErro = "";
            try
            {

                //string responseFromServer = File.ReadAllText("C:\\Users\\Rodrigo\\Desktop\\entrada.json");
                string responseFromServer = this.requisicao;
                
                //if(!FazRequisicao(ref mensagemErro, ref responseFromServer))
                //{
                //    return false;
                //}

                Save(responseFromServer, ref mensagemErro);
            }
            catch (Exception e)
            {
                Util.CL_Files.WriteOnTheLog("Erro na requisição: " + e.Message, Util.Global.TipoLog.SIMPLES);

                mensagemErro = "Erro: " + e.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Método que faz a requisição e retorno uma string com o Json
        /// </summary>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool FazRequisicao(ref string mensagemErro, ref string retorno)
        {
            mensagemErro = retorno = "";

            try
            {
                // Create a request for the URL. 		
                WebRequest request = WebRequest.Create(requisicao);
                // If required by the server, set the credentials.
                request.Credentials = CredentialCache.DefaultCredentials;
                // Get the response.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                retorno = reader.ReadToEnd();

                // Cleanup the streams and the response.
                reader.Close();
                dataStream.Close();
                response.Close();
            }
            catch(Exception e)
            {
                Util.CL_Files.WriteOnTheLog("Erro na requisição: " + e.Message, Util.Global.TipoLog.SIMPLES);

                mensagemErro = "Erro: " + e.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Método que salva a resposta do servidor
        /// </summary>
        /// <param name="responseFromServer">Resposta JSON do servidor a ser salva</param>
        /// <returns></returns>
        private bool Save(string responseFromServer, ref string mensagemErro)
        {
            mensagemErro = "";
            List<string> colunas = new List<string>();
            List<string> dados = new List<string>();

            MontaDados(responseFromServer, ref colunas, ref dados);
            try
            {
                if (Save_as_csv)
                {
                    if(!SaveResultAsCsv(colunas, dados, ref mensagemErro))
                    {
                        return false;
                    }
                }
                if (Save_as_xml)
                {
                    if (!SaveResultAsXML(colunas, dados, ref mensagemErro))
                    {
                        return false;
                    }
                }
                if (Save_as_json)
                {
                    if (!SaveResultAsJSON(responseFromServer, ref mensagemErro))
                    {
                        return false;
                    }
                }
                if (Save_on_bd)
                {
                    if (!SaveResultOnBD(colunas, dados, ref mensagemErro))
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                mensagemErro = e.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Método que monta os dados de colunas e informações da resposta do servidor
        /// </summary>
        /// <param name="responseFromServer">Resposta json do servidor</param>
        /// <param name="colunas">Colunas referentes aos parâmetros do json</param>
        /// <param name="dados">Dados referente ao JSON</param>
        public void MontaDados(string responseFromServer, ref List<string> colunas, ref List<string> dados)
        {
            colunas = new List<string>();

            List<JObject> response = JsonConvert.DeserializeObject<List<JObject>>(responseFromServer);

            foreach (dynamic en in response)
            {
                foreach (JToken o in en.PropertyValues())
                {
                    if (!colunas.Contains(o.Path.ToString().Replace(":", "").Replace(";", "").Replace(".", "").Replace(",", "")))
                    {
                        colunas.Add(o.Path.ToString().Replace(":", "").Replace(";","").Replace(".","").Replace(",",""));
                    }
                    dados.Add(o.ToString());
                }
                response = null;
            }
        }

        /// <summary>
        /// Método que salva o resultado da requisição do servidor
        /// </summary>
        /// <param name="colunas">Parêmetros do arquivo JSON</param>
        /// <param name="dados">Dados do JSON</param>
        /// <param name="mensagemErro">Variável para armazenar algum possível erro</param>
        /// <returns>True - sucesso; False - erro</returns>
        public bool SaveResultAsCsv(List<string> colunas,List<string> dados, ref string mensagemErro)
        {
            mensagemErro = "";

            try
            {
                if (File.Exists(Global.app_out_file_csv))
                    File.Delete(Global.app_out_file_csv);
                
                string texto = "";
                bool first = true;
                foreach(string coluna in colunas)
                {
                    if(first)
                        texto += coluna.Replace("\t", "").Replace("\n", "");
                    else
                        texto += ";" + coluna.Replace("\t", "").Replace("\n", "");
                    first = false;
                }

                texto += "\n";
                first = true;
                int i = 0;
                foreach (string dado in dados)
                {
                    i++;
                    if (first)
                        texto += dado.Replace("\t", "").Replace("\n", "");
                    else
                        texto += ";" + dado.Replace("\t", "").Replace("\n", "");

                    if (i == colunas.Count)
                    {
                        texto += "\n";
                    }

                    first = false;
                }

                File.AppendAllText(Global.app_out_file_csv, texto);
            }
            catch(Exception e)
            {
                mensagemErro = e.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Método que salva o resultado da requisição do servidor
        /// </summary>
        /// <param name="colunas">Parêmetros do arquivo JSON</param>
        /// <param name="dados">Dados do JSON</param>
        /// <param name="mensagemErro">Variável para armazenar algum possível erro</param>
        /// <returns>True - sucesso; False - erro</returns>
        public bool SaveResultAsXML(List<string> colunas, List<string> dados, ref string mensagemErro)
        {
            mensagemErro = "";
            string[] col = new string[colunas.Count];
            string[] inf = new string[dados.Count];
            col = colunas.ToArray();
            inf = dados.ToArray();

            try
            {
                if (File.Exists(Global.app_out_file_xml))
                    File.Delete(Global.app_out_file_xml);

                string texto = "<?xml version=\"1.0\" encoding=\"UTF - 8\"?>\n";
                texto +=       "<json>\n";
                
                for(int i = 0, j = 0; i < dados.Count; i++)
                {
                    if (j == 0)
                    {
                        texto += "\t<tweet>\n";
                    }

                    texto +=    "\t\t<" + col[j].Replace("\t", "").Replace("\n", "") + ">" + 
                                inf[i].Replace("\t", "").Replace("\n", "") +
                                "</" + col[j].Replace("\t", "").Replace("\n", "") + ">\n";
                    j++;
                    if (j == colunas.Count)
                    {
                        texto += "\t</tweet>\n";
                        j = 0;
                    }
                }

                texto +=       "</json>";
                
                File.AppendAllText(Global.app_out_file_xml, texto);
            }
            catch (Exception e)
            {
                mensagemErro = e.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Método que salva o resultado da requisição do servidor
        /// </summary>
        /// <param name="response">Resposta JSON do servidor</param>
        /// <param name="mensagemErro">Variável para armazenar algum possível erro</param>
        /// <returns>True - sucesso; False - erro</returns>
        public bool SaveResultAsJSON(string response, ref string mensagemErro)
        {
            mensagemErro = "";

            try
            {
                if (File.Exists(Global.app_out_file_json))
                    File.Delete(Global.app_out_file_json);
                File.AppendAllText(Global.app_out_file_json, response);
            }
            catch (Exception e)
            {
                mensagemErro = e.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Método que salva o resultado da requisição do servidor
        /// </summary>
        /// <param name="colunas">Parêmetros do arquivo JSON</param>
        /// <param name="dados">Dados do JSON</param>
        /// <param name="mensagemErro">Variável para armazenar algum possível erro</param>
        /// <returns>True - sucesso; False - erro</returns>
        public bool SaveResultOnBD(List<string> colunas, List<string> dados, ref string mensagemErro)
        {
            mensagemErro = "";
            ///Drop e cria a tabela
            MD_Retorno r = new MD_Retorno(true);
            r = null;
            try
            {
                string[] col = new string[colunas.Count];
                string[] inf = new string[dados.Count];
                col = colunas.ToArray();
                inf = dados.ToArray();
                string comando = "INSERT INTO RETORNO (";
                string values = "VALUES (";

                for (int i = 0;i < colunas.Count; i++)
                {
                    MD_Retorno retorno = new MD_Retorno(col[i], false);
                    comando += (i == 0 ? "CODIGO, " + retorno.Path : ", " + retorno.Path);
                    values += (i == 0 ? retorno.Inc() + ", '" + inf[0] + "'" : ",'" + inf[i].Replace("'", "") + "'");
                    retorno = null;
                }
                comando += ")";
                values += ")";
                Util.DataBase.Insert(comando + values);
                values = "VALUES (";

                for (int j = colunas.Count, i = 0; j < dados.Count; j++)
                {
                    MD_Retorno retorno = new MD_Retorno(col[i], false);
                    values += (i == 0 ? retorno.Inc() + ", '" + inf[j] + "'" : ",'" + inf[j].Replace("'", "") + "'");

                    i++;
                    if (i == colunas.Count)
                    {
                        values += ")";
                        Util.DataBase.Insert(comando + values);
                        values = "VALUES (";
                        i = 0;
                    }
                }

                Util.DataBase.Execute("DROP TABLE INCREMENTAIS");
            }
            catch (Exception e)
            {
                mensagemErro = e.Message;
                return false;
            }
            return true;
        }

        #endregion Métodos
    }
}
