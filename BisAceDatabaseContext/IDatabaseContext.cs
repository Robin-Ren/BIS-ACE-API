using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BisAceDatabaseContext
{
    /// <summary>
    /// Interface generalizing running database queries
    /// </summary>
    public interface IDatabaseContext
    {
        #region Synchronous methods
        /// <summary>
        /// Executes a non-query SQL statement
        /// </summary>
        /// <param name="commandText">The command text to execute</param>
        /// <param name="parameters">Optional parameters to pass to the command</param>
        /// <param name="isStoredProcedure">Optional. If true, indicates that the command text is the name of the stored procedure to run rather than a normal query. Default is false.</param>
        /// <returns>The number of records affected by the command</returns>
        /// <exception cref="ArgumentNullException">If the command text is null, empty, or whitespace.</exception>
        int Execute(string commandText, Dictionary<string, object> parameters, bool isStoredProcedure = false);

        /// <summary>
        /// Executes a SQL query that returns a list of rows as the result.
        /// </summary>
        /// <param name="commandText">The MySQL query to execute</param>
        /// <param name="parameters">Parameters to pass to the query. If no parameters are needed, null can be passed in.</param>
        /// <param name="isStoredProcedure">Optional. If true, indicates that the command text is the name of the stored procedure to run rather than a normal query. Default is false.</param>
        /// <returns>A list of all rows returned by the query. Each row is represented by a dictionary representing the 
        /// column name and corresponding value for each column, both as strings. If the query was not run, null will be returned.</returns>
        /// <exception cref="ArgumentNullException">If the command text is null, empty, or whitespace.</exception>
        List<Dictionary<string, string>> QueryStrings(string commandText, Dictionary<string, object> parameters, bool isStoredProcedure = false);

        /// <summary>
        /// Executes a SQL query that returns a list of rows as the result.
        /// </summary>
        /// <param name="commandText">The MySQL query to execute</param>
        /// <param name="parameters">Parameters to pass to the query. If no parameters are needed, null can be passed in.</param>
        /// <param name="isStoredProcedure">Optional. If true, indicates that the command text is the name of the stored procedure to run rather than a normal query. Default is false.</param>
        /// <returns>A list of all rows returned by the query. Each row is represented by a dictionary representing the 
        /// column name and corresponding value for each column. If the query was not run, null will be returned.</returns>
        /// <exception cref="ArgumentNullException">If the command text is null, empty, or whitespace.</exception>
        List<Dictionary<string, object>> QueryValues(string commandText, Dictionary<string, object> parameters, bool isStoredProcedure = false);

        /// <summary>
        /// Executes a SQL query that returns a single string value as the result.
        /// </summary>
        /// <param name="commandText">The query text to run.</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <param name="isStoredProcedure">Optional. If true, indicates that the command text is the name of the stored procedure to run rather than a normal query. Default is false.</param>
        /// <returns>String representing the value of the query.</returns>
        /// <exception cref="ArgumentNullException">If the command text is null, empty, or whitespace.</exception>
        string QueryString(string commandText, Dictionary<string, object> parameters, bool isStoredProcedure = false);

        /// <summary>
        /// Executes a SQL query that returns a single scalar value as the result.
        /// </summary>
        /// <param name="commandText">The query text to run.</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <param name="isStoredProcedure">Optional. If true, indicates that the command text is the name of the stored procedure to run rather than a normal query. Default is false.</param>
        /// <returns>Object representing the value of the query.</returns>
        /// <exception cref="ArgumentNullException">If the command text is null, empty, or whitespace.</exception>
        object QueryValue(string commandText, Dictionary<string, object> parameters, bool isStoredProcedure = false);
        #endregion Synchronous methods

        #region Asynchronous methods
        /// <summary>
        /// Asynchronously executes a non-query SQL statement
        /// </summary>
        /// <param name="commandText">The command text to execute</param>
        /// <param name="parameters">Optional parameters to pass to the command</param>
        /// <param name="isStoredProcedure">Optional. If true, indicates that the command text is the name of the stored procedure to run rather than a normal query. Default is false.</param>
        /// <returns>The number of records affected by the command</returns>
        /// <exception cref="ArgumentNullException">If the command text is null, empty, or whitespace.</exception>
        Task<int> ExecuteAsync(string commandText, Dictionary<string, object> parameters, bool isStoredProcedure = false);

        /// <summary>
        /// Asynchronously executes a SQL query that returns a list of rows as the result.
        /// </summary>
        /// <param name="commandText">The MySQL query to execute</param>
        /// <param name="parameters">Parameters to pass to the query. If no parameters are needed, null can be passed in.</param>
        /// <param name="isStoredProcedure">Optional. If true, indicates that the command text is the name of the stored procedure to run rather than a normal query. Default is false.</param>
        /// <returns>A list of all rows returned by the query. Each row is represented by a dictionary representing the 
        /// column name and corresponding value for each column, both as strings. If the query was not run, null will be returned.</returns>
        /// <exception cref="ArgumentNullException">If the command text is null, empty, or whitespace.</exception>
        Task<List<Dictionary<string, string>>> QueryStringsAsync(string commandText, Dictionary<string, object> parameters, bool isStoredProcedure = false);

        /// <summary>
        /// Asynchronously executes a SQL query that returns a list of rows as the result.
        /// </summary>
        /// <param name="commandText">The MySQL query to execute</param>
        /// <param name="parameters">Parameters to pass to the query. If no parameters are needed, null can be passed in.</param>
        /// <param name="isStoredProcedure">Optional. If true, indicates that the command text is the name of the stored procedure to run rather than a normal query. Default is false.</param>
        /// <returns>A list of all rows returned by the query. Each row is represented by a dictionary representing the 
        /// column name and corresponding value for each column. If the query was not run, null will be returned.</returns>
        /// <exception cref="ArgumentNullException">If the command text is null, empty, or whitespace.</exception>
        Task<List<Dictionary<string, object>>> QueryValuesAsync(string commandText, Dictionary<string, object> parameters, bool isStoredProcedure = false);

        /// <summary>
        /// Asynchronously executes a SQL query that returns a single string value as the result.
        /// </summary>
        /// <param name="commandText">The query text to run.</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <param name="isStoredProcedure">Optional. If true, indicates that the command text is the name of the stored procedure to run rather than a normal query. Default is false.</param>
        /// <returns>String representing the value of the query.</returns>
        /// <exception cref="ArgumentNullException">If the command text is null, empty, or whitespace.</exception>
        Task<string> QueryStringAsync(string commandText, Dictionary<string, object> parameters, bool isStoredProcedure = false);

        /// <summary>
        /// Asynchronously executes a SQL query that returns a single scalar value as the result.
        /// </summary>
        /// <param name="commandText">The query text to run.</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <param name="isStoredProcedure">Optional. If true, indicates that the command text is the name of the stored procedure to run rather than a normal query. Default is false.</param>
        /// <returns>Object representing the value of the query.</returns>
        /// <exception cref="ArgumentNullException">If the command text is null, empty, or whitespace.</exception>
        Task<object> QueryValueAsync(string commandText, Dictionary<string, object> parameters, bool isStoredProcedure = false);
        #endregion Asynchronous methods

        #region Metadata methods
        /// <summary>
        /// Determins if the column with the specified name exists in the specified table in the current database context.
        /// </summary>
        /// <param name="tableName">Name of the table to check for in the database.</param>
        /// <param name="columnName">Name of the column to check for in the database.</param>
        /// <returns>True if the column exists, false if it does not.</returns>
        bool DoesColumnExistInTable(string tableName, string columnName);

        /// <summary>
        /// Determines if the table with the specified name exists in the current database context.
        /// </summary>
        /// <param name="tableName">Name of the table to check for in the database.</param>
        /// <returns>True if the table exists, false if it does not.</returns>
        bool DoesTableExist(string tableName);

        /// <summary>
        /// Determines if the connection string for the context is valid.
        /// </summary>
        /// <returns>True if the connection is valid. False if it is not.</returns>
        /// <remarks>This doesn't check if a connection is open. It just checks that it can create a connection
        /// to the database.</remarks>
        bool IsConnectionValid();
        #endregion Metadata methods

        #region Conversion methods

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a date/time.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        System.DateTime ConvertToDateTime(string value, System.DateTime defaultValue);

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a nullable date/time.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        System.DateTime? ConvertToNullableDateTime(string value, System.DateTime? defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a date/time.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        System.DateTime ConvertToDateTime(object value, System.DateTime defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a nullable date/time.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        System.DateTime? ConvertToNullableDateTime(object value, System.DateTime? defaultValue);

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a double precision float.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        double ConvertToDouble(string value, double defaultValue);

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a nullable double precision float.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        double? ConvertToNullableDouble(string value, double? defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a double precision float.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        double ConvertToDouble(object value, double defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a nullable double precision float.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        double? ConvertToNullableDouble(object value, double? defaultValue);

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a single precision float.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        float ConvertToSingle(string value, float defaultValue);

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a nullable single precision float.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        float? ConvertToNullableSingle(string value, float? defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a single precision float.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        float ConvertToSingle(object value, float defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a nullable single precision float.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        float? ConvertToNullableSingle(object value, float? defaultValue);

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a decimal.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        decimal ConvertToDecimal(string value, decimal defaultValue);

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a nullable decimal.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        decimal? ConvertToNullableDecimal(string value, decimal? defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a decimal.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        decimal ConvertToDecimal(object value, decimal defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a nullable decimal.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        decimal? ConvertToNullableDecimal(object value, decimal? defaultValue);

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a 32-bit integer.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        int ConvertToInt32(string value, int defaultValue);

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a nullable 32-bit integer.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        int? ConvertToNullableInt32(string value, int? defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a 32-bit integer.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        int ConvertToInt32(object value, int defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a nullable 32-bit integer.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        int? ConvertToNullableInt32(object value, int? defaultValue);

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a 64-bit integer.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        long ConvertToInt64(string value, long defaultValue);

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a nullable 64-bit integer.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        long? ConvertToNullableInt64(string value, long? defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a 64-bit integer.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        long ConvertToInt64(object value, long defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a nullable 64-bit integer.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        long? ConvertToNullableInt64(object value, long? defaultValue);

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a GUID.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        Guid ConvertToGuid(string value, Guid defaultValue);

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a nullable GUID.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        Guid? ConvertToNullableGuid(string value, Guid? defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a GUID.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        Guid ConvertToGuid(object value, Guid defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a nullable GUID.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        Guid? ConvertToNullableGuid(object value, Guid? defaultValue);

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a 16-bit integer.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        short ConvertToInt16(string value, short defaultValue);

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a nullable 16-bit integer.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        short? ConvertToNullableInt16(string value, short? defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a 16-bit integer.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        short ConvertToInt16(object value, short defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a nullable 16-bit integer.
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        short? ConvertToNullableInt16(object value, short? defaultValue);

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to an 8-bit integer
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        byte ConvertToByte(string value, byte defaultValue);

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a nullable 8-bit integer
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        byte? ConvertToNullableByte(string value, byte? defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to an 8-bit integer
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        byte ConvertToByte(object value, byte defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a nullable 8-bit integer
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        byte? ConvertToNullableByte(object value, byte? defaultValue);

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a bool
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        bool ConvertToBool(string value, bool defaultValue);

        /// <summary>
        /// Converts a string result from the QueryStrings or QueryStringsAsync methods to a nullable bool
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        bool? ConvertToNullableBool(string value, bool? defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a bool
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        bool ConvertToBool(object value, bool defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a nullable bool
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        bool? ConvertToNullableBool(object value, bool? defaultValue);

        /// <summary>
        /// Converts an object result from the QueryStrings or QueryStringsAsync methods to a string
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null/empty.</param>
        string ConvertToString(string value, string defaultValue);

        /// <summary>
        /// Converts an object result from the QueryValues or QueryValuesAsync methods to a string
        /// </summary>
        /// <param name="value">Value returned from the database</param>
        /// <param name="defaultValue">Output to use if the value is null.</param>
        string ConvertToString(object value, string defaultValue);

        #endregion Conversion methods
    }
}
