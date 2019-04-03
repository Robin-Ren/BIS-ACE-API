using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BisAceAPIModels.Options
{
    /// <summary>
    /// Class to handle loading configuration files and locating
    /// options sections within them.
    /// </summary>
    public class BisConfigurationManager
    {
        #region Private Members
        /// <summary>
        /// List of files to load
        /// </summary>
        private List<string> _filesToLoad;

        /// <summary>
        /// The loaded XML to search through for configuration sections.
        /// </summary>
        private XDocument _loadedXml;
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public BisConfigurationManager()
        {
            _filesToLoad = new List<string>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a file to be loaded to support configuration management.
        /// </summary>
        /// <param name="fileName">The full path and name of the file to load.</param>
        /// <param name="optional">If true, the file does not need to exist. If false, an
        /// exception will be thrown if the file doesn't exist.</param>
        /// <exception cref="ArgumentException">The file name does not exist and is required.</exception>
        public void AddFile(string fileName, bool optional)
        {
            // If we don't already have the file loaded
            if (!_filesToLoad.Contains(fileName))
            {
                // If the file exists, add it to the list
                // If the file does not exist and is not optional, throw an exception
                if (File.Exists(fileName))
                {
                    _filesToLoad.Add(fileName);
                }
                else if (!optional)
                {
                    throw new ArgumentException("File Not Found And Is Required");
                }
            }
        }

        /// <summary>
        /// Builds the configuration manager to be ready to support configurations
        /// </summary>
        public void Build()
        {
            _loadedXml = new XDocument();
            _loadedXml.Add(new XElement("OptionsRoot"));

            foreach (string fileName in _filesToLoad)
            {
                try
                {
                    XDocument xDoc = XDocument.Load(fileName);
                    _loadedXml.Root.Add(xDoc.Root);
                }
                catch (System.Xml.XmlException ex)
                {
                    string sLocalizedMessage = string.Format("Option File: {0}; either contains no data or badly formatted xml. You may fix the xml or remove the file from environment's setting file(s).", fileName); 
                    throw new System.Xml.XmlException(sLocalizedMessage, ex);
                }
            }
        }

        /// <summary>
        /// Find the section with the specified from the loaded Xml documents
        /// </summary>
        /// <param name="sectionName">Name of the section to get</param>
        /// <returns>The XElement of the section if one exists. If one does not exist, null will be returned.</returns>
        /// <exception cref="Exception">This will throw an exception if it finds more than one section with the given name.</exception>
        public XElement GetXmlSection(string sectionName)
        {
            return _loadedXml.Descendants(sectionName).SingleOrDefault();
        }
        #endregion
    }
}
