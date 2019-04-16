using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BisAceAPIModels
{
    public static class BisConstants
    {
        #region Configuration Setup
        public static readonly string CONFIG_SYS_CONFIG_PREFIX = "Config:";

        public static readonly string CONFIG_SYS_FOLDER_CONFIG_SETTING = "SystemConfigFilesFolder";

        public static readonly string LOGGING_CONFIGURATION_FILENAME = "logging.settings";

        public static readonly string CHECK_SPECIFIED_QUOTE_LAST_SERVER_UPDATE_SETTING = "CheckKnownQuoteLastServerUpdate";

        public static readonly string API_CONFIG_PC360_BASE_URL_CLAIM = "pc360_base_url";
        #endregion

        #region Configuration API Constants
        public static readonly string API_CONFIG_CONFIG_SESSION_BASE = "ConfiguratorSession";

        public static readonly string API_CONFIG_NO_SESSION_ERROR_MESSAGE = "YourSessionHasExpiredPleaseRefreshYourBrowserOrCreateANewLineItem";

        public static readonly string API_NO_CALL_CONTEXT_ERROR_MESSAGE = "No Call Context Was Found For This Environment";

        /// <summary>
        /// Defines the subfolder name that dynamically generated images will be saved to, within the brand subfolder within the image render directory.
        /// </summary>
        public static readonly string API_CONFIG_DYNAMIC_IMAGES_DIRECTORY_NAME = "_dynamic";

        /// <summary>
        /// Subfolder within API_CONFIG_DYNAMIC_IMAGES_DIRECTORY_NAME for the windowset icons.
        /// </summary>
        public static readonly string API_CONFIG_WINDOWSET_ICON_IMAGES_DIRECTORY_NAME = "_wsimages";

        /// <summary>
        /// Defines the subfolder name that built-in static answer images (from our application rather than the catalog) will be saved to, within the image render directory.
        /// </summary>
        public static readonly string API_CONFIG_STATIC_ANSWER_IMAGES_DIRECTORY_NAME = "staticanswers";

        public static readonly string API_NULL_MODEL_ERROR_MESSAGE = "The provided model cannot be null";

        /// <summary>
        /// Defines the subfolder name that built-in static part images will be saved to, within the brand subfolder within the image render directory.
        /// </summary>
        public static readonly string API_CONFIG_PART_IMAGES_DIRECTORY_NAME = "_partsimages";
        #endregion

        #region APIMessages
        public static readonly string BIS_SERVER_NAME = "BisServerName";
        public static readonly string RESPONSE_REQUEST_BODY_MUST_BE_PROVIDED = "Request body must be provided!";
        public static readonly string RESPONSE_CARD_NUMBER_MUST_BE_PROVIDED = "Card Number must be provided!";
        public static readonly string RESPONSE_BIS_API_CALL_FAILED = "Failed to call BIS API.";
        public static readonly string RESPONSE_LOAD_OR_SAVE_PERSON_FAILED = "Failed to load or save PERSON.";
        public static readonly string RESPONSE_UNABLE_TO_CREATE_OR_UPDATE_CARD = "Unable to create or update card.";
        public static readonly string RESPONSE_LOGIN_ERROR = "Failed to login.";
        public static readonly string RESPONSE_CARD_NOT_FOUND = "Card can not be found.";
        public static readonly string RESPONSE_PERSON_NOT_FOUND = "Person can not be found.";
        #endregion
    }
}
