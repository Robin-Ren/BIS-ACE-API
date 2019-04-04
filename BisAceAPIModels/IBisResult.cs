using System.Collections.Generic;
using System.Threading.Tasks;
using BisAceAPIModels.Models.Enums;

namespace BisAceAPIModels.Models
{
    /// <summary>
    /// Interface for working with action results in WebCP
    /// </summary>
    public interface IBisResult
    {
        /// <summary>
        /// Gets a flag indicating whether the result is valid or not.
        /// </summary>
        bool IsSucceeded { get; }

        /// <summary>
        /// The type of error on the result.
        /// </summary>
        BisErrorType ErrorType { get; set; }

        /// <summary>
        /// The primary error message on the result.
        /// </summary>
        string ErrorMessage { get; set; }

        object Model { get; }

        /// <summary>
        /// Adds an error message to the result.
        /// </summary>
        /// <param name="message">The message to add to the result.</param>
        void AddErrorMessage(string message);

        /// <summary>
        /// Adds a list of error message to the result.
        /// </summary>
        /// <param name="messages">The messages to add to the result.</param>
        void AddErrorMessages(IEnumerable<string> messages);

        /// <summary>
        /// Adds an error message which has already been localized
        /// </summary>
        /// <param name="messages">messages to add</param>
        void AddLocalizedErrorMessage(string message);

        /// <summary>
        /// Creates an error response model to return from the API.
        /// </summary>
        /// <returns>
        /// A new response model containing the info about the error which occurred.
        /// </returns>
        BisErrorResponseViewModel CreateErrorResponseModel();

        /// <summary>
        /// Gets the collection of error messages on the result.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{string}"/> containing the error messages. If there
        /// aren't any messages, the collection will be empty.</returns>
        IEnumerable<string> GetErrorMessages();

        /// <summary>
        /// Gets the resource stored in the result.
        /// </summary>
        /// <typeparam name="T">The type of the resource stored.</typeparam>
        /// <returns>The resource cast as the type.</returns>
        T GetResource<T>() where T : class;

        /// <summary>
        /// Merges another IWcpResult with this one. The error type and primary error message from the merging one
        /// will only overwrite the values in this one if this one doesn't have data for those fields.
        /// </summary>
        /// <param name="otherResult">The result to merge with this one.</param>
        void MergeWithOtherResult(IBisResult otherResult);

        /// <summary>
        /// Saves the resource to the result.
        /// </summary>
        /// <param name="resource">Resource saved to the result.</param>
        void SetResource(object resource);
    }
}
