// -----------------------------------------------------------------------
// <copyright file="ConfiguratorSerializer.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------


namespace WebServer.BusinessLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Xml.Serialization;

    public static class ConfiguratorSerializer
    {
        private static readonly string PathToConfigFile = Configurator.Instance.ConfiguratorPath;

        // Create a new instance of the test class
        //Configurator configurator = new Configurator();

        /// <summary>
        /// Write Configuration To XML File
        /// </summary>
        public static bool Save()
        {
            try
            {
                // Create a new XmlSerializer instance with the type of the test class
                XmlSerializer xmlserializer = new XmlSerializer(typeof(Configurator));

                // Create a new file stream to write the serialized object to a file
                using (TextWriter writer = new StreamWriter(PathToConfigFile))
                {
                    xmlserializer.Serialize(writer, Configurator.Instance);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Serialize Configurator.Instance into basic config file if it doesn't exist
        /// </summary>
        public static void Initialize()
        {
            if (!File.Exists(PathToConfigFile))
            {
                Save();
            }
        }

        /// <summary>
        /// Read Settings
        /// </summary>
        public static void Load()
        {

            XmlSerializer xmlserializer = new XmlSerializer(typeof(Configurator));

            if (File.Exists(PathToConfigFile))
            {
                // Create a new file stream for reading the XML file
                using (FileStream filestream = new FileStream(PathToConfigFile, FileMode.Open))
                {
                    // Load the object saved above by using the Deserialize function
                    Configurator.Instance = (Configurator) xmlserializer.Deserialize(filestream);
                }
            }
        }
    }
}
