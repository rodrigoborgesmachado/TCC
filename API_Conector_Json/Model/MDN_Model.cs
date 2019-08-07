using System;
using System.Collections.Generic;
using System.Text;

namespace TCC.Model
{
    public abstract class MDN_Model
    {
        /// <summary>
        /// Control if the class is empty
        /// </summary>
        public bool Empty = true;

        /// <summary>
        /// Class's control table 
        /// </summary>
        public MDN_Table table;

        /// <summary>
        /// Class's main constructor
        /// </summary>
        public MDN_Model()
        {

        }

        /// <summary>
        /// Abstract method to be implement on the heirs class
        /// </summary>
        /// <returns>True - Sucess; False - Error</returns>
        public abstract bool Insert();

        /// <summary>
        /// Abstract method to be implement on the heirs class
        /// </summary>
        /// <returns>True - Sucess; False - Error</returns>
        public abstract bool Update();

        /// <summary>
        /// Abstract method to be implement on the heirs class
        /// </summary>
        /// <returns>True - Sucess; False - Error</returns>
        public abstract bool Delete();

        /// <summary>
        /// Abstract method to be implement on the heirs class
        /// </summary>
        public abstract void Load();
    }
}
