using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BisAceDatabaseContext
{
    /// <summary>
    /// Abstract class generalizing running database queries
    /// </summary>
    /// <typeparam name="TConn">Type for connecting to the database</typeparam>
    /// <typeparam name="TComm">Type for running commands on the database</typeparam>
    /// <typeparam name="TParam">Type for parameters to the commands</typeparam>
    public abstract class ADatabase<TConn, TComm, TParam> : IDatabaseContext
        where TConn : System.Data.Common.DbConnection, new()
        where TComm : System.Data.Common.DbCommand, new()
        where TParam : System.Data.Common.DbParameter, new()
    {

        protected string m_connectionString;

        #region IDatabaseContext Members
        /// <summary>
        /// Executes a non-query SQL statement
        /// </summary>
        /// <param name="commandText">The command text to execute</param>
        /// <param name="parameters">Optional parameters to pass to the command</param>
        /// <param name="isStoredProcedure">Optional. If true, indicates that the command text is the name of the stored procedure to run rather than a normal query. Default is false.</param>
        /// <returns>The number of records affected by the command</returns>
        /// <exception cref="ArgumentNullException">If the command text is null, empty, or whitespace.</exception>
        public int Execute(string commandText, Dictionary<string, object> parameters, bool isStoredProcedure = false)
        {
            int result = 0;  // Number of records affected by the command

            // Throw exception if the command text is null or blank
            if (String.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentNullException("commandText", "Command text cannot be null or empty.");
            }

            // Create the connection and command
            using (TConn connection = new TConn())
            using (TComm command = CreateCommand(connection, commandText, parameters, isStoredProcedure))
            {
                connection.ConnectionString = m_connectionString;

                // Open the connection then run the command
                connection.Open();

                result = command.ExecuteNonQuery();
            }

            return result;
        }

        /// <summary>
        /// Executes a non-query SQL statement asynchronously
        /// </summary>
        /// <param name="commandText">The command text to execute</param>
        /// <param name="parameters">Optional parameters to pass to the command</param>
        /// <param name="isStoredProcedure">Optional. If true, indicates that the command text is the name of the stored procedure to run rather than a normal query. Default is false.</param>
        /// <returns>The number of records affected by the command</returns>
        /// <exception cref="ArgumentNullException">If the command text is null, empty, or whitespace.</exception>
        public async Task<int> ExecuteAsync(string commandText, Dictionary<string, object> parameters, bool isStoredProcedure = false)
        {
            int result = 0;  // Number of records affected by the command

            // Throw exception if the command text is null or blank
            if (String.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentNullException("commandText", "Command text cannot be null or empty.");
            }

            // Create the connection and command
            using (TConn connection = new TConn())
            using (TComm command = CreateCommand(connection, commandText, parameters, isStoredProcedure))
            {
                connection.ConnectionString = m_connectionString;

                // Open the connection then run the command
                connection.Open();

                result = await command.ExecuteNonQueryAsync();
            }

            return result;
        }

        /// <summary>
        /// Executes a SQL query that returns a list of rows as the result.
        /// </summary>
        /// <param name="commandText">The MySQL query to execute</param>
        /// <param name="parameters">Parameters to pass to the query. If no parameters are needed, null can be passed in.</param>
        /// <param name="isStoredProcedure">Optional. If true, indicates that the command text is the name of the stored procedure to run rather than a normal query. Default is false.</param>
        /// <returns>A list of all rows returned by the query. Each row is represented by a dictionary representing the 
        /// column name and corresponding value for each column, both as strings. If the query was not run, null will be returned.</returns>
        /// <exception cref="ArgumentNullException">If the command text is null, empty, or whitespace.</exception>
        public List<Dictionary<string, string>> QueryStrings(string commandText, Dictionary<string, object> parameters, bool isStoredProcedure = false)
        {
            // Create the list to hold the rows. Leave as null incase the query doesn't run.
            List<Dictionary<string, string>> queryResults = null;

            // Throw exception if the command text is null or blank
            if (String.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException("Command text cannot be null, empty, or whitespace.");
            }

            // Create the connection and command
            using (TConn connection = new TConn())
            using (TComm command = CreateCommand(connection, commandText, parameters, isStoredProcedure))
            {
                connection.ConnectionString = m_connectionString;

                // Open the connection
                connection.Open();

                // Execute the query
                using (var reader = command.ExecuteReader())
                {
                    // Create a new list to hold the returned rows
                    queryResults = new List<Dictionary<string, string>>();

                    // While we have data to read
                    while (reader.Read())
                    {
                        // Create a new dictionary to hold the row data
                        Dictionary<string, string> row = new Dictionary<string, string>();

                        // for each field we queried for, get it and save it off to the dictionary
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            // Get the column name and the value
                            string columnName = reader.GetName(i);
                            string columnValue = ReadValueAsString(reader, i);

                            // Add the column data to the row data
                            row.Add(columnName, columnValue);
                        }

                        // Add all of the queried fields to the results as a row
                        queryResults.Add(row);
                    }
                }
            }

            return queryResults;
        }

        /// <summary>
        /// Executes a SQL query that returns a list of rows as the result.
        /// </summary>
        /// <param name="commandText">The MySQL query to execute</param>
        /// <param name="parameters">Parameters to pass to the query. If no parameters are needed, null can be passed in.</param>
        /// <param name="isStoredProcedure">Optional. If true, indicates that the command text is the name of the stored procedure to run rather than a normal query. Default is false.</param>
        /// <returns>A list of all rows returned by the query. Each row is represented by a dictionary representing the 
        /// column name and corresponding value for each column, both as strings. If the query was not run, null will be returned.</returns>
        /// <exception cref="ArgumentNullException">If the command text is null, empty, or whitespace.</exception>
        public async Task<List<Dictionary<string, string>>> QueryStringsAsync(string commandText, Dictionary<string, object> parameters, bool isStoredProcedure = false)
        {
            // Create the list to hold the rows. Leave as null incase the query doesn't run.
            List<Dictionary<string, string>> queryResults = null;

            // Throw exception if the command text is null or blank
            if (String.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException("Command text cannot be null, empty, or whitespace.");
            }

            // Create the connection and command
            using (TConn connection = new TConn())
            using (TComm command = CreateCommand(connection, commandText, parameters, isStoredProcedure))
            {
                connection.ConnectionString = m_connectionString;

                // Open the connection
                connection.Open();

                // Execute the query
                using (var reader = await command.ExecuteReaderAsync())
                {
                    // Create a new list to hold the returned rows
                    queryResults = new List<Dictionary<string, string>>();

                    // While we have data to read
                    while (await reader.ReadAsync())
                    {
                        // Create a new dictionary to hold the row data
                        Dictionary<string, string> row = new Dictionary<string, string>();

                        // for each field we queried for, get it and save it off to the dictionary
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            // Get the column name and the value
                            string columnName = reader.GetName(i);
                            string columnValue = ReadValueAsString(reader, i);

                            // Add the column data to the row data
                            row.Add(columnName, columnValue);
                        }

                        // Add all of the queried fields to the results as a row
                        queryResults.Add(row);
                    }
                }
            }

            return queryResults;
        }

        /// <summary>
        /// Executes a SQL query that returns a list of rows as the result.
        /// </summary>
        /// <param name="commandText">The MySQL query to execute</param>
        /// <param name="parameters">Parameters to pass to the query. If no parameters are needed, null can be passed in.</param>
        /// <param name="isStoredProcedure">Optional. If true, indicates that the command text is the name of the stored procedure to run rather than a normal query. Default is false.</param>
        /// <returns>A list of all rows returned by the query. Each row is represented by a dictionary representing the 
        /// column name and corresponding value for each column. If the query was not run, null will be returned.</returns>
        /// <exception cref="ArgumentNullException">If the command text is null, empty, or whitespace.</exception>
        public List<Dictionary<string, object>> QueryValues(string commandText, Dictionary<string, object> parameters, bool isStoredProcedure = false)
        {
            // Create the list to hold the rows. Leave as null incase the query doesn't run.
            List<Dictionary<string, object>> queryResults = null;

            // Throw exception if the command text is null or blank
            if (String.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException("Command text cannot be null, empty, or whitespace.");
            }

            // Create the connection and command
            using (TConn connection = new TConn())
            using (TComm command = CreateCommand(connection, commandText, parameters, isStoredProcedure))
            {
                connection.ConnectionString = m_connectionString;

                // Open the connection
                connection.Open();

                // Execute the query
                using (var reader = command.ExecuteReader())
                {
                    // Create a new list to hold the returned rows
                    queryResults = new List<Dictionary<string, object>>();

                    // While we have data to read
                    while (reader.Read())
                    {
                        // Create a new dictionary to hold the row data
                        Dictionary<string, object> row = new Dictionary<string, object>();

                        // for each field we queried for, get it and save it off to the dictionary
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            // Get the column name and the value
                            string columnName = reader.GetName(i);
                            object columnValue = reader.IsDBNull(i) ? null : reader.GetValue(i);

                            // Add the column data to the row data
                            row.Add(columnName, columnValue);
                        }

                        // Add all of the queried fields to the results as a row
                        queryResults.Add(row);
                    }
                }
            }

            return queryResults;
        }

        /// <summary>
        /// Executes a SQL query that returns a list of rows as the result.
        /// </summary>
        /// <param name="commandText">The MySQL query to execute</param>
        /// <param name="parameters">Parameters to pass to the query. If no parameters are needed, null can be passed in.</param>
        /// <param name="isStoredProcedure">Optional. If true, indicates that the command text is the name of the stored procedure to run rather than a normal query. Default is false.</param>
        /// <returns>A list of all rows returned by the query. Each row is represented by a dictionary representing the 
        /// column name and corresponding value for each column. If the query was not run, null will be returned.</returns>
        /// <exception cref="ArgumentNullException">If the command text is null, empty, or whitespace.</exception>
        public async Task<List<Dictionary<string, object>>> QueryValuesAsync(string commandText, Dictionary<string, object> parameters, bool isStoredProcedure = false)
        {
            // Create the list to hold the rows. Leave as null incase the query doesn't run.
            List<Dictionary<string, object>> queryResults = null;

            // Throw exception if the command text is null or blank
            if (String.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException("Command text cannot be null, empty, or whitespace.");
            }

            // Create the connection and command
            using (TConn connection = new TConn())
            using (TComm command = CreateCommand(connection, commandText, parameters, isStoredProcedure))
            {
                connection.ConnectionString = m_connectionString;

                // Open the connection
                connection.Open();

                // Execute the query
                using (var reader = await command.ExecuteReaderAsync())
                {
                    // Create a new list to hold the returned rows
                    queryResults = new List<Dictionary<string, object>>();

                    // While we have data to read
                    while (await reader.ReadAsync())
                    {
                        // Create a new dictionary to hold the row data
                        Dictionary<string, object> row = new Dictionary<string, object>();

                        // for each field we queried for, get it and save it off to the dictionary
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            // Get the column name and the value
                            string columnName = reader.GetName(i);
                            object columnValue = reader.IsDBNull(i) ? null : reader.GetValue(i);

                            // Add the column data to the row data
                            row.Add(columnName, columnValue);
                        }

                        // Add all of the queried fields to the results as a row
                        queryResults.Add(row);
                    }
                }
            }

            return queryResults;
        }

        /// <summary>
        /// Executes a SQL query that returns a single string value as the result.
        /// </summary>
        /// <param name="commandText">The query text to run.</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <param name="isStoredProcedure">Optional. If true, indicates that the command text is the name of the stored procedure to run rather than a normal query. Default is false.</param>
        /// <returns>String representing the value of the query.</returns>
        /// <exception cref="ArgumentNullException">If the command text is null, empty, or whitespace.</exception>
        public string QueryString(string commandText, Dictionary<string, object> parameters, bool isStoredProcedure = false)
        {
            return QueryValue(commandText, parameters, isStoredProcedure) as string;
        }


        /// <summary>
        /// Executes a SQL query that returns a single string value as the result.
        /// </summary>
        /// <param name="commandText">The query text to run.</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <param name="isStoredProcedure">Optional. If true, indicates that the command text is the name of the stored procedure to run rather than a normal query. Default is false.</param>
        /// <returns>String representing the value of the query.</returns>
        /// <exception cref="ArgumentNullException">If the command text is null, empty, or whitespace.</exception>
        public async Task<string> QueryStringAsync(string commandText, Dictionary<string, object> parameters, bool isStoredProcedure = false)
        {
            var val = await QueryValueAsync(commandText, parameters, isStoredProcedure) as string;
            return val;
        }

        /// <summary>
        /// Executes a SQL query that returns a single scalar value as the result.
        /// </summary>
        /// <param name="commandText">The query text to run.</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <param name="isStoredProcedure">Optional. If true, indicates that the command text is the name of the stored procedure to run rather than a normal query. Default is false.</param>
        /// <returns>Object representing the value of the query.</returns>
        /// <exception cref="ArgumentNullException">If the command text is null, empty, or whitespace.</exception>
        public object QueryValue(string commandText, Dictionary<string, object> parameters, bool isStoredProcedure = false)
        {
            object result = null;  // Return value of the scalar query

            // Throw exception if the command text is null or blank
            if (String.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            // Create the connection and command
            using (TConn connection = new TConn())
            using (TComm command = CreateCommand(connection, commandText, parameters, isStoredProcedure))
            {
                connection.ConnectionString = m_connectionString;

                // Open the connection then run the command
                connection.Open();

                var queryResult = command.ExecuteScalar();

                // Only set the result if it isn't null.
                if (queryResult != DBNull.Value)
                {
                    result = queryResult;
                }
            }

            return result;
        }

        /// <summary>
        /// Executes a SQL query that returns a single scalar value as the result.
        /// </summary>
        /// <param name="commandText">The query text to run.</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <param name="isStoredProcedure">Optional. If true, indicates that the command text is the name of the stored procedure to run rather than a normal query. Default is false.</param>
        /// <returns>Object representing the value of the query.</returns>
        /// <exception cref="ArgumentNullException">If the command text is null, empty, or whitespace.</exception>
        public async Task<object> QueryValueAsync(string commandText, Dictionary<string, object> parameters, bool isStoredProcedure = false)
        {
            object result = null;  // Return value of the scalar query

            // Throw exception if the command text is null or blank
            if (String.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            // Create the connection and command
            using (TConn connection = new TConn())
            using (TComm command = CreateCommand(connection, commandText, parameters, isStoredProcedure))
            {
                connection.ConnectionString = m_connectionString;

                // Open the connection then run the command
                connection.Open();

                var queryResult = await command.ExecuteScalarAsync();

                // Only set the result if it isn't null.
                if (queryResult != DBNull.Value)
                {
                    result = queryResult;
                }
            }

            return Task.FromResult<object>(result).Result;
        }

        #region Metadata Methods
        public bool DoesColumnExistInTable(string tableName, string columnName)
        {
            bool columnExists = false;

            string getColumn = @"SELECT 1
                                FROM information_schema.columns
                                WHERE table_name = @TableName AND column_name = @ColumnName";

            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                { "TableName", tableName },
                { "ColumnName", columnName }
            };

            var result = QueryValue(getColumn, parameters, false);

            columnExists = Convert.ToInt32(result) == 1;

            return columnExists;
        }

        /// <summary>
        /// Determines if the table with the specified name exists in the current database context.
        /// </summary>
        /// <param name="tableName">Name of the table to check for in the database.</param>
        /// <returns>True if the table exists, false if it does not.</returns>
        public bool DoesTableExist(string tableName)
        {
            bool tableExists = false;

            string getTable = @"SELECT 1 FROM information_schema.tables WHERE table_name = @TableName";

            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                { "TableName", tableName }
            };

            var result = QueryValue(getTable, parameters, false);

            tableExists = Convert.ToInt32(result) == 1;

            return tableExists;
        }

        /// <summary>
        /// Determines if the connection string for the context is valid.
        /// </summary>
        /// <returns>True if the connection is valid. False if it is not.</returns>
        /// <remarks>This doesn't check if a connection is open. It just checks that it can create a connection
        /// to the database.</remarks>
        public bool IsConnectionValid()
        {
            bool connIsValid = true;

            try
            {
                using (TConn connection = new TConn())
                {
                    connection.ConnectionString = m_connectionString;

                    // Open the connection then run the command
                    connection.Open();
                }
            }
            catch
            {
                connIsValid = false;
            }

            return connIsValid;
        }
        #endregion Metadata Methods


        #endregion IDatabaseContext Members

        #region Conversion methods

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a date/time.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        public virtual System.DateTime ConvertToDateTime(string value, System.DateTime defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
        }

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a date/time.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        public virtual System.DateTime? ConvertToNullableDateTime(string value, System.DateTime? defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a date/time.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        public virtual System.DateTime ConvertToDateTime(object value, System.DateTime defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            return Convert.ToDateTime(value);
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a date/time.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        public virtual System.DateTime? ConvertToNullableDateTime(object value, System.DateTime? defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            return Convert.ToDateTime(value);
        }

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a double precision float.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        public virtual double ConvertToDouble(string value, double defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a double precision float.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        public virtual double? ConvertToNullableDouble(string value, double? defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a double precision float.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        public virtual double ConvertToDouble(object value, double defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            return Convert.ToDouble(value);
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a double precision float.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        public virtual double? ConvertToNullableDouble(object value, double? defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            return Convert.ToDouble(value);
        }

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a single precision float.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        public virtual float ConvertToSingle(string value, float defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return float.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a single precision float.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        public virtual float? ConvertToNullableSingle(string value, float? defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return float.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a single precision float.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        public virtual float ConvertToSingle(object value, float defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            return Convert.ToSingle(value);
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a single precision float.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        public virtual float? ConvertToNullableSingle(object value, float? defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            return Convert.ToSingle(value);
        }

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a decimal.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        public virtual decimal ConvertToDecimal(string value, decimal defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a decimal.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        public virtual decimal? ConvertToNullableDecimal(string value, decimal? defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a decimal.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        public virtual decimal ConvertToDecimal(object value, decimal defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            return Convert.ToDecimal(value);
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a decimal.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        public virtual decimal? ConvertToNullableDecimal(object value, decimal? defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            return Convert.ToDecimal(value);
        }

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a 32-bit integer.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        public virtual int ConvertToInt32(string value, int defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return int.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a nullable 32-bit integer.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        public virtual int? ConvertToNullableInt32(string value, int? defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return int.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a 32-bit integer.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        public virtual int ConvertToInt32(object value, int defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a nullable 32-bit integer.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        public virtual int? ConvertToNullableInt32(object value, int? defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a 64-bit integer.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        public virtual long ConvertToInt64(string value, long defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return long.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a nullable 64-bit integer.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        public virtual long? ConvertToNullableInt64(string value, long? defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return long.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a 64-bit integer.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        public virtual long ConvertToInt64(object value, long defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            return Convert.ToInt64(value);
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a nullable 64-bit integer.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        public virtual long? ConvertToNullableInt64(object value, long? defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            return Convert.ToInt64(value);
        }

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a GUID.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        public virtual Guid ConvertToGuid(string value, Guid defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return Guid.Parse(value);
        }

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a nullable GUID.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        public virtual Guid? ConvertToNullableGuid(string value, Guid? defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return Guid.Parse(value);
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a GUID.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        public virtual Guid ConvertToGuid(object value, Guid defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            //Our base implementation assumes the database stores the value as a uniqueidentifier.
            //Some future contexts (like SQLite) may store guids as a strings and thus may need to override the conversion logic.
            return (Guid)value;
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a nullable GUID.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        public virtual Guid? ConvertToNullableGuid(object value, Guid? defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            //Our base implementation assumes the database stores the value as a uniqueidentifier.
            //Some future contexts (like SQLite) may store guids as a strings and thus may need to override the conversion logic.
            return (Guid)value;
        }

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a 16-bit integer
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        public virtual short ConvertToInt16(string value, short defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return short.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a nullable 16-bit integer
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        public virtual short? ConvertToNullableInt16(string value, short? defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return short.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a 16-bit integer
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        public virtual short ConvertToInt16(object value, short defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            return Convert.ToInt16(value);
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a nullable 16-bit integer
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        public virtual short? ConvertToNullableInt16(object value, short? defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            return Convert.ToInt16(value);
        }

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to an 8-bit integer
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        public virtual byte ConvertToByte(string value, byte defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return byte.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a nullable 8-bit integer
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        public virtual byte? ConvertToNullableByte(string value, byte? defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return byte.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to an 8-bit integer
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        public virtual byte ConvertToByte(object value, byte defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            return Convert.ToByte(value);
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a nullable 8-bit integer
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        public virtual byte? ConvertToNullableByte(object value, byte? defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            return Convert.ToByte(value);
        }

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a bool
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        /// <returns>
        /// The boolean representation of the given value; otherwise, the default value passed in.
        /// </returns>
        /// <exception cref="System.FormatException">String was not recognized as a valid Boolean or Integer.</exception>
        public virtual bool ConvertToBool(string value, bool defaultValue = default(bool))
        {
            if (!string.IsNullOrEmpty(value))
            {
                // If the value is from a boolean field it should parse correctly here.
                bool bValue;
                if (bool.TryParse(value, out bValue))
                    return bValue;

                // If the value is from an int-type field we need to try to parse to an int first.
                int iValue;
                if (int.TryParse(value, out iValue))
                    return Convert.ToBoolean(iValue);   // This will return true if the value is not zero.

                throw new FormatException("String was not recognized as a valid Boolean or Integer.");
            }

            return defaultValue;
        }

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a nullable bool
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        /// <returns>
        /// The nullable boolean representation of the given value; otherwise, the default value passed in.
        /// </returns>
        public virtual bool? ConvertToNullableBool(string value, bool? defaultValue = default(bool?))
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            // We know the value string has data so convert it to a standard boolean value.
            return ConvertToBool(value);
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a bool
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        /// <returns>
        /// The boolean representation of the given value; otherwise, the default value passed in.
        /// </returns>
        /// <exception cref="System.FormatException">String was not recognized as a valid Boolean or Integer.</exception>
        public virtual bool ConvertToBool(object value, bool defaultValue = default(bool))
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            return Convert.ToBoolean(value);
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a nullable bool
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        /// <returns>
        /// The nullable boolean representation of the given value; otherwise, the default value passed in.
        /// </returns>
        public virtual bool? ConvertToNullableBool(object value, bool? defaultValue = default(bool?))
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            return Convert.ToBoolean(value);
        }

        /// <summary>
        /// Converts an object result from the QueryStrings or QueryStringsAsync methods to a string
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        public virtual string ConvertToString(string value, string defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return value;
        }

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a string
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        public virtual string ConvertToString(object value, string defaultValue)
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            return value.ToString();
        }

        #endregion Conversion methods

        #region Private Methods
        private TComm CreateCommand(TConn connection, string commandText, Dictionary<string, object> parameters, bool isStoredProcedure)
        {
            TComm command = new TComm();
            command.Connection = connection;
            command.CommandText = commandText;

            if (isStoredProcedure)
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
            }

            // Add in the parameters
            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> param in parameters)
                {
                    TParam p = new TParam();
                    p.ParameterName = param.Key;
                    p.Value = param.Value ?? DBNull.Value;  // If there isn't a value, pass in DBNULL

                    command.Parameters.Add(p);
                }
            }

            return command;
        }

        /// <summary>
        /// Reads one value and converts it into a culture-invariant string that can later be converted back to the original value.
        /// </summary>
        private static string ReadValueAsString(System.Data.Common.DbDataReader reader, int fieldIndex)
        {
            string columnValue = null;
            if (!reader.IsDBNull(fieldIndex))
            {
                Type fieldType = reader.GetFieldType(fieldIndex);
                if (fieldType == typeof(double))
                {
                    columnValue = reader.GetDouble(fieldIndex).ToString("R", System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (fieldType == typeof(float))
                {
                    columnValue = reader.GetFloat(fieldIndex).ToString("R", System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (fieldType == typeof(decimal))
                {
                    columnValue = reader.GetDecimal(fieldIndex).ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (fieldType == typeof(DateTime))
                {
                    columnValue = reader.GetDateTime(fieldIndex).ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    columnValue = reader[fieldIndex].ToString();
                }
            }
            return columnValue;
        }

        #endregion Private Methods
    }
}
