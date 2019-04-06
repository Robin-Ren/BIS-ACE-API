using System;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace BisAceAPIModels.Options
{
    /// <summary>
    /// An interface containing an options class.
    /// </summary>
    /// <typeparam name="T">The type of the options class contained within.</typeparam>
    public interface IBisOptions<T> where T : class, new()
    {
        /// <summary>
        /// The id of the system the options are for
        /// </summary>
        int SystemId { get; set; }

        /// <summary>
        /// The options object.
        /// </summary>
        T Value { get; }
    }

    /// <summary>
    /// Implementation of the IWcpOptions interface
    /// </summary>
    /// <typeparam name="T">The type of the options class</typeparam>
    public class BisOptions<T> : IBisOptions<T> where T : class, new()
    {
        private readonly T _value;

        /// <summary>
        /// The options object.
        /// </summary>
        public T Value
        {
            get { return _value; }
        }

        /// <summary>
        /// The id of the system the options are for
        /// </summary>
        public int SystemId { get; set; }

        /// <summary>
        /// Constructor. Sets the options within.
        /// </summary>
        /// <param name="value">Object containing the set options.</param>
        public BisOptions(T value)
        {
            _value = value;
        }

        /// <summary>
        /// Creates a new IWcpOptions object from the provided xml.
        /// </summary>
        /// <param name="xml">The xml to deserialize to the options object.</param>
        /// <param name="systemId">The id of the system the options are for.</param>
        /// <param name="afterCreate">Optional. Method taking in the new options to perform post-processing.</param>
        /// <returns>An IWcpOptions object with the options set.</returns>
        public static IBisOptions<T> ConfigureFromXml(string xml, int systemId, Action<T> afterCreate = null)
        {
            T options = new T();

            // If we have xml, attempt to deserialize it. If it doesn't work,
            // let an exception be thrown. We don't want to have an options
            // class set with data that doesn't match what the customer wants.
            if (!string.IsNullOrEmpty(xml))
            {
                XmlSerializer optionsSerializer = new XmlSerializer(typeof(T));

                MemoryStream ms = new MemoryStream();
                StreamWriter sw = new StreamWriter(ms);
                sw.Write(xml);
                sw.Flush();
                ms.Position = 0;

                options = (T)optionsSerializer.Deserialize(ms);

                afterCreate?.Invoke(options);
            }

            return new BisOptions<T>(options) { SystemId = systemId };
        }

        /// <summary>
        /// Given a WcpConfigurationManager and a section name, creates an IWcpOptions object for the type and specified system id
        /// </summary>
        /// <param name="config">Configuration manager holding all of the config settings.</param>
        /// <param name="sectionName">The name of the section in the manager containing the settings for the needed options class.</param>
        /// <param name="systemId">The id of the system the resulting options are for.</param>
        /// <param name="afterCreate">Optional. Method taking in the new options to perform post-processing.</param>
        /// <returns>An IWcpOptions class containing the options for the system.</returns>
        public static IBisOptions<T> ConfigureFromConfiguration(BisConfigurationManager config, string sectionName, int systemId, Action<T> afterCreate = null)
        {
            // We need to build the type as an element. Start by getting the name
            string typeName = typeof(T).ToString();

            // Then pull off the namespace
            typeName = typeName.Substring(typeName.LastIndexOf(".") + 1);

            XElement optionsEl = new XElement(typeName);

            XElement configEl = config.GetXmlSection(sectionName);

            if (configEl != null)
            {
                //Only add top level elements to avoid adding child elements to inappropriate places in the document
                optionsEl.Add(configEl.Elements());
                //optionsEl.Add(configEl.Descendants());
            }

            string xml = optionsEl.ToString();

            return ConfigureFromXml(xml, systemId, afterCreate);
        }
    }
}
