using System;
using System.Reflection;
namespace BisAceAPILogging.ExceptionFormatters
{
    /// <summary>
    /// Formats exceptions for logging.
    /// </summary>
    /// <remarks>
    /// Only standard .net exceptions that skip important properties as part of ToString() should be included here
    /// Do not add any custom exceptions here - those exceptions should handle ToString() correctly
    /// </remarks>
    public static class ExceptionFormatter
    {
        /// <summary>
        /// Determines whether a explicit handling is available for the exception
        /// </summary>
        /// <param name="exception">The exception.</param>
        public static bool HasExplicitFormatter(Exception exception)
        {
            if(exception is ReflectionTypeLoadException)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Formats the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public static string Format(Exception exception)
        {
            if (exception is ReflectionTypeLoadException)
            {
                return Format((ReflectionTypeLoadException)exception);
            }
            return exception.ToString();
        }

        /// <summary>
        /// Formats the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public static string Format(ReflectionTypeLoadException exception)
        {
            var baseData = exception.ToString();
            var subData = string.Empty;
            if(exception.LoaderExceptions != null)
            {
                subData = string.Join<Exception>(System.Environment.NewLine, exception.LoaderExceptions);
            }
            return string.Join(System.Environment.NewLine, baseData, subData);
        }
    }
}
