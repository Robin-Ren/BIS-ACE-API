namespace BisAceAPIModels.Models.Enums
{
    public enum BisErrorType
    {
        /// <summary>
        /// Indicates there is no error
        /// </summary>
        None = 0,

        /// <summary>
        /// The type of the error is not known.
        /// </summary>
        Other = 1,

        /// <summary>
        /// The input provided to the API is invalid beyond just the model validation that the
        /// <see cref="ValidationModelAttribute"/> performs. This could be that the validation
        /// to perform on a field is too complex for the attribute checking (e.g. it's a value
        /// which points to another record) as well.
        /// </summary>
        InvalidInput = 2,

        /// <summary>
        /// System configuration prevented the API from returning data which is part of a larger
        /// resource which the system does have access to.
        /// For example, if the system is set up to not display the list price in the configurator
        /// and the user calls the pricing API endpoint, the call will return a 400 and in the
        /// response body have this for the error type.
        /// </summary>
        Configuration = 3,

        /// <summary>
        /// The user's security settings prevented the API from returning data which is part of a
        /// larger resource which the user does have access to. For example, if a user doesn't have
        /// security to see the list price during a configuration, the pricing call will return a
        /// 400 and in the response body have this for the error type. If we add the price to part
        /// of the configurator state and the user doesn't have access to see it, a 200 response
        /// will still be sent and the price field will just be blank.
        /// </summary>
        Unauthorised = 4,

        /// <summary>
        /// This indicates that the resource the user was looking for based on the URI was not found.
        /// </summary>
        NotFound = 5
    }
}
