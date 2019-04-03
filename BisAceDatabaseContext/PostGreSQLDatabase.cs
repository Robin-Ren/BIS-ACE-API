using Npgsql;
using System;

namespace BisAceDatabaseContext
{
    public class PostGreSQLDatabase : ADatabase<NpgsqlConnection, NpgsqlCommand, NpgsqlParameter>
    {
        #region Constructors
        public PostGreSQLDatabase(string connString)
        {
            if (string.IsNullOrWhiteSpace(connString))
            {
                throw new ArgumentNullException("connString", "The connection string cannot be null or empty.");
            }

            //MinPoolSize defaults to 1 for NpgsqlConnection.
            //We're used to a MinPoolSize of 0 with SqlConnection.
            //Leaving it at 1 greatly increases the risk that it'll reuse a connection that has already been closed by the server,
            // and thus would hit socket exceptions when running commands.
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(connString);
            if (builder.MinPoolSize == 1 && builder.Pooling)
            {
                builder.MinPoolSize = 0;
                connString = builder.ConnectionString;
            }

            m_connectionString = connString;
        }
        #endregion
    }
}
