using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.WebUI
{
    public abstract class LinkedObjectBase<T>
    {
       
        public T Parent
        {
            get;
            protected internal set;
        }
        
        public T PreviousSibling
        {
            get;
            protected internal set;
        }
        
        public T NextSibling
        {
            get;
            protected internal set;
        }
       
    }
}
