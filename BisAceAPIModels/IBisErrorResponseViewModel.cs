using System.Collections.Generic;

namespace BisAceAPIModels.Models
{
    public interface IBisErrorResponseViewModel
    {
        /// <summary>
        /// Gets a flag indicating whether the result is valid or not.
        /// </summary>
        bool IsSucceeded { get; }

        /// <summary>
        /// A code indicating the type of error which occurred.
        /// </summary>
        /// <remarks>This is in lower case to match up with the object format returned from the Auth
        /// API. That format is set by Microsoft so we can't change that.</remarks>
        string Error { get; set; }

        /// <summary>
        /// The error message to display.
        /// </summary>
        /// <remarks>This is in lower case to match up with the object format returned from the Auth
        /// API. That format is set by Microsoft so we can't change that.</remarks>
        string ErrorDescription { get; set; }

        /// <summary>
        /// The resource object to return. This can be used for returning a complicated input model
        /// which has specific validation messages in it.
        /// Only used when not in an OData context.
        /// </summary>
        /// <value>
        /// The model data to return with the result.
        /// </value>
        /// <remarks>
        /// This property is ignored when in an OData context because OData will not use property with object type.
        /// </remarks>
        object Model { get; set; }

        /// <summary>
        /// A list of additional properties to return when in an OData context.
        /// The intention is for the Model property to be stored here when in an OData context
        /// because OData can not use the Model property directly.
        /// This property is excluded from the Json used when not in an OData context.
        /// </summary>
        /// <value>
        /// Additional properties to include on the model when using OData context.
        /// </value>
        [Newtonsoft.Json.JsonIgnore]
        IDictionary<string, object> DynamicProperties { get; }
    }
}
