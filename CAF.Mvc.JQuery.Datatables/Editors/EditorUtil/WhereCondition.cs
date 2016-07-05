using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Mvc.JQuery.Datatables.Core.Editors.EditorUtil
{
    internal class WhereCondition
    {
        public string Key { get; set; }
        public dynamic Value { get; set; }
        public string Operator { get; set; }
        public Action<Query> Custom { get; set; }
    }
}
