using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BisAceDIContainer.DIContainer
{
    /// <summary>
    /// Class containing shared math functions.
    /// </summary>
    public static class SharedMath
    {
        /// <summary>
        /// Rounds a price to some number of decimal places (defaulted 2 decimals for now, may change later).
        /// </summary>
        /// <param name="price">The unrounded price.</param>
        /// <param name="digits">The number of fractional digits to round to. Defaulted to 2.</param>
        /// <returns>
        /// The rounded price.
        /// </returns>
        public static double RoundPrice(double price, int digits = 2)
        {
            //Long term we probably should be respecting CultureInfo.NumberFormat.CurrencyDecimalDigits based on some predefined culture chosen by the manufacturer (they ought to tell the system what currency is used by their catalog's pricing logic).
            //However, existing CenterPoint business logic has a hard coded rounding of 2 in a great many places and uses the local machine's culture info in other places.
            //That works for now since we only deal in US and Canadian dollars, and assume the local machine's culture uses a 2 decimal point currency with a dollar sign.
            return System.Math.Round(price, digits);
        }

        /// <summary>
        /// Rounds a price to some number of decimal places (hard coded 2 decimals for now, may change later).
        /// </summary>
        /// <param name="price">The unrounded price.</param>
        /// <param name="digits">The number of fractional digits to round to. Defaulted to 2.</param>
        /// <returns>
        /// The rounded price. Returns null if the input is null.
        /// </returns>
        public static double? RoundPrice(double? price, int digits = 2)
        {
            if (price == null) return null;
            return RoundPrice(price.Value, digits);
        }

        /// <summary>
        /// Rounds a decimal price to some number of decimal places (defaulted 2 decimals for now, may change later).
        /// </summary>
        /// <param name="price">The unrounded price.</param>
        /// <param name="digits">The number of fractional digits to round to. Defaulted to 2.</param>
        /// <returns>
        /// The rounded price.
        /// </returns>
        public static decimal RoundPrice(decimal price, int digits = 2)
        {
            return System.Math.Round(price, digits);
        }

        /// <summary>
        /// Rounds a decimal price to some number of decimal places (hard coded 2 decimals for now, may change later).
        /// </summary>
        /// <param name="price">The unrounded price.</param>
        /// <param name="digits">The number of fractional digits to round to. Defaulted to 2.</param>
        /// <returns>
        /// The rounded price. Returns null if the input is null.
        /// </returns>
        public static decimal? RoundPrice(decimal? price, int digits = 2)
        {
            if (price == null) return null;
            return RoundPrice(price.Value, digits);
        }
    }
}
