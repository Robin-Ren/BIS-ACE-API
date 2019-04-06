using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BisAceAPIDataAccessInterface;
using BisAceDatabaseContext;

namespace BisAceAPIDataAccess
{
    public class DoorAccessGroupsDataAccess : IDoorAccessGroupsDataAccess
    {
        #region Private Members
        private readonly IDatabaseContext _dbContext;
        #endregion Private Members

        #region Constructors
        /// <summary>
        /// Constructor. Takes in the database context object to use
        /// </summary>
        /// <param name="databaseContext">Database context to use to make queries.</param>
        public DoorAccessGroupsDataAccess(IDatabaseContext databaseContext)
        {
            _dbContext = databaseContext;
        }
        #endregion
    }
}
