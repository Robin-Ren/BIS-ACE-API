using MySql.Data.MySqlClient;
using System;

namespace BisAceDatabaseContext
{
    public class MySqlDatabase : ADatabase<MySqlConnection, MySqlCommand, MySqlParameter>
    {
        #region Constructors
        public MySqlDatabase(string connString)
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
