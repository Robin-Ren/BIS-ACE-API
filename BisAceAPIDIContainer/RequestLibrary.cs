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