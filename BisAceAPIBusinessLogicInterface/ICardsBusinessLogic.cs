using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BisAceAPIModels.Models;

namespace BisAceAPIBusinessLogicInterface
{
    public interface ICardsBusinessLogic
    {
        /// <summary>
        /// Validate if card exists by given card number.
        /// </summary>
        /// <param name="ace">BIS Access engine.</param>
        /// <param name="cardNumber">Card number.</param>
        /// <returns>Instance of IBisResult.</returns>
        IBisResult ValidateCardExist(AccessEngine ace, string cardNumber);

        /// <summary>
        /// Get card identifier by given card number
        /// </summary>
        /// <param name="cardNumber">Specified card number.</param>
        /// <param name="ace">BIS Access engine.</param>
        /// <returns>Card ID if found.</returns>
        string GetCardId(string strCardNo, AccessEngine ace);

        /// <summary>
        /// Get card information by given ace card.
        /// </summary>
        /// <param name="ace">BIS Access engine.</param>
        /// <param name="cardNumber">Card number.</param>
        /// <returns>Instance of IBisResult.</returns>
        IBisResult PopulateCardFromACECards(AccessEngine ace, ACECards aceCard);

        /// <summary>
        /// Save card by input card information
        /// </summary>
        /// <param name="ace">BIS Access engine.</param>
        /// <param name="card">Contains required information of card.</param>
        /// <returns></returns>
        IBisResult CreateCard(AccessEngine ace, BisCard card);

        /// <summary>
        /// Update card by input card information
        /// </summary>
        /// <param name="ace">BIS Access engine.</param>
        /// <param name="aceCard">Existing ACECards instance.</param>
        /// <param name="card">Contains required information of card.</param>
        IBisResult UpdateCard(AccessEngine ace, ACECards aceCard, BisCard card);

        /// <summary>
        /// Delete card.
        /// </summary>
        /// <param name="aceCard">The ACECards instance to delete.</param>
        /// <returns>API_RETURN_CODES_CS Code of delete operation.</returns>
        API_RETURN_CODES_CS DeleteCard(ACECards aceCard);
    }
}
