// <copyright>Copyright (c) 2014 SpryMedia Ltd - All Rights Reserved</copyright>
//
// <summary>
// Editor class for reading tables as well as creating, editing and deleting rows
// </summary>
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using CAF.Mvc.JQuery.Datatables.Core.Editors.EditorUtil;

namespace CAF.Mvc.JQuery.Datatables.Core.Editors
{
    /// <summary>
    /// DataTables Editor base class for creating editable tables.
    ///
    /// Editor class instances are capable of servicing all of the requests that
    /// DataTables and Editor will make from the client-side - specifically:
    ///
    /// * Get data
    /// * Create new record
    /// * Edit existing record
    /// * Delete existing records
    ///
    /// The Editor instance is configured with information regarding the
    /// database table fields that you which to make editable, and other information
    /// needed to read and write to the database (table name for example!).
    ///
    /// This documentation is very much focused on describing the API presented
    /// by these DataTables Editor classes. For a more general overview of how
    /// the Editor class is used, and how to install Editor on your server, please
    /// refer to the Editor manual ( https://editor.datatables.net/manual ).
    /// </summary>
    public class Editor
    {
        /// <summary>
        /// Version string
        /// </summary>
        public const string Version = "1.4.0-beta.1";

        /// <summary>
        /// Create a new Editor instance
        /// </summary>
        /// <param name="db">An instance of the DataTables Database class that we can use for the DB connection. Can also be set with the <code>Db()</code> method.</param>
        /// <param name="table">The table name in the database to read and write information from and to. Can also be set with the <code>Table()</code> method.</param>
        /// <param name="pkey">Primary key column name in the table given. Can also be set with the <code>PKey()</code> method.</param>
        public Editor( string table = null, string pkey = null)
        {

            if (table != null)
            {
                Table(table);
            }

            if (pkey != null)
            {
                Pkey(pkey);
            }
        }


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Private parameters
         */

        private List<Field> _Field = new List<Field>();
        private string _IdPrefix = "row_";
        private DataTablesEditorRequest _ProcessData;
        private Dictionary<string, object> _FormData;
        private Type _UserModelT;
        private string _Pkey = "id";
        private List<string> _Table = new List<string>();
        private bool _Transaction = true;
        private List<WhereCondition> _Where = new List<WhereCondition>();
        private List<LeftJoin> _LeftJoin = new List<LeftJoin>();
        private DataTablesEditorResponse _Out = new DataTablesEditorResponse();


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Public methods
         */

        /// <summary>
        /// Get the response object that has been created by this instance. This
        /// is only useful after <code>process()</code> has been called.
        /// </summary>
        /// <returns>The response object as populated by this instance</returns>
        public DataTablesEditorResponse Data()
        {
            return _Out;
        }


     

        /// <summary>
        /// Get the fields that have been configured for this instance
        /// </summary>
        /// <returns>List of fields</returns>
        public List<Field> Field()
        {
            return _Field;
        }

        /// <summary>
        /// Add a new field to this instance
        /// </summary>
        /// <param name="f">New field to add</param>
        /// <returns>Self for chaining</returns>
        public Editor Field(Field f)
        {
            _Field.Add(f);
            return this;
        }

        /// <summary>
        /// Add multiple fields too this instance
        /// </summary>
        /// <param name="fields">Collection of fields to add</param>
        /// <returns>Self for chaining</returns>
        public Editor Field(IEnumerable<Field> fields)
        {
            foreach (Field f in fields)
            {
                _Field.Add(f);
            }
            return this;
        }

        /// <summary>
        /// Get the DOM prefix.
        /// 
        /// Typically primary keys are numeric and this is not a valid ID value in an
        /// HTML document - is also increases the likelihood of an ID clash if multiple
        /// tables are used on a single page. As such, a prefix is assigned to the 
        /// primary key value for each row, and this is used as the DOM ID, so Editor
        /// can track individual rows.
        /// </summary>
        /// <returns>DOM prefix</returns>
        public string IdPrefix()
        {
            return _IdPrefix;
        }

        /// <summary>
        /// Set the DOM prefix.
        /// 
        /// Typically primary keys are numeric and this is not a valid ID value in an
        /// HTML document - is also increases the likelihood of an ID clash if multiple
        /// tables are used on a single page. As such, a prefix is assigned to the 
        /// primary key value for each row, and this is used as the DOM ID, so Editor
        /// can track individual rows.
        /// </summary>
        /// <param name="prefix">Prefix to set</param>
        /// <returns>Self for chaining</returns>
        public Editor IdPrefix(string prefix)
        {
            _IdPrefix = prefix;
            return this;
        }

