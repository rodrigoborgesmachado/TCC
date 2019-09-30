using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using TCC.Util;

namespace TCC.Model
{
    public class MD_Retorno : MDN_Model
    {
        #region Atributos e Propriedades

        string path = "";
        /// <summary>
        /// [PATH] Campo de retorno referente ao parâmetro do JSON
        /// </summary>
        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
            }
        }

        string value = "";
        /// <summary>
        /// [VALUE] Campo de retorno referente ao value do JSON
        /// </summary>
        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        string tabelaIncremental = "INCREMENTAIS";

        #endregion Atributos e Propriedades

        #region Construtores

        /// <summary>
        /// Construtor principal da classe
        /// </summary>
        /// <param name="delete">Valida se exclui ou não a tabela</param>
        public MD_Retorno(bool delete): base()
        {
            CL_Files.WriteOnTheLog("MD_Retorno.MD_Retorno", Global.TipoLog.DETALHADO);
            base.table = new MDN_Table("RETORNO");
            this.table.Fields_Table.Add(new MDN_Campo("CODIGO", true, Util.Enumerator.DataType.STRING, "-", true, false, 1000, 0));
            this.table.CreateTable(delete);
        }

        /// <summary>
        /// Construtor secundário da classe
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="delete">Valida se exclui ou não a tabela</param>
        public MD_Retorno(string path, bool delete) : this(delete)
        {
            CL_Files.WriteOnTheLog("MD_Retorno.MD_Retorno", Global.TipoLog.DETALHADO);
            this.path = path.Replace(":", "").Replace(";", "").Replace(".", "").Replace(",","").Replace("!","").Replace("?","");
            CreateTableIncremental();
            AdicionaColuna();
            Load();
        }

        #endregion Construtores

        #region Métodos

        /// <summary>
        /// Método que pega o Incremental
        /// </summary>
        /// <returns></returns>
        public int Inc()
        {
            CL_Files.WriteOnTheLog("MD_Retorno.Inc", Global.TipoLog.DETALHADO);
            CreateTableIncremental();

            string sentenca = "SELECT CODIGO FROM " + tabelaIncremental;
            SQLiteDataReader reader = Util.DataBase.Select(sentenca);
            int r = 0;
            
            if (reader.Read())
            {
                r = reader.GetInt16(0);
                reader.Close();
                Util.DataBase.Execute("UPDATE " + tabelaIncremental + " SET CODIGO = CODIGO + 1");
            }
            return r;
        }

        /// <summary>
        /// Método que cria a tabela de incrementais
        /// </summary>
        public void CreateTableIncremental()
        {
            CL_Files.WriteOnTheLog("MD_Retorno.CreateTableIncremental", Global.TipoLog.DETALHADO);
            if (!ExisteTabelaIncrementais())
            {
                CriaTabelaIncrementais();
                PreencheTabelaIncrementais();
            }
        }

        /// <summary>
        /// Método que cria uma linha do incremental
        /// </summary>
        private void PreencheTabelaIncrementais()
        {
            CL_Files.WriteOnTheLog("MD_Retorno.PreencheTabelaIncrementais", Global.TipoLog.DETALHADO);
            string sentenca = "INSERT INTO " + tabelaIncremental + " (CODIGO) VALUES (0)";
            Util.DataBase.Insert(sentenca);
        }

        /// <summary>
        /// Cria tabela de incrementais
        /// </summary>
        private void CriaTabelaIncrementais()
        {
            CL_Files.WriteOnTheLog("MD_Retorno.CriaTabelaIncrementais", Global.TipoLog.DETALHADO);
            string sentenca = "CREATE TABLE " + tabelaIncremental + " (CODIGO INTEGER PRIMARY KEY)";
            Util.DataBase.Execute(sentenca);
        }

        /// <summary>
        /// Método que verifica a existência da tabela de incrementais
        /// </summary>
        /// <returns></returns>
        private bool ExisteTabelaIncrementais()
        {
            CL_Files.WriteOnTheLog("MD_Retorno.ExisteTabelaIncrementais", Global.TipoLog.DETALHADO);
            string sentenca = "SELECT 1 FROM " + tabelaIncremental;
            return Util.DataBase.Execute(sentenca);
        }

        /// <summary>
        /// Método que adiciona Coluna
        /// </summary>
        public void AdicionaColuna()
        {
            CL_Files.WriteOnTheLog("MD_Retorno.AdicionaColuna", Global.TipoLog.DETALHADO);
            if (!ExisteColuna())
            {
                CreateColuna();
            }
        }

        /// <summary>
        /// Método que cria a coluna
        /// </summary>
        /// <returns></returns>
        private bool CreateColuna()
        {
            CL_Files.WriteOnTheLog("MD_Retorno.CreateColuna", Global.TipoLog.DETALHADO);
            string sentenca = "ALTER TABLE " + this.table.Table_Name + " ADD " + path + " VARCHAR(4000)";
            return Util.DataBase.Execute(sentenca);
        }

        /// <summary>
        /// Método que verifica se a coluna existe
        /// </summary>
        /// <returns></returns>
        private bool ExisteColuna()
        {
            CL_Files.WriteOnTheLog("MD_Retorno.ExisteColuna", Global.TipoLog.DETALHADO);
            string sentenca = "SELECT " + path + " FROM " + this.table.Table_Name;
            return Util.DataBase.Execute(sentenca);
        }

        /// <summary>
        /// Método que faz o Load da classe
        /// </summary>
        public override void Load()
        {
            CL_Files.WriteOnTheLog("MD_Retorno.Load", Global.TipoLog.DETALHADO);
            string sentenca = "SELECT CODIGO FROM " + base.table.Table_Name + " WHERE CODIGO = '" + path + "'";
            SQLiteDataReader reader = Util.DataBase.Select(sentenca);
            if (reader == null)
            {
                Empty = true;
                return;
            }
                
            if (reader.HasRows)
            {
                Empty = false;
                reader.Close();
            }
        }

        /// <summary>
        /// Método que faz exclusão do objeto
        /// </summary>
        /// <returns>True - Sucesso; False - Erro</returns>
        public override bool Delete()
        {
            CL_Files.WriteOnTheLog("MD_Retorno.Delete", Global.TipoLog.DETALHADO);
            string sentenca = "DELETE FROM " + table.Table_Name + " WHERE CODIGO = '" + path + "'";
            return Util.DataBase.Delete(sentenca);
        }

        /// <summary>
        /// Método que faz o insert do objeto
        /// </summary>
        /// <returns>True - Insert feito com sucesso; False - Erro no insert</returns>
        public override bool Insert()
        {
            CL_Files.WriteOnTheLog("MD_Retorno.Insert", Global.TipoLog.DETALHADO);
            if (!(new MD_Retorno(path, false).Empty))
                return true;
            string sentenca = "INSERT INTO " + this.table.Table_Name + "(" + path + ") VALUES('" + value + "')";
            return Util.DataBase.Insert(sentenca);
        }

        /// <summary>
        /// Método que faz o update do objeto
        /// </summary>
        /// <returns></returns>
        public override bool Update()
        {
            CL_Files.WriteOnTheLog("MD_Retorno.Update", Global.TipoLog.DETALHADO);
            string sentenca = "UPDATE " + this.table.Table_Name + " SET VALUE = '" + value + "' WHERE PATH = '" + path + "'";
            return Util.DataBase.Update(sentenca);
        }

        #endregion Métodos

    }
}
