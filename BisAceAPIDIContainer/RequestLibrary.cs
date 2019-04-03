using BisAceAPIBase;
using BisAceAPIModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;

namespace BisAceDIContainer.DIContainer
{
    public static class RequestLibrary
    {
        ///// <summary>
        ///// Gets the call context for an API request from the specified Owin environment.
        ///// </summary>
        ///// <typeparam name="T">The specific type of the call context. This must inherit from <see cref="AWcpAPICallContext"/>.</typeparam>
        ///// <param name="owinEnvironment">The Owin environment containing the call context.</param>
        ///// <returns>The call context in the Owin environment, if found. If not found, null will be returned.</returns>
        //public static T GetCallContext<T>(IDictionary<string, object> owinEnvironment)
        //    where T : ABisAPICallContext
        //{
        //    T callContext = null;

        //    if (owinEnvironment != null)
        //    {
        //        object context;
        //        if (owinEnvironment.TryGetValue(BisConstants.OWIN_CALL_CONTEXT, out context))
        //        {
        //            callContext = context as T;
        //        }
        //    }

        //    // The call context either wasn't present at all or didn't cast appropriately
        //    if (callContext == null)
        //    {
        //        //Treat this as a 500 internal server error rather than a 400 bad request.
        //        //It's a setup issue or bug rather than a problem caused by user/caller.
        //        throw new WcpInvalidCallContextException(APIMessages.NoCallContextWasFoundForThisEnvironment);
        //    }

        //    return callContext;
        //}

        ///// <summary>
        ///// Gets the call context for an API request from the specified request message's Owin environment.
        ///// </summary>
        ///// <typeparam name="T">The specific type of the call context. This must inherit from <see cref="AWcpAPICallContext"/>.</typeparam>
        ///// <param name="request">The request being processed.</param>
        ///// <returns>The call context for the request, if found. If not found, an ApplicationException will be thrown.</returns>
        ///// <exception cref="ApplicationException">Thrown if a call context isn't available for the request.</exception>
        //public static T GetCallContext<T>(System.Net.Http.HttpRequestMessage request)
        //    where T : ABisAPICallContext
        //{
        //    return GetCallContext<T>(request.GetOwinEnvironment());
        //}

        ///// <summary>
        ///// Populates session identity info from the specified request context.
        ///// </summary>
        ///// <param name="RequestContext">request context</param>
        ///// <param name="sessionIdentityInfo">session identity info</param>
        ///// <param name="cpUserIdOptional">if set to <c>true</c> the user identifier is optional.</param>
        ///// <param name="cpSystemIdOptional">if set to <c>true</c> the system identifier is optional.</param>
        //public static void PopulateUserIdentityClaimInfo(System.Web.Http.Controllers.HttpRequestContext RequestContext, WcpSessionIdentityInfo sessionIdentityInfo, bool cpUserIdOptional = false, bool cpSystemIdOptional = false)
        //{
        //    ClaimsPrincipal claimsPrincipal = RequestContext.Principal as ClaimsPrincipal;

        //    // Check the current role.
        //    sessionIdentityInfo.IsManufacturerAdmin = claimsPrincipal.IsInRole("ManufacturerAdmin");
        //    Guid userId = GetCPUserIdFromClaim(claimsPrincipal, !cpUserIdOptional);
        //    bool isInspire = GetIsInspireFromClaim(claimsPrincipal);
        //    sessionIdentityInfo.WebConfiguratorContext = isInspire ? WcpWebConfiguratorContext.Inspire : WcpWebConfiguratorContext.WebCenterPoint;
        //    sessionIdentityInfo.CPUserID = userId;
        //    sessionIdentityInfo.WebCPUserName = RequestContext.Principal.Identity.GetUserName();
        //    sessionIdentityInfo.SystemID = GetCPSystemIdFromClaim(claimsPrincipal, !cpSystemIdOptional);
        //    if (isInspire)
        //    {
        //        sessionIdentityInfo.InspireTenantID = GetInspireTenantIdFromClaim(claimsPrincipal); //CSR 1/18 WEB-1214 add Inspire tenant concept
        //        sessionIdentityInfo.InspireUserName = GetInspireUserNameFromClaim(claimsPrincipal); //CSR 12/17 WEB-1214 switch from user id to user name
        //        sessionIdentityInfo.AnonymousSessionID = GetAnonymousSessionIDFromClaim(claimsPrincipal);
        //    }
        //}

        ///// <summary>
        ///// Builds the Web CenterPoint request context from the http request.
        ///// </summary>
        ///// <param name="request">The http request.</param>
        ///// <param name="controllerName">The name of the controller.</param>
        ///// <param name="actionName">The name of the action.</param>
        ///// <returns>
        ///// The Web CenterPoint request context.
        ///// </returns>
        //private static WcpRequestContext BuildWcpRequestContext(HttpRequestMessage request, string controllerName, string actionName)
        //{
        //    var requestcontext = new WcpRequestContext();

        //    // Check for the bearer token
        //    if (request.Headers?.Authorization != null && string.Equals(request.Headers.Authorization.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
        //    {
        //        requestcontext.Bearer = request.Headers.Authorization.Parameter;
        //    }

        //    // Check for the localization value.
        //    var localizationValue = request.GetHeaderFieldValue("Localization");
        //    if (!string.IsNullOrWhiteSpace(localizationValue))
        //    {
        //        requestcontext.Localization = request.GetHeaderFieldValue("Localization");
        //    }

        //    // Update the request endpoint information.
        //    requestcontext.SetEndpointInformation(controllerName?.ToString(), actionName?.ToString());

        //    return requestcontext;
        //}

