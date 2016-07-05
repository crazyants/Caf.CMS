using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Mvc.JQuery.Datatables.Core.Editors.EditorUtil
{
    /// <summary>
    /// Container class to hold information about join details
    /// </summary>
    internal class LeftJoin
    {
        /// <summary>
        /// Join table name
        /// </summary>
        internal string Table;

        /// <summary>
        /// Table 1 field for the join
        /// </summary>
        internal string Field1;

        /// <summary>
        /// Table 2 field for the join
        /// </summary>
        internal string Field2;

        /// <summary>
        /// Join logic operator
        /// </summary>
        internal string Operator;


        /// <summary>
        /// Left join information container
        /// </summary>
        /// <param name="table">Join table name</param>
        /// <param name="field1">Table 1 field</param>
        /// <param name="op">Join logic operator</param>
        /// <param name="field2">Table 2 field</param>
        internal LeftJoin(string table, string field1, string op, string field2)
        {
            Table = table;
            Field1 = field1;
            Field2 = field2;
            Operator = op;
        }
    }
}
