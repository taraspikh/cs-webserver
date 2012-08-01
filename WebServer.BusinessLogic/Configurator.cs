// -----------------------------------------------------------------------
// <copyright file="Configurator.cs" company="SoftServe IT Academy">
// TODO: Це тіки заглушка на разі....слід продумати функціонал повністю
// </copyright>
// -----------------------------------------------------------------------

namespace WebServer.BusinessLogic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using System.IO;
    using System.Net;

    public enum ServerConfiguration
    {
        LoadBalancer,
        Redirect,
        Normal,
    }

    /// <summary>
    /// Initialaze starting 
    /// </summary>
    public class Configurator : INotifyPropertyChanged
    {
        private static Configurator _instance;

        [XmlIgnore]
        public static Configurator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Configurator();
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        private int _port;

        /// <summary>
        /// <Gets>number of used port</Gets>
        /// </summary>
        public int Port
        {
            get
            {
                return this._port;
            }
            set
            {
                this._port = value;
                this.OnPropertyChanged("Port");
            }
        }

        /// <summary>
        /// Port number for load balancer to get data about servers' loads
        /// </summary>
        public int LoadBalancerPort { get; set; }

        /// <summary>
        /// <Gets>mode of ConfigurationServer</Gets>
        /// </summary>
        public ServerConfiguration ServConfig { get; set; }

        /// <summary>
        /// <Gets>Max allowed number of users</Gets>
        /// </summary>
        public int MaxUsers { get; set; }

        /// <summary>
        /// <Gets>Relative path to the WWW main folder</Gets>
        /// </summary>
        public string RelativeWwwPath { get; set; }

        /// <summary>
        /// Gets the Host of WebServer
        /// </summary>
        [XmlIgnore]
        public IPAddress Host { get; set; }

        /// <summary>
        /// Name of webserver. Used in Response
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Version of webserver. Used in Response
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Indicated whether to encode session 
        /// </summary>
        public bool IsCookielessSession { get; set; }

        /// <summary>
        /// login constant for admin loging
        /// </summary>
        public string AdminLogin { get; set; }

        /// <summary>
        /// password constant for admin logging
        /// </summary>
        public string AdminPassword { get; set; }

        /// <summary>
        /// Disables or enables logging functionality
        /// </summary>
        public bool EnableLogging { get; set; }

        /// <summary>
        /// Relative or absolute path to the logger folder
        /// </summary>
        public string LoggerPath { get; set; }

        /// <summary>
        /// Path to the Configurator XML file
        /// </summary>
        public string ConfiguratorPath { get; set; }

        /// <summary>
        /// Gets or sets if the server acts like load balancer
        /// </summary>
        public bool IsLoadBalancer { get; set; }

        private ObservableCollection<string> _loadBalancerAddresses;

        /// <summary>
        /// Gets or sets array of other servers for load balancer mode
        /// </summary>
        [XmlArray]
        [XmlArrayItem("Address")]
        public ObservableCollection<string> LoadBalancerAddresses
        {
            get
            {
                return this._loadBalancerAddresses;
            }
            set
            {
                this._loadBalancerAddresses = value;
                this.OnPropertyChanged("LoadBalancerAddresses");
            }
        }

        /// <summary>
        /// Enable or disable compression for response, like Gzip
        /// </summary>
        public bool UseResponseCompression { get; set; }

        private Configurator()
        {
            this.Port = 8080;
            this.ServConfig = ServerConfiguration.Normal;
            this.MaxUsers = 500;
            this.RelativeWwwPath = "www";
            this.ServerName = "MyHttpServer";
            this.Version = "1.0";
            this.IsCookielessSession = false;
            this.EnableLogging = true;
            this.UseResponseCompression = true;

            // administrator configuration
            this.AdminLogin = "Admin";
            this.AdminPassword = "Admin";

            this.LoggerPath = @"conf\logs\";
            this.ConfiguratorPath = @"conf\config.xml";

            // create localhost address by default
            this.Host = IPAddress.Parse("127.0.0.1");
        }

        /// <summary>
        /// :this() executes base constructor and then current one
        /// </summary>
        /// <param name="port">Port of WebServer to listen to</param>
        private Configurator(int port) : this()
        {
            this.Port = port;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
