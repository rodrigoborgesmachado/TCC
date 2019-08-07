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

        #endregion Atributos e Propriedades

        #region Construtores

        /// <summary>
        /// Construtor principal da classe
        /// </summary>
        /// <param name="delete">Valida se exclui ou não a tabela</param>
        public MD_Retorno(bool delete): base()
        {
            base.table = new MDN_Table("RETORNO");
            this.table.Fields_Table.Add(new MDN_Campo("PATH", true, Util.Enumerator.DataType.STRING, "-", true, false, 1000, 0));
            this.table.Fields_Table.Add(new MDN_Campo("VALUE", true, Util.Enumerator.DataType.STRING, "-", false, false, 1000, 0));
            this.table.CreateTable(delete);
        }

        /// <summary>
        /// Construtor secundário da classe
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="delete">Valida se exclui ou não a tabela</param>
        public MD_Retorno(string path, bool delete) : this(delete)
        {
            this.path = path;
            Load();
        }

        #endregion Construtores

        #region Métodos

        /// <summary>
        /// Método que faz o Load da classe
        /// </summary>
        public override void Load()
        {
            string sentenca = "SELECT PATH, VALUE FROM " + base.table.Table_Name + " WHERE PATH = '" + path + "'";
            SQLiteDataReader reader = Util.DataBase.Select(sentenca);
            if (reader == null)
            {
                Empty = true;
                return;
            }
                
            if (reader.HasRows)
            {
                value = reader.GetString(1);
                Empty = false;
            }
        }

        /// <summary>
        /// Método que faz exclusão do objeto
        /// </summary>
        /// <returns>True - Sucesso; False - Erro</returns>
        public override bool Delete()
        {
            string sentenca = "DELETE FROM " + table.Table_Name + " WHERE PATH = '" + path + "'";
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
            string sentenca = "INSERT INTO " + this.table.Table_Name + "(PATH, VALUE) VALUES('" + path + "', '" + value + "')";
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
