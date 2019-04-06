using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace BisAceAPI.WebCore
{
    /// <summary>
    /// Authorization manager
    /// </summary>
    public class AuthorizationManager : ClaimsAuthorizationManager
    {
        /// <summary>
        /// Check access for current authorization context
        /// </summary>
        /// <param name="context">Current authorization context</param>
        /// <returns>True if access is allowed; false otherwise.</returns>
        public override bool CheckAccess(AuthorizationContext context)
        {
            var user = context.Principal;
            if (!(user.Identity is ClaimsIdentity identity))
            {
                return false;
            }

            var usernameClaim = identity.Claims.SingleOrDefault(item => item.Type == System.IdentityModel.Claims.ClaimTypes.Upn);
            if (usernameClaim == null)
            {
                return false;
            }

            SecurityToken token = (user as ClaimsPrincipal).Identities.FirstOrDefault().BootstrapContext as SecurityToken;

            var Resource = context.Resource.FirstOrDefault().Value; // Resource name
            var Operation = context.Action.FirstOrDefault().Value; // Operation name
            var Principal = context.Principal; // Current Logged in user claims principal
            switch (Resource)
            {
                case "Home":
                    // Here you can check the user access permission related logic.
                    if (Operation.Equals("View"))
                    {

                    }
                    else
                        return false;
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}