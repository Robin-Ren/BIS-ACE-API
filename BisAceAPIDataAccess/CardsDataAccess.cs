using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BisAceAPIDataAccessInterface;
using BisAceDatabaseContext;

namespace BisAceAPIDataAccess
{
    public class CardsDataAccess : ICardsDataAccess
    {
        #region Private Members
        private IDatabaseContext _dbContext;
        #endregion Private Members

        #region Constructors
        /// <summary>
        /// Constructor. Takes in the database context object to use
        /// </summary>
        /// <param name="databaseContext">Database context to use to make queries.</param>
        public CardsDataAccess(IDatabaseContext databaseContext)
        {
            _dbContext = databaseContext;
        }
        #endregion
    }
}