        ///// <summary>
        ///// Gets the session identity information from the request.
        ///// </summary>
        ///// <param name="request">The request.</param>
        ///// <param name="controllerName">Name of the controller.</param>
        ///// <param name="actionName">Name of the action.</param>
        ///// <param name="cpUserIdOptional">if set to <c>true</c> the user identifier is optional.</param>
        ///// <param name="cpSystemIdOptional">if set to <c>true</c> the system identifier is optional.</param>
        ///// <returns>
        ///// The session identity information.
        ///// </returns>
        //public static WcpSessionIdentityInfo GetSessionInformationFromRequest(HttpRequestMessage request, string controllerName, string actionName, bool cpUserIdOptional = false, bool cpSystemIdOptional = false)
        //{
        //    var info = new WcpSessionIdentityInfo();

        //    // Get the identity information from the request.
        //    PopulateUserIdentityClaimInfo(request.GetRequestContext(), info, cpUserIdOptional, cpSystemIdOptional);

        //    // Set the request context information.
        //    info.RequestContext = BuildWcpRequestContext(request, controllerName, actionName);

        //    return info;
        //}

        ///// <summary>
        ///// Gets the session identity information from the request.
        ///// </summary>
        ///// <param name="request">The request to process.</param>
        ///// <param name="createNewIfNotCached">if set to <c>true</c> and information is not already cached in the request, then a new session information object will be created for use.</param>
        ///// <param name="cpUserIdOptional">Determines if the user id information in the claim token is optional, when provided. If null, the information cached in the request will be used.</param>
        ///// <param name="cpSystemIdOptional">Determines if the system id information in the claim token is optional, when provided. If null, the information cached in the request will be used.</param>
        ///// <returns>
        ///// The session identity information.
        ///// </returns>
        ///// <exception cref="WcpInvalidClaimException">Error thrown when the session information is not already cached and not allowed to be recreated.</exception>
        //public static WcpSessionIdentityInfo GetSessionInformationFromRequest(HttpRequestMessage request, bool createNewIfNotCached = false, bool? cpUserIdOptional = null, bool? cpSystemIdOptional = null)
        //{
        //    var props = request.Properties;

        //    if (props.TryGetValue(WcpSessionIdentityInfo.SESSION_INFORMATION_PROPERTY, out object propertyValue))
        //        return (WcpSessionIdentityInfo)propertyValue;
        //    else if (!createNewIfNotCached)
        //        throw new WcpInvalidClaimException(APIMessages.NoSessionIdentityAvailable);

        //    // Get the additional information from the session properties and update the request endpoint information.
        //    props.TryGetValue(WcpSessionIdentityInfo.SESSION_CONTROLLER_NAME_PROPERTY, out object controllerName);
        //    props.TryGetValue(WcpSessionIdentityInfo.SESSION_ACTION_NAME_PROPERTY, out object actionName);

        //    // Check to see if the user is optional.
        //    if (props.TryGetValue(WcpSessionIdentityInfo.SESSION_USER_OPTIONAL, out propertyValue) && propertyValue is bool)
        //        cpUserIdOptional = (bool)propertyValue;

        //    // Check to see if the system is optional.
        //    if (props.TryGetValue(WcpSessionIdentityInfo.SESSION_SYSTEM_OPTIONAL, out propertyValue) && propertyValue is bool)
        //        cpSystemIdOptional = (bool)propertyValue;

        //    return GetSessionInformationFromRequest(request, controllerName?.ToString(), actionName?.ToString(), cpUserIdOptional.GetValueOrDefault(false), cpSystemIdOptional.GetValueOrDefault(false));
        //}

        /// <summary>
        /// gets the content as a string, decompressing gzip/deflate if appropriate
        /// </summary>
        /// <param name="message">message</param>
        /// <returns>result as string</returns>
        public static string GetContentAsString(HttpRequestMessage message)
        {
            return GetContentAsString(message.Content);
        }

        /// <summary>
        /// gets the content as a string, decompressing gzip/deflate if appropriate
        /// </summary>
        /// <param name="message">message</param>
        /// <returns>result as string</returns>
        public static string GetContentAsString(HttpResponseMessage message)
        {
            return GetContentAsString(message.Content);
        }


        /// <summary>
        /// gets the content as a stream, decompressing gzip/deflate if appropriate
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>result as string</returns>
        private static string GetContentAsString(HttpContent content)
        {
            bool IsGZip = false;
            bool IsDeflate = false;
            foreach (KeyValuePair<string, IEnumerable<string>> header in content.Headers)
            {
                string key = header.Key;
                if (key.Equals("Content-Encoding", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (string nextHeaderValue in header.Value)
                    {
                        string nextHeaderValueLower = nextHeaderValue.ToLower();
                        if (nextHeaderValueLower.Contains("gzip"))
                            IsGZip = true;
                        else if (nextHeaderValueLower.Contains("deflate"))
                            IsDeflate = true;
                    }
                    break;
                }
            }

            //reset the stream position in case it is already read
            Stream inputStream = content.ReadAsStreamAsync().Result;
            inputStream.Position = 0;

            string ret;
            if (IsGZip || IsDeflate) //compressed
            {
                Stream compressionStream = null;
                if (IsGZip)
                    compressionStream = new GZipStream(inputStream, CompressionMode.Decompress);
                else if (IsDeflate)
                    compressionStream = new DeflateStream(inputStream, CompressionMode.Decompress);
                StreamReader reader = new StreamReader(compressionStream);
                ret = reader.ReadToEnd();
            }
            else //not compressed
            {
                ret = content.ReadAsStringAsync().Result;
            }
            return ret;
        }
    }
}