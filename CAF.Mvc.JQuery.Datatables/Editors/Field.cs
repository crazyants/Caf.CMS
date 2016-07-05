// <copyright>Copyright (c) 2014 SpryMedia Ltd - All Rights Reserved</copyright>
//
// <summary>
// Field class which defines how individual fields for Editor
// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Text.RegularExpressions;

using CAF.Mvc.JQuery.Datatables.Core.Editors.EditorUtil;

namespace CAF.Mvc.JQuery.Datatables.Core.Editors
{
    /// <summary>
    /// Field definitions for the DataTables Editor.
    ///
    /// Each Database column that is used with Editor can be described with this 
    /// Field method (both for Editor and Join instances). It basically tells
    /// Editor what table column to use, how to format the data and if you want
    /// to read and/or write this column.
    /// </summary>
    public class Field
    {

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Statics
         */

        /// <summary>
        /// Set options for the <code>Set()</code> method, controlling when this
        /// field's value is set on the database
        /// </summary>
        public enum SetType
        {
            /// <summary>
            /// Never set this field's value
            /// </summary>
            None,

            /// <summary>
            /// Set the value on both create and edit actions
            /// </summary>
            Both,

            /// <summary>
            /// Set the value on only the create action
            /// </summary>
            Create,

            /// <summary>
            /// Set the value on only the edit action
            /// </summary>
            Edit
        };


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Constructor
         */

        /// <summary>
        /// Create a new Field instance - common db name and http name
        /// </summary>
        /// <param name="dbField">Database name for the field. This is also used as the HTTP name for the field</param>
        public Field(string dbField)
        {
            Name(dbField);
            DbField(dbField);
        }

        /// <summary>
        /// Create a new Field instance - different db and http names
        /// </summary>
        /// <param name="dbField">Database name for the field</param>
        /// <param name="name">HTTP name (JSON data and form submit)</param>
        public Field(string dbField, string name)
        {
            Name(name);
            DbField(dbField);
        }

        /// <summary>
        /// Create a new Field instance - common db name and http name with type specified
        /// </summary>
        /// <param name="dbField">Database name for the field. This is also used as the HTTP name for the field</param>
        /// <param name="type">Type that the value should take</param>
        /// <param name="typeError">Error message if the field's value cannot be cast to the given type</param>
        public Field(string dbField = null, Type type = null, string typeError = null)
        {
            if (dbField != null)
            {
                Name(dbField);
                DbField(dbField);
            }

            if (type != null)
            {
                Type(type);
            }

            if (typeError != null)
            {
                TypeError(typeError);
            }
        }


        /// <summary>
        /// Create a new Field instance - different db and http names with type specified
        /// </summary>
        /// <param name="dbField">Database name for the field. This is also used as the HTTP name for the field</param>
        /// <param name="name">HTTP name (JSON data and form submit)</param>
        /// <param name="type">Type that the value should take</param>
        /// <param name="typeError">Error message if the field's value cannot be cast to the given type</param>
        public Field(string dbField, string name = null, Type type = null, string typeError = null)
        {
            if (dbField != null && name == null)
            {
                Name(dbField);
                DbField(dbField);
            }
            else
            {
                Name(name);
                DbField(dbField);
            }

            if (type != null)
            {
                Type(type);
            }

            if (typeError != null)
            {
                TypeError(typeError);
            }
        }


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Private parameters
         */
        private string _DbField = null;
        private bool _Get = true;
        private Func<dynamic, Dictionary<string, object>, dynamic> _GetFormatter;
        private dynamic _GetValue = null;
        private string _Name = null;
        private Type _Type = typeof(string);
        private string _TypeError = "Type error - Data set for {fieldName} is not of type {typeName}.";
        private SetType _Set = Field.SetType.Both;
        private Func<dynamic, Dictionary<string, object>, dynamic> _SetFormatter;
        private dynamic _SetValue = null;
        private List<Func<dynamic, Dictionary<string, object>, ValidationHost, string>> _Validators =
            new List<Func<dynamic, Dictionary<string, object>, ValidationHost, string>>();
        private Func<List<Dictionary<string, object>>> _optsFn;
        private string _optsTable;
        private string _optsValue;
        private string _optsLabel;


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Public methods
         */

        /// <summary>
        /// Set the DB field name.
        /// </summary>
        /// <returns>Database field name</returns>
        public string DbField()
        {
            return _DbField;
        }

