using System;
using System.Data.SqlClient;

namespace BisAceDatabaseContext
{
    public class SqlServerDatabase : ADatabase<SqlConnection, SqlCommand, SqlParameter>
    {
        #region Constructors
        /// <summary>
        /// Empty Constructor to help DeSerialization
        /// </summary>
        public SqlServerDatabase(){
        }
        /// <summary>
        /// Constructor. Sets the connection string.
        /// </summary>
        /// <param name="connString">Connection string to the database the queries should run against.</param>
        /// <exception cref="ArgumentNullException">If the connection string is null, empty, or whitespace.</exception>
        public SqlServerDatabase(string connString)
        {
            if (string.IsNullOrWhiteSpace(connString))
            {
                throw new ArgumentNullException("connString", "The connection string cannot be null or empty.");
            }

            m_connectionString = connString;
        }
        
        #endregion
    }
}