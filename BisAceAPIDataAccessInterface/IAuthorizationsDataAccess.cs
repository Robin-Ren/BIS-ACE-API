﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BisAceAPIModels.Models;

namespace BisAceAPIDataAccessInterface
{
    public interface IAuthorizationsDataAccess
    {
        List<ACEAuthorizations> GetAuthorizationsForViaAuthPerPerson(AccessEngine ace, string personId);

        List<ACEAuthorizations> GetAuthorizationsForViaAuthProfile(AccessEngine ace, string personId);

        /// <summary>
        /// Get all active cards identifiers.
        /// </summary>
        /// <param name="ace">BIS Access engine.</param>
        /// <returns>All Card IDs (Tuple.Item1) and related Person IDs (Tuple.Item2).</returns>
        List<Tuple<string, string>> GetAllCardIds(AccessEngine ace);
    }
}
