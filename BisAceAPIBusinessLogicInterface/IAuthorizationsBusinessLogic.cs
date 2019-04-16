using System.Collections.Generic;
using System.Threading.Tasks;
using BisAceAPIModels.Models;

namespace BisAceAPIBusinessLogicInterface
{
    public interface IAuthorizationsBusinessLogic
    {
        List<ACEAuthorizations> GetAuthorizationsForPersonId(AccessEngine ace, string personId);
        IBisResult GetAllAuthorizationsForActiveCards(AccessEngine ace);
    }
}
