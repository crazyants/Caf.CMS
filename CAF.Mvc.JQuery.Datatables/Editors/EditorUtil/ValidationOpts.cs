// <copyright>Copyright (c) 2014 SpryMedia Ltd - All Rights Reserved</copyright>
//
// <summary>
// Options to configure a validation method
// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Mvc.JQuery.Datatables.Core.Editors
{
    /// <summary>
    /// Common validation options that can be specified for all validation methods.
    /// </summary>
    public class ValidationOpts
    {
        /// <summary>
        /// Error message should the validation fail
        /// </summary>
        public string Message = "Input not valid";

        /// <summary>
        /// Allow a field to be empty, i.e. a zero length string -
        /// `''` (`true` - default) or require it to be non-zero length (`false`).
        /// </summary>
        public bool Empty = true;

        /// <summary>
        /// Require the field to be submitted (`false`) or not
        /// (`true` - default). When set to `true` the field does not need to be
        /// included in the list of parameters sent by the client - if set to `false`
        /// then it must be included. This option can be particularly useful in Editor
        /// as Editor will not set a value for fields which have not been submitted -
        /// giving the ability to submit just a partial list of options.
        /// </summary>
        public bool Optional = true;


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Public static methods
         */

        /// <summary>
        /// Check to see if validation options have been given. If not, create
        /// and instance with the default options and return that. This is
        /// useful for checking to see if a user has passed in validation options
        /// or not, since a new instance can't be a default parameter value.
        /// </summary>
        /// <param name="user">User set validation options or null</param>
        /// <returns>Validation options to use for the validation</returns>
        public static ValidationOpts Select(ValidationOpts user)
        {
            return user != null ?
                user :
                new ValidationOpts();
        }
    }
}
