using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BisAceAPIBase
{
    /// <summary>
    /// Interface for application api configuration
    /// </summary>
    /// <remarks>
    /// Please update the API README.md if/when additional items are added here
    /// </remarks>
    public interface IBisApplicationConfig
    {
        /// <summary>
        /// The connection string to the BIS ACE database
        /// </summary>
        string BISConnectionString { get; }
    }
}
