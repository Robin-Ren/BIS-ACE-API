using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BisAceAPIDataAccessInterface;
using BisAceDatabaseContext;

namespace BisAceAPIDataAccess
{
    public class AuthorizationsDataAccess : IAuthorizationsDataAccess
    {
        #region Private Members
        private readonly IDatabaseContext _dbContext;
        #endregion Private Members

        #region Constructors
        /// <summary>
        /// Constructor. Takes in the database context object to use
        /// </summary>
        /// <param name="databaseContext">Database context to use to make queries.</param>
        public AuthorizationsDataAccess(IDatabaseContext databaseContext)
        {
            _dbContext = databaseContext;
        }
        #endregion

        public List<ACEAuthorizations> GetAuthorizationsForViaAuthPerPerson(AccessEngine ace, string personId)
        {
            List<ACEAuthorizations> listAuthorizations = new List<ACEAuthorizations>();

            // Directly Assigned Authorizations
            var query = new ACEQuery(ace);
            String str_Columns = "bsuser.authorizations.authid";
            String str_Tables = "bsuser.authperperson JOIN bsuser.authorizations on(bsuser.authperperson.authid = bsuser.authorizations.authid) ";
            String str_Where = string.Format("bsuser.authperperson.persid = \'{0}\'", personId);

            API_RETURN_CODES_CS result = query.Select(str_Columns, str_Tables, str_Where);


            if (API_RETURN_CODES_CS.API_SUCCESS_CS == result)
            {
                var AuthIds = new List<string>();
                ACEColumnValue cellValueID;

                while (query.FetchNextRow())
                {
                    cellValueID = new ACEColumnValue();

                    result = query.GetRowData("authid", cellValueID);

                    AuthIds.Add(cellValueID.value);
                }

                if (AuthIds.Count > 0)
                {
                    foreach (var authId in AuthIds)
                    {
                        ACEAuthorizations auth = new ACEAuthorizations(ace);
                        result = auth.Get(authId);

                        if (API_RETURN_CODES_CS.API_SUCCESS_CS == result)
                        {
                            listAuthorizations.Add(auth);
                        }
                    }
                }
            }

            return listAuthorizations;
        }

        public List<ACEAuthorizations> GetAuthorizationsForViaAuthProfile(AccessEngine ace, string personId)
        {
            List<ACEAuthorizations> listAuthorizations = new List<ACEAuthorizations>();

            // Directly Assigned Authorizations
            var query = new ACEQuery(ace);
            String str_Columns = "AU.authid";
            String str_Tables = @"bsuser.ACPERSONS ap
                    join bsuser.AUTHPROFILES aup on aup.PROFILEID=ap.AUTHPROFILEID
                    JOIN bsuser.AUTHPERPROFILE app on aup.PROFILEID=app.PROFILEID
                    join bsuser.AUTHORIZATIONS AU on AU.AUTHID=app.AUTHID";
            String str_Where = string.Format("ap.persid = \'{0}\'", personId);

            API_RETURN_CODES_CS result = query.Select(str_Columns, str_Tables, str_Where);

            if (API_RETURN_CODES_CS.API_SUCCESS_CS == result)
            {
                var AuthIds = new List<string>();
                ACEColumnValue cellValueID;

                while (query.FetchNextRow())
                {
                    cellValueID = new ACEColumnValue();

                    result = query.GetRowData("authid", cellValueID);

                    AuthIds.Add(cellValueID.value);
                }

                if (AuthIds.Count > 0)
                {
                    foreach (var authId in AuthIds)
                    {
                        ACEAuthorizations auth = new ACEAuthorizations(ace);
                        result = auth.Get(authId);

                        if (API_RETURN_CODES_CS.API_SUCCESS_CS == result)
                        {
                            listAuthorizations.Add(auth);
                        }
                    }
                }
            }

            return listAuthorizations;
        }

        /// <summary>
        /// Get all active cards identifiers.
        /// </summary>
        /// <param name="ace">BIS Access engine.</param>
        /// <returns>All Card IDs.</returns>
        public List<string> GetAllCardIds(AccessEngine ace)
        {
            List<string> allCardIds = new List<string>();
            // Create query
            var ace_Query = new ACEQuery(ace);
            string strColumn = "cardid,persid";
            string strTable = "bsuser.cards";
            string strWhere = "status > 0";

            API_RETURN_CODES_CS result = ace_Query.Select(strColumn, strTable, strWhere);

            if (result == API_RETURN_CODES_CS.API_SUCCESS_CS)
            {
                var cardid = new ACEColumnValue();
                var personid = new ACEColumnValue();

                while (ace_Query.FetchNextRow())
                {
                    result = ace_Query.GetRowData("cardid", cardid);
                    //result = ace_Query.GetRowData("persid", personid);

                    if (result == API_RETURN_CODES_CS.API_SUCCESS_CS)
                    {
                        allCardIds.Add(cardid.value);
                    }
                }
            }
            return allCardIds;
        }
    }
}
