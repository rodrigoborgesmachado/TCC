using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Model
{
    public class MDN_Table
    {
        #region Atributes

        string table_name;

        /// <summary>
        /// Table's name
        /// </summary>
        public string Table_Name
        {
            get
            {
                return table_name;
            }
            set
            {
                this.table_name = value;
            }
        }

        List<MDN_Campo> fields_Table = new List<MDN_Campo>();
        /// <summary>
        /// List of the table's fields
        /// </summary>
        public List<MDN_Campo> Fields_Table
        {
            set
            {
                this.fields_Table = value;
            }
            get
            {
                return this.fields_Table;
            }
        }

        int level = 0;
        /// <summary>
        /// Table's Relationship level (the greater more idependent the table be)
        /// </summary>
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                this.level = value;
            }
        }

        /// <summary>
        /// Data table to representes the table on the data base
        /// </summary>
        DataTable data_table = new DataTable();

        #endregion Atributes

        #region Construtores

        /// <summary>
        /// Construtor principal da classe
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lista"></param>
        /// <param name="nivel"></param>
        public MDN_Table(string name)
        {
            this.table_name = name;
        }

        #endregion Construtores

        #region Methods 

        /// <summary>
        /// Method that make the creation of the table
        /// </summary>
        /// <param name="delete">Identify if delete the values on the table if it exists</param>
        /// <param name="num_lines">Number of lines to be fill on the table if 'fill' is true</param>
        public void CreateTable(bool delete)
        {
            if (delete) this.DeleteTable();

            if (!ExistsTable())
            {
                string command = "CREATE TABLE " + this.Table_Name + " (";
                int qt = fields_Table.Count, i = 0;
                foreach (MDN_Campo field in fields_Table)
                {
                    i++;
                    command += field.InsertFieldDataBase() + (i != qt ? ", " : "");
                }

                string primary = CreateCommandPrimaryKey();
                if (!string.IsNullOrEmpty(primary))
                    command += ", " + primary;

                command += ")";

                Util.DataBase.CreateTable(command);
            }

            data_table.Clear();
            foreach (MDN_Campo field in fields_Table)
            {
                data_table.Columns.Add(new DataColumn(field.Name_Field));
            }

            if (!Table_Name.Equals("LDXED"))
            {
                //FillLib();
            }
        }

        /// <summary>
        /// Method that create the dictionary of the table
        /// </summary>
        public void FillLib()
        {
            string insert_base = "INSERT INTO LDXED (tab,field,type,size,prec,flags) VALUES (";
            int i = fields_Table.Count;

            foreach (MDN_Campo field in fields_Table)
            {
                string insert = insert_base + "'" + this.Table_Name + "'";
                insert += ", '" + field.Name_Field + "'";
                switch (field.Type)
                {
                    case Util.Enumerator.DataType.CHAR:
                        insert += ",'C'";
                        break;
                    case Util.Enumerator.DataType.STRING:
                        insert += ",'C'";
                        break;
                    case Util.Enumerator.DataType.DATE:
                        insert += ",'T'";
                        break;
                    case Util.Enumerator.DataType.DECIMAL:
                        insert += ",'F'";
                        break;
                    case Util.Enumerator.DataType.INT:
                        insert += ",'I'";
                        break;
                }
                insert += "," + field.Size;
                insert += "," + field.Precision;
                insert += ",0)";

                Util.DataBase.Insert(insert);
            }
        }

        /// <summary>
        /// Method how creates the command of primary key. Is usefull when the system is creating the database
        /// </summary>
        /// <returns>Command</returns>
        private string CreateCommandPrimaryKey()
        {
            string command_sql = "";
            int qt = 0;

            foreach (MDN_Campo field in fields_Table)
            {
                if (field.PrimaryKey)
                {
                    if (qt == 0)
                        command_sql = " PRIMARY KEY ( ";
                    else
                        command_sql += ", ";

                    command_sql += field.Name_Field + " ";
                    qt++;
                }
            }

            if (!string.IsNullOrEmpty(command_sql))
                command_sql += " )";

            return command_sql;
        }

        /// <summary>
        /// Method how make the DELETE on the database
        /// </summary>
        /// <returns>true - Sucess; false - Error</returns>
        public bool DeleteTable()
        {
            if (ExistsTable())
            {
                string command = "DROP TABLE " + this.table_name;
                return Util.DataBase.Execute(command);
            }
            return true;
        }

        /// <summary>
        /// Method that verify if the table on the database is equal if the data table on the system
        /// </summary>
        /// <param name="database_directory">Disrectory of the database</param>
        /// <returns>True - correct; False - error</returns>
        public bool CheckDataBaseWithDataTable(string database_directory)
        {
            try
            {
                Util.DataBase.CloseConnection();
                if (Util.DataBase.OpenConnection(database_directory))
                {
                    bool were_error = false;
                    string command = CreateCommandSQLTable();
                    SQLiteDataReader reader = Util.DataBase.Select(command);
                    DataTable table = new DataTable();

                    // Verify if all colluns were created on the database
                    foreach (DataColumn collumn in data_table.Columns)
                    {
                        string field = collumn.ColumnName;
                        int i = 0;
                        bool match = false;
                        for (i = 0; i < fields_Table.Count && !match; i++)
                        {
                            if (reader.GetName(i).ToUpper().Equals(field.ToUpper()))
                            {
                                table.Columns.Add(field);
                                match = true;
                            }
                        }

                        if (!match)
                        {
                            were_error = true;
                        }
                    }

                    List<string> list = new List<string>();
                    while (reader.Read())
                    {
                        list = new List<string>();
                        foreach (DataColumn collumn in data_table.Columns)
                        {
                            list.Add(reader[collumn.ColumnName].ToString());
                        }
                        table.Rows.Add(list.ToArray());
                    }

                    for (int i = 0; i < data_table.Rows.Count; i++)
                    {
                        List<object> l1 = data_table.Rows[i].ItemArray.ToList();
                        List<object> l2 = table.Rows[i].ItemArray.ToList();

                        if (l1.Count != l2.Count)
                        {
                            were_error = true;
                            break;
                        }

                        for (int j = 0; j < l1.Count; j++)
                        {
                            if (l1[j].ToString().Replace(',', '.') != l2[j].ToString().Replace(',', '.'))
                            {
                                were_error = true;
                                break;
                            }
                        }
                        if (were_error)
                            break;
                    }

                    if (were_error)
                    {
                        Util.CL_Files.WriteOnTheLog("[TESTE_ZFX_2]The new database don't mach!", Util.Global.TipoLog.SIMPLES);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Util.CL_Files.WriteOnTheLog("[TESTE_ZFX_2]Error comparing datatable.Error: " + e.Message, Util.Global.TipoLog.SIMPLES);
            }
            return false;
        }

        /// <summary>
        /// Method that creates the command for select in table
        /// </summary>
        /// <returns>Command SQL</returns>
        public string CreateCommandSQLTable()
        {
            string command = " SELECT ";
            string fields = "";
            int qt = fields_Table.Count, i = 1;

            foreach (MDN_Campo field in fields_Table)
            {
                fields += field.Name_Field;
                if (i < qt)
                    fields += ", ";
                i++;
            }
            command += fields + " FROM " + this.Table_Name;
            return command;
        }

        /// <summary>
        /// Método que valida se a tabela existe
        /// </summary>
        /// <returns></returns>
        public bool ExistsTable()
        {
            return Util.DataBase.ExistsTable(this.Table_Name);
        }

        #endregion Methods 
    }
}
