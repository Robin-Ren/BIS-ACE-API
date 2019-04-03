using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using BisAceAPIModels.Models;

namespace BisAceAPIBase
{
    public class BisApplicationConfig : IBisApplicationConfig
    {
        #region Private Properties

        #endregion

        #region Properties
        /// <summary>
        /// The connection string to the Centerpoint database
        /// </summary>
        public string BISConnectionString { get; private set; }

        /// <summary>
        /// Gets the list of Option files. 
        /// You should specify the full path of a file
        /// If you have multiple files, you should use ';' as seperator
        /// </summary>
        public string OptionFiles { get; private set; }
        #endregion Properties

        #region Constructor

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="settings">settings values</param>
        /// <param name="systemId">system ID</param>
        /// <param name="configDirectory">directory where the configuration file lives</param>
        public BisApplicationConfig(IDictionary<string, string> settings)
        {
            var baseConnectionString = ConfigurationManager.AppSettings["BisConnectionString"];

            if (settings.TryGetValue("BisAceDatabaseName", out string dbName))
            {
                SqlConnectionStringBuilder builder;
                try
                {
                    builder = new SqlConnectionStringBuilder(baseConnectionString);
                }
                catch
                {
                    throw new BisException("Invalid base connection string in configuration!");
                }

                builder.InitialCatalog = dbName;
                BISConnectionString = builder.ToString();
            }
            else
            {
                throw new BisException("BisAceDatabaseName Not Specified In The Settings File!");
            }
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