        /// <summary>
        /// Set the DB field name.
        /// 
        /// Note that when used as a setter, an alias can be given for the field
        /// using the SQL `as` keyword - for example: `firstName as name`. In this
        /// situation the dbField is set to the field name before the `as`, and the
        /// field's name (`name()`) is set to the name after the ` as `.
        ///
        /// As a result of this, the following constructs have identical
        /// functionality:
        ///
        /// * `.field.DbField( 'firstName as name' );`
        /// * `.field.DbField( 'firstName', 'name' );`
        /// </summary>
        /// <param name="field">Database field name</param>
        /// <returns>Self for chaining</returns>
        public Field DbField(string field)
        {
            if (field.IndexOf(" as ", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                string[] a = Regex.Split(field, " as ", RegexOptions.IgnoreCase);
                _DbField = a[0].Trim();
                _Name = a[1].Trim();
            }
            else
            {
                _DbField = field;
            }

            return this;
        }

        /// <summary>
        /// Get the 'Get' flag for the field.
        /// </summary>
        /// <returns>Get flag</returns>
        public bool Get()
        {
            return _Get;
        }

        /// <summary>
        /// Set the 'Get' flag of the field.
        ///
        /// A field can be marked as write only by setting the Get property to false
        /// </summary>
        /// <param name="get">Flag value to set</param>
        /// <returns>Self for chaining</returns>
        public Field Get(bool get)
        {
            _Get = get;
            return this;
        }

        /// <summary>
        /// Get formatter for the field's data.
        ///
        /// When the data has been retrieved from the server, it can be passed through
        /// a formatter here, which will manipulate (format) the data as required. This
        /// can be useful when, for example, working with dates and a particular format
        /// is required on the client-side.
        ///
        /// Editor has a number of formatters available with the <code>Format</code> class
        /// which can be used directly with this method.
        /// </summary>
        /// <param name="fn">Get formatter that will transform the db value into the http value</param>
        /// <returns>Self for chaining</returns>
        public Field GetFormatter(Func<dynamic, Dictionary<string, object>, dynamic> fn)
        {
            _GetFormatter = fn;

            return this;
        }

        /// <summary>
        /// Get the 'Get' value for the field
        /// </summary>
        /// <returns>Get value</returns>
        public dynamic GetValue()
        {
            return _GetValue;
        }

        /// <summary>
        /// Set a "Get" value. When set this value is used to send to the
        /// client-side, regardless of what value is held by the database.
        /// </summary>
        /// <param name="val">Value to set for "Get"</param>
        /// <returns>Self for chaining</returns>
        public Field GetValue(dynamic val)
        {
            _GetValue = val;
            return this;
        }

        /// <summary>
        /// Get the HTTP / JSON name for the field.
        ///  
        /// The name is typically the same as the `DbField` name, since it makes things
        /// less confusing(!), but it is possible to set a different name for the data
        /// which is used in the JSON returned to DataTables in a 'get' operation and
        /// the field name used in a 'set' operation.
        /// </summary>
        /// <returns>Field HTTP name</returns>
        public string Name()
        {
            return _Name;
        }

        /// <summary>
        /// Set the HTTP / JSON name for the field.
        /// 
        /// The name is typically the same as the `DbField` name, since it makes things
        /// less confusing(!), but it is possible to set a different name for the data
        /// which is used in the JSON returned to DataTables in a 'get' operation and
        /// the field name used in a 'set' operation.
        /// </summary>
        /// <param name="name">Name to set</param>
        /// <returns>Self for chaining</returns>
        public Field Name(string name)
        {
            _Name = name;

            return this;
        }

        /// <summary>
        /// Set a function that will retrieve a list of values that can be used
        /// for the options list in radio, select and checkbox inputs from the
        /// database for this field.
        /// </summary>
        /// <param name="fn">Delegate that will return a list of options</param>
        /// <returns>Self for chaining</returns>
        public Field Options(Func<List<Dictionary<string, object>>> fn)
        {
            _optsFn = fn;
            return this;
        }

        /// <summary>
        /// Provide database information for where to get a list of values that
        /// can be used for the options list in radio, select and checkbox
        /// inputs from the database for this field.
        ///
        /// Note that this is for simple cases only. For more complex operations
        /// use the delegate overload.
        /// </summary>
        /// <param name="table">Table name to read the options from</param>
        /// <param name="value">Column name to read the option values from</param>
        /// <param name="label">Column name to read the label values from</param>
        /// <returns>Self for chaining</returns>
        public Field Options(string table, string value, string label)
        {
            _optsFn = null;
            _optsTable = table;
            _optsValue = value;
            _optsLabel = label;
            return this;
        }

        /// <summary>
        /// Get the "Set" flag for this field
        /// </summary>
        /// <returns>Set flag</returns>
        public SetType Set()
        {
            return _Set;
        }

        /// <summary>
        /// Set the "Set" flag for this field.
        ///
        /// A field can be marked as read only using this option, to be set only
        /// during an create or edit action or to be set during both actions. This
        /// provides the ability to have fields that are only set when a new row is
        /// created (for example a "created" time stamp).
        ///
        /// For more control, use the `SetType` overload.
        /// </summary>
        /// <param name="set">Set flag</param>
        /// <returns>Self for chaining</returns>
        public Field Set(bool set)
        {
            Set(set ? Field.SetType.Both : Field.SetType.None);
            return this;
        }

        /// <summary>
        /// Set the "Set" flag for this field.
        ///
        /// A field can be marked to be set on create, edit, both or none using
        /// this method, providing the ability, for example, to write to
        /// `created` and `updated` datetime columns as appropriate.
        /// </summary>
        /// <param name="set">Set flag</param>
        /// <returns>Self for chaining</returns>
        public Field Set(SetType set)
        {
            _Set = set;
            return this;
        }

        /// <summary>
        /// Set formatter for the field's data.
        ///
        /// When the data has been retrieved from the server, it can be passed through
        /// a formatter here, which will manipulate (format) the data as required. This
        /// can be useful when, for example, working with dates and a particular format
        /// is required on the client-side.
        ///
        /// Editor has a number of formatters available with the <code>Format</code> class
        /// which can be used directly with this method.
        /// </summary>
        /// <param name="fn">Get formatter delegate</param>
        /// <returns>Self for chaining</returns>
        public Field SetFormatter(Func<dynamic, Dictionary<string, object>, dynamic> fn)
        {
            _SetFormatter = fn;

            return this;
        }

        /// <summary>
        /// Retrieve the "Set" value for the field
        /// </summary>
        /// <returns>"Set" value</returns>
        public dynamic SetValue()
        {
            return _SetValue;
        }

        /// <summary>
        /// Set a "Set" value. When set this value is used to write to the
        /// database regardless of what data is sent from the client-side.
        /// </summary>
        /// <param name="val">Value to set for "Set"</param>
        /// <returns>Self for chaining</returns>
        public Field SetValue(dynamic val)
        {
            _SetValue = val;
            return this;
        }

        /// <summary>
        /// Get the field type
        /// </summary>
        /// <returns>Type</returns>
        public Type Type()
        {
            return _Type;
        }

        /// <summary>
        /// Set the data type for the field's values
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>Self for chaining</returns>
        public Field Type(Type t)
        {
            _Type = t;
            return this;
        }

        /// <summary>
        /// Get the type error message
        /// </summary>
        /// <returns>Type error message</returns>
        public string TypeError()
        {
            return _TypeError;
        }

        /// <summary>
        /// If the value retrieved from the database can't be cast to the type
        /// given, this is the error message that will be given.
        /// </summary>
        /// <param name="err">Error message</param>
        /// <returns>Self for chaining</returns>
        public Field TypeError(string err)
        {
            _TypeError = err;
            return this;
        }

        /// <summary>
        /// Set the 'validator' of the field.
        ///
        /// The validator can be used to check if any abstract piece of data is valid
        /// or not according to the given rules of the validation function used.
        ///
        /// Multiple validation options can be applied to a field instance by calling
        /// this method multiple times. For example, it would be possible to have a
        /// 'Required' validation and a 'MaxLength' validation with multiple calls.
        /// 
        /// Editor has a number of validation available with the <code>Validation</code> class
        /// which can be used directly with this method.
        /// </summary>
        /// <param name="fn">Validation method</param>
        /// <returns>Self for chaining</returns>
        public Field Validator(Func<dynamic, Dictionary<string, object>, ValidationHost, string> fn)
        {
            _Validators.Add(fn);

            return this;
        }



        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Internal methods
         */

        /// <summary>
        /// Check to see if a field should be used for a particular action (get or set).
        ///
        /// Called by the Editor / Join class instances - not expected for general
        /// consumption - internal.
        /// </summary>
        /// <param name="action">Direction that the data is travelling  - 'get' is reading DB data, `create` and `edit` for writing to the DB</param>
        /// <param name="data">Data submitted from the client-side when setting.</param>
        /// <returns>true if the field should be used in the get / set.</returns>
        internal bool Apply(string action, Dictionary<string, object> data = null)
        {
            if (action == "get")
            {
                // Get action - can we get this field
                return _Get;
            }
            else
            {
                // Note that validation must be done on input data before we get here.

                // Create or edit action, are we configured to use this field
                if (action == "create" && (_Set == Field.SetType.None || _Set == Field.SetType.Edit))
                {
                    return false;
                }
                else if (action == "edit" && (_Set == Field.SetType.None || _Set == Field.SetType.Create))
                {
                    return false;
                }

                // Check it was in the submitted data. If not, then not required
                // (validation would have failed if it was) and therefore we don't
                // Set it. Check for as Set formatter and value as well, as it can
                // format data from some other source
                if (_SetValue == null && _SetFormatter == null && !_InData(Name(), data))
                {
                    return false;
                }

                // In the data set, so use it
                return true;
            }
        }

        /// <summary>
        /// Execute the ipOpts to get the list of options to return to the client-
        /// side
        /// </summary>
        /// <param name="db">Database instance</param>
        /// <returns>List of options</returns>
        internal List<Dictionary<string, object>> OptionsExec(Database db)
        {
            if (_optsFn != null)
            {
                return _optsFn();
            }
            else if (_optsTable != null)
            {
                return db.SelectDistinct(
                    _optsTable,
                    new string[] { _optsValue + " as value, " + _optsLabel + " as label" },
                    null,
                    new string[] { "label asc" }
                ).FetchAll();
            }

            return null;
        }

        /// <summary>
        /// Get the value of the field, taking into account if it is coming from the
        /// DB or from a POST. If formatting has been specified for this field, it
        /// will be applied here.
        /// </summary>
        /// <param name="action">Direction that the data is travelling  - 'get' is reading DB data, `create` and `edit` for writing to the DB</param>
        /// <param name="data">Data submitted from the client-side when setting.</param>
        /// <returns>Value for the field</returns>
        internal dynamic Val(string direction, Dictionary<string, Object> data)
        {
            dynamic val;

            if (direction == "get")
            {
                // Use data from the database, so the db name
                if (_GetValue != null)
                {
                    val = _GetAssignedValue(_GetValue);
                }
                else
                {
                    val = data.ContainsKey(_DbField) ?
                        data[_DbField] :
                        null;
                }

                return _Format(val, data, _GetFormatter);
            }
            else
            {
                // Use data from setting from the POST / GET data, so use the name
                val = _SetValue != null ?
                    _GetAssignedValue(_SetValue) :
                    _ReadProp(Name(), data);

                return _Format(val, data, _SetFormatter);
            }
        }

        /// <summary>
        /// Check the validity of the field based on the data submitted. Note that
        /// this validation is performed on the wire data - i.e. that which is
        /// submitted, before any setFormatter is run
        /// </summary>
        /// <param name="data">Data from HTTP to check</param>
        /// <param name="editor">Editor instance</param>
        /// <returns>`null` if valid, or error message string if not valid</returns>
        internal string Validate(Dictionary<string, object> data, Editor editor)
        {
            if (_Validators == null)
            {
                return null;
            }

            dynamic val = _ReadProp(Name(), data);
            DtRequest processData = editor.InData();
            ValidationHost host = new ValidationHost()
            {
                Action = processData.Action,
                Id = processData.Ids.Count() > 0 ?
                    processData.Ids[0].Replace(editor.IdPrefix(), "") :
                    null,
                Field = this,
                Editor = editor
            };

            foreach (var validator in _Validators)
            {
                string res = validator(val, data, host);

                if (res != null)
                {
                    return res;
                }
            }

            return null;
        }

        /// <summary>
        /// Write the value for this field to the output array for a read operation
        /// </summary>
        /// <param name="outData">Row output data (to the JSON)</param>
        /// <param name="srcData">Row input data (raw, from the database)</param>
        internal void Write(Dictionary<string, object> outData, Dictionary<string, object> srcData)
        {
            _WriteProp(outData, Name(), Val("get", srcData));
        }


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Private methods
         */

        /// <summary>
        /// Apply a formatter to data. The caller will decide what formatter to apply
        /// (get or set)
        /// </summary>
        /// <param name="val">Value to be formatted</param>
        /// <param name="data">Full row data</param>
        /// <param name="formatter">Formatting function to be called</param>
        /// <returns>Formatted value</returns>
        private dynamic _Format(dynamic val, Dictionary<string, object> data, Func<dynamic, Dictionary<string, object>, dynamic> formatter)
        {
            return formatter != null ?
                formatter(val, data) :
                val;
        }

        /// <summary>
        /// Get the value from `_[gs]etValue` - taking into account if it is callable
        /// function or not
        /// </summary>
        /// <param name="val">Value to be evaluated</param>
        /// <returns>Value assigned, or returned from the function</returns>
        private dynamic _GetAssignedValue(dynamic val)
        {
            return val is Delegate ?
                val() :
                val;
        }

        /// <summary>
        /// Check is a parameter is in the submitted data set. This is functionally
        /// the same as the `_readProp()` method, but in this case a binary value
        /// is required to indicate if the value is present or not.
        /// </summary>
        /// <param name="name">Javascript dotted object name to write to</param>
        /// <param name="data">Data source array to read from</param>
        /// <returns>`true` if present, `false` otherwise</returns>
        private bool _InData(string name, Dictionary<string, dynamic> data)
        {
            if (!name.Contains('.'))
            {
                return data.ContainsKey(name) ?
                    true :
                    false;
            }

            string[] names = name.Split('.');
            Dictionary<string, dynamic> inner = data;

            for (int i = 0, ien = names.Count() - 1; i < ien; i++)
            {
                if (!inner.ContainsKey(names[i]))
                {
                    return false;
                }

                inner = inner[names[i]] as Dictionary<string, dynamic>;
            }

            return inner.ContainsKey(names.Last()) ?
                true :
                false;
        }

        /// <summary>
        /// Read a value from a data structure, using Javascript dotted object
        /// notation. This is the inverse of the `_writeProp` method and provides
        /// the same support, matching DataTables' ability to read nested JSON
        /// data objects.
        /// </summary>
        /// <param name="name">Javascript dotted object name to write to</param>
        /// <param name="inData">Data source array to read from</param>
        /// <returns>The read value, or null if no value found.</returns>
        private dynamic _ReadProp(string name, IDictionary<string, Object> inData)
        {
            if (!name.Contains("."))
            {
                return inData.ContainsKey(name) ?
                    inData[name] :
                    null;
            }

            string[] names = name.Split('.');
            IDictionary<string, Object> inner = inData;

            for (int i = 0; i < names.Count() - 1; i++)
            {
                if (!inner.ContainsKey(names[i]))
                {
                    return null;
                }

                inner = inner[names[i]] as IDictionary<string, Object>;
            }

            return inner.ContainsKey(names.Last()) ?
                inner[names.Last()] :
                null;
        }

        /// <summary>
        /// Write the field's value to an array structure, using Javascript dotted
        /// object notation to indicate JSON data structure. For example `name.first`
        /// gives the data structure: `name: { first: ... }`. This matches DataTables
        /// own ability to do this on the client-side, although this doesn't
        /// implement implement quite such a complex structure (no array / function
        /// support).
        /// </summary>
        /// <param name="outData">Dic to write the data to</param>
        /// <param name="name">Javascript dotted object name to write to</param>
        /// <param name="value">Value to write</param>
        private void _WriteProp(Dictionary<string, object> outData, string name, dynamic value)
        {
            if (!name.Contains("."))
            {
                _WriteCast(outData, name, value);
                return;
            }

            string[] names = name.Split('.');
            Dictionary<string, object> inner = outData;

            for (int i = 0; i < names.Count() - 1; i++)
            {
                string loopName = names[i];

                if (!inner.ContainsKey(loopName))
                {
                    inner.Add(loopName, new Dictionary<string, object>());
                }
                else if (!(inner[loopName] is Dictionary<string, object>))
                {
                    throw new Exception(
                        "A property with the name `" + name + "` already exists. This " +
                        "can occur if you have properties which share a prefix - " +
                        "for example `name` and `name.first`"
                    );
                }

                inner = inner[loopName] as Dictionary<string, object>;
            }

            if (inner.ContainsKey(names.Last()))
            {
                throw new Exception(
                    "Duplicate field detected - a field with the name `" + name + "` " +
                    "already exists."
                );
            }

            // Attempt to cast to the type given
            _WriteCast(inner, names.Last(), value);
        }

        /// <summary>
        /// Convert the value to the field's type, with error handling
        /// </summary>
        /// <param name="outData">Dic to write the data to</param>
        /// <param name="name">Javascript dotted object name to write to</param>
        /// <param name="value">Value to write</param>
        private void _WriteCast(Dictionary<string, object> outData, string name, dynamic value)
        {
            try
            {
                if (value == null)
                {
                    outData.Add(name, null);
                }
                else
                {
                    outData.Add(name, Convert.ChangeType(value, _Type));
                }
            }
            catch (Exception e)
            {
                throw new Exception(
                    TypeError()
                        .Replace("{fieldName}", Name())
                        .Replace("{typeName}", _Type.Name)
                );
            }
        }
    }
}
