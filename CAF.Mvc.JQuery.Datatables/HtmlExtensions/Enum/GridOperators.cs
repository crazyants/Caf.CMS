using System;
namespace CAF.Mvc.JQuery.Datatables.Core
{
    [Flags]
    public enum GridOperators
    {
        Add=1,
        Edit=2,
        Delete=4,
        Search=8
    }
}