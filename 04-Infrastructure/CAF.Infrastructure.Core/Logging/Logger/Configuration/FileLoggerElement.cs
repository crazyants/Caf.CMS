#region License and copyright notice
/* 
 * Kaliko Logger
 * 
 * Copyright (c) 2011 Fredrik Schultz
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 */
#endregion

namespace CAF.Infrastructure.Core.Logger.Configuration
{
    using System.Configuration;

    public class FileLoggerElement : ConfigurationElement {

        [ConfigurationProperty("filename", IsRequired = true)]
        public string Filename {
            get { return (string)base["filename"]; }
        }

        [ConfigurationProperty("treshold", DefaultValue = Logger.Severity.Minor)]
        public Logger.Severity Treshold {
            get { return (Logger.Severity)base["treshold"]; }
        }
    }
}