﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BisAceAPI
{
    public static class ConfigurationHelper
    {
        internal static string SERVER_NAME = GetConfigString(Models.ConstantStrings.BIS_SERVER_NAME, true);

        /// <summary>
        /// Returns the specified string value from the application .config file
        /// </summary>
        internal static string GetConfigString(string key, bool isRequired)
        {

            string s = System.Convert.ToString(ConfigurationManager.AppSettings.Get(key));
            if (s == null)
            {
                if (isRequired)
                {
                    throw new ConfigurationErrorsException("key <" + key + "> is missing from .config file");
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return s;
            }
        }
    }
}