using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
 

namespace CAF.WebSite.Application.Services.Tables
{
   
    /// <summary>
    /// Interface that should be implemented by each Grid
    /// </summary>
    public partial interface IGridRegistration
    {
        /// <summary>
        /// Register Grids
        /// </summary>
        void RegisterGrids();
    }
}
