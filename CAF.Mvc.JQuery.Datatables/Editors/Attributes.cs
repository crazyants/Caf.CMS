// <copyright>Copyright (c) 2014 SpryMedia Ltd - All Rights Reserved</copyright>
//
// <summary>
// Attributes that can be used for properties in Editor models
// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Mvc.JQuery.Datatables.Core.Editors
{
    /// <summary>
    /// Define an error message for cases where the data given cannot be
    /// stored in parameter type.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class EditorTypeErrorAttribute : System.Attribute
    {
        /// <summary>
        /// Error message
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// Constructor for a field attribute defining an error message
        /// </summary>
        /// <param name="msg">Error message</param>
        public EditorTypeErrorAttribute(string msg)
        {
            Msg = msg;
        }
    }

    /// <summary>
    /// Define the HTTP name (used for both the JSON and incoming HTTP values
    /// on form submit) for the field defined by the parameter. The parameter
    /// name is used as the database column name automatically.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class EditorHttpNameAttribute : System.Attribute
    {
        /// <summary>
        /// Field's HTTP name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Constructor for a field attribute defining the HTTP name for a property
        /// </summary>
        /// <param name="name">HTTP name</param>
        public EditorHttpNameAttribute(string name)
        {
            Name = name;
        }
    }
}
