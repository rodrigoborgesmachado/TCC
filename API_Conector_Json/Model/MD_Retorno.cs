using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

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
            string sentenca = "INSERT INTO " + tabelaIncremental + " (CODIGO) VALUES (0)";
            Util.DataBase.Insert(sentenca);
        }

        /// <summary>
        /// Cria tabela de incrementais
        /// </summary>
        private void CriaTabelaIncrementais()
        {
            string sentenca = "CREATE TABLE " + tabelaIncremental + " (CODIGO INTEGER PRIMARY KEY)";
            Util.DataBase.Execute(sentenca);
        }

        /// <summary>
        /// Método que verifica a existência da tabela de incrementais
        /// </summary>
        /// <returns></returns>
        private bool ExisteTabelaIncrementais()
        {
            string sentenca = "SELECT 1 FROM " + tabelaIncremental;
            return Util.DataBase.Execute(sentenca);
        }

        /// <summary>
        /// Método que adiciona Coluna
        /// </summary>
        public void AdicionaColuna()
        {
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
            string sentenca = "ALTER TABLE " + this.table.Table_Name + " ADD " + path + " VARCHAR(4000)";
            return Util.DataBase.Execute(sentenca);
        }

        /// <summary>
        /// Método que verifica se a coluna existe
        /// </summary>
        /// <returns></returns>
        private bool ExisteColuna()
        {
            string sentenca = "SELECT " + path + " FROM " + this.table.Table_Name;
            return Util.DataBase.Execute(sentenca);
        }

        /// <summary>
        /// Método que faz o Load da classe
        /// </summary>
        public override void Load()
        {
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
            string sentenca = "DELETE FROM " + table.Table_Name + " WHERE CODIGO = '" + path + "'";
            return Util.DataBase.Delete(sentenca);
        }

        /// <summary>
        /// Método que faz o insert do objeto
        /// </summary>
        /// <returns>True - Insert feito com sucesso; False - Erro no insert</returns>
        public override bool Insert()
        {
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
            string sentenca = "UPDATE " + this.table.Table_Name + " SET VALUE = '" + value + "' WHERE PATH = '" + path + "'";
            return Util.DataBase.Update(sentenca);
        }

        #endregion Métodos

    }
}