        /// <summary>
        /// Get the data that is being processed by the Editor instance. This is only
        /// useful once the <code>Process()</code> method has been called, and
        /// is available for use in validation and formatter methods.
        /// </summary>
        /// <returns>Data given to <code>Process()</code></returns>
        public DataTablesEditorRequest InData()
        {
            return _ProcessData;
        }

        /// <summary>
        /// Add a left join condition to the Editor instance, allowing it to operate
        /// over multiple tables. Multiple <code>leftJoin()</code> calls can be made for a
        /// single Editor instance to join multiple tables.
        ///
        /// A left join is the most common type of join that is used with Editor
        /// so this method is provided to make its use very easy to configure. Its
        /// parameters are basically the same as writing an SQL left join statement,
        /// but in this case Editor will handle the create, update and remove
        /// requirements of the join for you:
        ///
        /// * Create - On create Editor will insert the data into the primary table
        ///   and then into the joined tables - selecting the required data for each
        ///   table.
        /// * Edit - On edit Editor will update the main table, and then either
        ///   update the existing rows in the joined table that match the join and
        ///   edit conditions, or insert a new row into the joined table if required.
        /// * Remove - On delete Editor will remove the main row and then loop over
        ///   each of the joined tables and remove the joined data matching the join
        ///   link from the main table.
        ///
        /// Please note that when using join tables, Editor requires that you fully
        /// qualify each field with the field's table name. SQL can result table
        /// names for ambiguous field names, but for Editor to provide its full CRUD
        /// options, the table name must also be given. For example the field
        /// <code>first_name</code> in the table <code>users</code> would be given
        /// as <code>users.first_name</code>.
        /// </summary>
        /// <param name="table">Table name to do a join onto</param>
        /// <param name="field1">Field from the parent table to use as the join link</param>
        /// <param name="op">Join condition (`=`, '<`, etc)</param>
        /// <param name="field2">Field from the child table to use as the join link</param>
        /// <returns>Self for chaining</returns>
        public Editor LeftJoin(string table, string field1, string op, string field2)
        {
            _LeftJoin.Add(new LeftJoin(table, field1, op, field2));

            return this;
        }

        /// <summary>
        /// Set a model to use.
        ///
        /// In keeping with the MVC style of coding, you can define the fields
        /// and their types that you wish to get from the database in a simple
        /// class. Editor will automatically add fields from the model.
        ///
        /// Note that fields that are defined in the model can also be defined
        /// as <code>Field</code> instances should you wish to add additional
        /// options to a specific field such as formatters or validation.
        /// </summary>
        /// <typeparam name="T">Model to use</typeparam>
        /// <returns>Self for chaining</returns>
        public Editor Model<T>()
        {
            _UserModelT = typeof(T);

            return this;
        }


        /// <summary>
        /// Get the primary key field that has been configured.
        /// 
        /// The primary key must be known to Editor so it will know which rows are being
        /// edited / deleted upon those actions. The default value is 'id'.
        /// </summary>
        /// <returns>Primary key</returns>
        public string Pkey()
        {
            return _Pkey;
        }

        /// <summary>
        /// Set the primary key field to use. Please note that at this time
        /// Editor does not support composite primary keys in a table, only a
        /// single field primary key is supported.
        /// 
        /// The primary key must be known to Editor so it will know which rows are being
        /// edited / deleted upon those actions. The default value is 'id'.
        /// </summary>
        /// <param name="id">Primary key column name</param>
        /// <returns>Self for chaining</returns>
        public Editor Pkey(string id)
        {
            _Pkey = id;
            return this;
        }

        /// <summary>
        /// Process a request from the Editor client-side to get / set data.
        /// </summary>
        /// <param name="data">Data sent from the client-side</param>
        /// <returns>Self for chaining</returns>
        public Editor Process(DataTablesEditorRequest data)
        {
            try
            {
                _ProcessData = data;
                _FormData = data.Data ?? null;

                _PrepModel();

                if (data.RequestType == DataTablesEditorRequest.RequestTypes.DataTablesGet ||
                    data.RequestType == DataTablesEditorRequest.RequestTypes.DataTablesSsp)
                {
                    // DataTable get request
                    //_Out.Merge(_Get(null, data));
                }
                else if (data.RequestType == DataTablesEditorRequest.RequestTypes.EditorRemove)
                {
                    // Remove rows
                   // _Remove(data);
                }
                else
                {
                        _Out.row = data.RequestType == DataTablesEditorRequest.RequestTypes.EditorCreate ?
                            _Insert() :
                            _Update(data.Ids);
                    
                }

            }
            catch (Exception e)
            {
                _Out.error = e.Message;

            }

            return this;
        }

