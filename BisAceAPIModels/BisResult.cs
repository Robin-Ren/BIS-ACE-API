using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using BisAceAPIModels.Models.Enums;
using BisAceAPIModels.Utils;

namespace BisAceAPIModels.Models
{
    public class BisResult : IBisResult
    {
        #region Private Members
        /// <summary>
        /// The collection of error messages on the result
        /// </summary>
        private List<string> _errorMessages = new List<string>();

        /// <summary>
        /// The primary error message string
        /// </summary>
        private string _primaryErrorMessage;

        /// <summary>
        /// The resource stored on the result.
        /// </summary>
        private object _resource;

        /// <summary>
        /// Factory to create error responses;
        /// </summary>
        private Func<IBisErrorResponseViewModel> _errorResponseFactory;
        #endregion

        /// <summary>
        /// Constructor. Creates a new instance of a BisResult.
        /// </summary>
        /// <param name="errorResponseFactory">Factory to create error response messages.</param>
        public BisResult(Func<IBisErrorResponseViewModel> errorResponseFactory)
        {
            _errorResponseFactory = errorResponseFactory;
        }

        #region IBisResult Members
        /// <summary>
        /// Gets a flag indicating whether the result is valid or not.
        /// </summary>
        public bool IsSucceeded
        {
            get
            {
                return ErrorType == BisErrorType.None &&
                    string.IsNullOrEmpty(_primaryErrorMessage) &&
                    _errorMessages.Count == 0;
            }
        }

        /// <summary>
        /// Gets the resource stored in the result.
        /// </summary>
        /// <typeparam name="T">The type of the resource stored.</typeparam>
        /// <returns>The resource cast as the type.</returns>
        public object Model
        {
            get
            {
                return _resource ?? null;
            }
        }


        /// <summary>
        /// The type of error on the result.
        /// </summary>
        public BisErrorType ErrorType { get; set; } = BisErrorType.None;

        /// <summary>
        /// The primary error message on the result.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return _primaryErrorMessage;
            }

            set
            {
                _primaryErrorMessage = value;
            }
        }

        /// <summary>
        /// Adds an error message to the result.
        /// </summary>
        /// <param name="message">The message to add to the result.</param>
        public void AddErrorMessage(string message)
        {
            // Only add in the message if it isn't empty
            if (!string.IsNullOrEmpty(message))
            {
                _errorMessages.Add(message);
            }
        }

        /// <summary>
        /// Adds an error message which has already been localized
        /// </summary>
        /// <param name="message">The localized error message</param>
        public void AddLocalizedErrorMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                _errorMessages.Add(message);
            }
        }

        /// <summary>
        /// Adds a list of error message to the result.
        /// </summary>
        /// <param name="messages">The messages to add to the result.</param>
        public void AddErrorMessages(IEnumerable<string> messages)
        {
            // Only process the list if it exists.
            if (messages != null)
            {
                foreach (var message in messages)
                    AddErrorMessage(message);
            }
        }

        /// <summary>
        /// This method takes n string messages and concats them into one single error message
        /// </summary>
        /// <param name="messages">array of message strings</param>
        /// <remarks>
        /// Each individual string in the message array parameter will attempt a translation
        /// </remarks>
        public void AddCompoundErrorMessage(params string[] messages)
        {
            StringBuilder sbTranslatedMessage = new StringBuilder();

            for (int i = 0; i < messages.Length; i++)
            {
                // append the translated messages into the string builder
                sbTranslatedMessage.Append(messages[i]);

                // add a space between words
                if (i < messages.Length - 1)
                    sbTranslatedMessage.Append(" ");
            }

            // now add the final translated message
            this.AddLocalizedErrorMessage(sbTranslatedMessage.ToString());
        }


        /// <summary>
        /// Creates an error response model to return from the API.
        /// </summary>
        /// <returns>A new response model containing the info about the error which occurred.</returns>
        public BisErrorResponseViewModel CreateErrorResponseModel()
        {
            // Only create a response model if there is an error
            if (!IsSucceeded)
            {
                // Create the new model
                var responseVM = _errorResponseFactory();

                // If the error type is None, set it to Other since there is obviously an error
                var errorEnum = ErrorType == BisErrorType.None ? BisErrorType.Other : ErrorType;
                responseVM.Error = EnumHelper.GetEnumDescription(errorEnum);

                // If we have a Primary message, set that as the model's error message 
                // and use the list of error messages as the related messages.
                if (!string.IsNullOrEmpty(_primaryErrorMessage))
                {
                    responseVM.ErrorDescription = _primaryErrorMessage;
                }

                // Set the model if we want it included and it is set
                if (_resource != null)
                {
                    // Update the model, used when returning to an API controller.
                    responseVM.Model = _resource;

                    // Update the dynamic model property, used when returning to an OData controller.
                    // It should be empty unless a custom process added a property with the same name.
                    if (!responseVM.DynamicProperties.ContainsKey("Model"))
                        responseVM.DynamicProperties.Add("Model", responseVM.Model);
                }


                return responseVM as BisErrorResponseViewModel;
            }

            return null;
        }

        /// <summary>
        /// Merges another IBisResult with this one. The error type and primary error message from the merging one
        /// will only overwrite the values in this one if this one doesn't have data for those fields.
        /// </summary>
        /// <param name="otherResult">The result to merge with this one.</param>
        public void MergeWithOtherResult(IBisResult otherResult)
        {
            // Copy the Error type over if this one is set to None.
            ErrorType = ErrorType != BisErrorType.None ? ErrorType : otherResult.ErrorType;

            // If this Primary message is empty, use the message from the new one
            // If this Primary message is not empty, add the message from the new one into the list (if it isn't empty)
            if (string.IsNullOrEmpty(_primaryErrorMessage))
            {
                _primaryErrorMessage = otherResult.ErrorMessage;
            }
            else if (!string.IsNullOrEmpty(otherResult.ErrorMessage))
            {
                this.AddErrorMessage(otherResult.ErrorMessage);
            }

            // Add in the range of error messages
            this.AddErrorMessages(otherResult.GetErrorMessages());

            // Copy the resource to this result if not already being included and the other result
            // wants to include its own.
            _resource = otherResult.GetResource<object>();
        }

        /// <summary>
        /// Gets the collection of error messages on the result.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{string}"/> containing the error messages. If there
        /// aren't any messages, the collection will be empty.</returns>
        public IEnumerable<string> GetErrorMessages()
        {
            return _errorMessages;
        }

        /// <summary>
        /// Gets the resource stored in the result.
        /// </summary>
        /// <typeparam name="T">The type of the resource stored.</typeparam>
        /// <returns>The resource cast as the type.</returns>
        public T GetResource<T>() where T : class
        {
            return _resource as T;
        }

        /// <summary>
        /// Saves the resource to the result.
        /// </summary>
        /// <param name="resource">Resource saved to the result.</param>
        public void SetResource(object resource)
        {
            _resource = resource;
        }
        #endregion
    }
}