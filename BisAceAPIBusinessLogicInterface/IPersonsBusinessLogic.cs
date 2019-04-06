using System.Threading.Tasks;
using BisAceAPIModels.Models;

namespace BisAceAPIBusinessLogicInterface
{
    public interface IPersonsBusinessLogic
    {
        /// <summary>
        /// Get person information by given ace person.
        /// </summary>
        /// <param name="ace">BIS Access engine.</param>
        /// <param name="personNumber">Person number.</param>
        /// <returns>Instance of IBisResult.</returns>
        IBisResult GetPerson(AccessEngine ace, string personId);

        /// <summary>
        /// Update person data by card information.
        /// </summary>
        /// <param name="ace">BIS Access engine.</param>
        /// <param name="card">Input card information.</param>
        /// <param name="person">Person instance to update.</param>
        IBisResult UpdatePersonFromCardInfo(AccessEngine ace, BisCard card, ACEPersons person);
    }
}