        /// <summary>
        /// Process a request from the Editor client-side to get / set data.
        /// For use with WebAPI's 'FormDataCollection' collection
        /// </summary>
        /// <param name="data">Data sent from the client-side</param>
        /// <returns>Self for chaining</returns>
        public Editor Process(IEnumerable<KeyValuePair<string, string>> data = null)
        {
            return Process(new DataTablesEditorRequest(data));
        }

        /// <summary>
        /// Process a request from the Editor client-side to get / set data.
        /// For use with MVC's 'Request.Form' collection
        /// </summary>
        /// <param name="data">Data sent from the client-side</param>
        /// <returns>Self for chaining</returns>
        public Editor Process(NameValueCollection data = null)
        {
            var list = new List<KeyValuePair<string, string>>();

            if (data != null)
            {
                foreach (var key in data.AllKeys)
                {
                    list.Add(new KeyValuePair<string, string>(key, data[key]));
                }
            }

            return Process(new DataTablesEditorRequest(list));
        }

        /// <summary>
        /// Where condition to add to the query used to get data from the database.
        /// Multiple conditions can be added if required.
        /// 
        /// Can be used in two different ways:
        /// 
        /// * Simple case: `where( field, value, operator )`
        /// * Complex: `where( fn )`
        ///
        /// The simple case is fairly self explanatory, a condition is applied to the
        /// data that looks like `field operator value` (e.g. `name = 'Allan'`). The
        /// complex case allows full control over the query conditions by providing a
        /// closure function that has access to the database Query that Editor is
        /// using, so you can use the `where()`, `or_where()`, `and_where()` and
        /// `where_group()` methods as you require.
        ///
        /// Please be very careful when using this method! If an edit made by a user
        /// using Editor removes the row from the where condition, the result is
        /// undefined (since Editor expects the row to still be available, but the
        /// condition removes it from the result set).
        /// </summary>
        /// <param name="key">Database column name to perform the condition on</param>
        /// <param name="value">Value to use for the condition</param>
        /// <param name="op">Conditional operator</param>
        /// <returns>Self for chaining</returns>
        public Editor Where(string key, dynamic value, string op = "=")
        {
            _Where.Add(new WhereCondition
            {
                Key = key,
                Value = value,
                Operator = op
            });

            return this;
        }


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Private methods
         */
        private void _PrepModel()
        {
            // Add fields which are defined in the model, but not in the _Field list
            if (_UserModelT != null)
            {
                _FieldFromModel(_UserModelT);
            }
        }

        private void _FieldFromModel(Type model, string parent = "")
        {
            // Add the properties
            foreach (var pi in model.GetProperties())
            {
                var field = _FindField(parent + pi.Name, "name");

                // If the field doesn't exist yet, create it
                if (field == null)
                {
                    field = new Field(parent + pi.Name);
                    Field(field);
                }

                // Then assign the information from the model
                field.Type(pi.PropertyType);

                if (pi.IsDefined(typeof(EditorTypeErrorAttribute)) == true)
                {
                    var err = pi.GetCustomAttribute<EditorTypeErrorAttribute>();
                    field.TypeError(err.Msg);
                }

                if (pi.IsDefined(typeof(EditorHttpNameAttribute)) == true)
                {
                    var name = pi.GetCustomAttribute<EditorHttpNameAttribute>();
                    field.Name(name.Name);
                }
            }

            // Add any nested classes and their properties
            var nested = model.GetNestedTypes(BindingFlags.Public | BindingFlags.Instance);

            foreach (var t in nested)
            {
                _FieldFromModel(t, parent + t.Name + ".");
            }
        }
        private Field _FindField(string name, string type)
        {
            for (int i = 0, ien = _Field.Count(); i < ien; i++)
            {
                Field field = _Field[i];

                if (type == "name" && field.Name() == name)
                {
                    return field;
                }
                else if (type == "db" && field.DbField() == name)
                {
                    return field;
                }
            }

            return null;
        }
        
    }
}
