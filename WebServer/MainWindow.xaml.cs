using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using WebServer.BusinessLogic;


namespace WebServer
{
    using System.Threading;
    using System.Windows.Threading;

    using Application = System.Windows.Application;
    using Binding = System.Windows.Data.Binding;
    using MessageBox = System.Windows.MessageBox;
    using TextBox = System.Windows.Controls.TextBox;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon _notifyIcon;

        private bool _shouldClose = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            //Application.Current.DispatcherUnhandledException += (sender, args) => MessageBox.Show("Error");
            //AppDomain.CurrentDomain.UnhandledException += (o, args) => OnCurrentDomainOnUnhandledException(o, args);
            this.CreateNotifyIcon();
            ConfiguratorSerializer.Initialize();
            ConfiguratorSerializer.Load();

            InitializeComponent();

            switch (Configurator.Instance.ServConfig)
            {
                    case ServerConfiguration.Normal:
                    radioButtonNormal.IsChecked = true;
                    break;

                    case ServerConfiguration.Redirect:
                    radioButtonRedirect.IsChecked = true;
                    break;

                    case ServerConfiguration.LoadBalancer:
                    radioButtonLoadBalancer.IsChecked = true;
                    break;
            }

            Binding activeConnectionsBinding = new Binding("ActiveConnections");
            activeConnectionsBinding.Source = Communicator.Instance;
            activeConnectionsBinding.Converter = new IntStringFormatConverter();
            labelConnections.SetBinding(ContentProperty, activeConnectionsBinding);

            Binding portBinding = new Binding("Port");
            portBinding.Source = Configurator.Instance;
            portBinding.Converter = new IntStringConverter();
            textBoxPort.SetBinding(TextBox.TextProperty, portBinding);

        }

        private void OnCurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Communicator.Instance.StopListening();
            Thread.Sleep(1000);
            MessageBox.Show(e.ExceptionObject.ToString());
            
            ExceptionWindow wnd = new ExceptionWindow(Configurator.Instance.Port);
            wnd.ShowDialog();
        }

        static void Application_ThreadException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Communicator.Instance.StopListening();
            Thread.Sleep(1000);
            MessageBox.Show(e.Exception.Message, "Unhandled Thread Exception");
            // here you can log the exception ...
        }

        /// <summary>
        /// Creates notify icon.
        /// </summary>
        private void CreateNotifyIcon()
        {
            try
            {
                this._notifyIcon = new NotifyIcon();
                this._notifyIcon.Icon = Properties.Resources.EnabledServer;
                this._notifyIcon.BalloonTipTitle = "Text";
                this._notifyIcon.BalloonTipText = "Text";
                this._notifyIcon.Visible = true;
                this._notifyIcon.MouseDoubleClick += this.NotifyIcon_MouseDoubleClick;

                this.AddContextMenuToNotifyIcon();
            }
            catch(Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Adds context menu to notify icon.
        /// </summary>
        private void AddContextMenuToNotifyIcon()
        {
            System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();

            System.Windows.Forms.MenuItem startItem = new System.Windows.Forms.MenuItem();
            startItem.Text = "Start";
            startItem.Enabled = false;
            contextMenu.MenuItems.Add(startItem);

            System.Windows.Forms.MenuItem stopItem = new System.Windows.Forms.MenuItem();
            stopItem.Text = "Stop";
            contextMenu.MenuItems.Add(stopItem);

            System.Windows.Forms.MenuItem exitItem = new System.Windows.Forms.MenuItem();
            exitItem.Text = "Exit";
            contextMenu.MenuItems.Add(exitItem);

            startItem.Click += delegate
            {
                startItem.Enabled = false;
                _notifyIcon.ContextMenu.MenuItems[1].Enabled = true;

                _myHttpServer.Start();
                _notifyIcon.Icon = Properties.Resources.EnabledServer;
            };

            stopItem.Click += delegate
            {
                stopItem.Enabled = false;
                _notifyIcon.ContextMenu.MenuItems[0].Enabled = true;

                _myHttpServer.Stop();
                _notifyIcon.Icon = Properties.Resources.DisabledServer;
            };

            exitItem.Click += delegate
            {
                this._shouldClose = true;
                this.Close();
            };

            this._notifyIcon.ContextMenu = contextMenu;
        }

        void NotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.Focus();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                _notifyIcon.Visible = true;
            }

            else if (this.WindowState == WindowState.Normal)
            {
                this.ShowInTaskbar = true;
            }
        }

        private MyHttpServer _myHttpServer;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //create new MyHttpServer
            _myHttpServer = new MyHttpServer();

            //start webserver
            _myHttpServer.Start();            

            textBoxPort.Text = Configurator.Instance.Port.ToString();
                textBoxMaxUsers.Text = Configurator.Instance.MaxUsers.ToString();
                textBoxRootDirectory.Text = Configurator.Instance.RelativeWwwPath;
            
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!_shouldClose)
            {
                e.Cancel = true;
                this.WindowState = WindowState.Minimized;
            }
            else
            {
                _myHttpServer.Stop();
                _notifyIcon.Visible = false;

                ConfiguratorSerializer.Save();
            }
        }

        private void buttonApply_Click(object sender, RoutedEventArgs e)
        {
            Configurator.Instance.Port = Convert.ToInt32(textBoxPort.Text);
            Configurator.Instance.MaxUsers = Convert.ToInt32(textBoxMaxUsers.Text);
            Configurator.Instance.RelativeWwwPath = textBoxRootDirectory.Text;

            if (radioButtonNormal.IsChecked == true)
            {
                Configurator.Instance.ServConfig = ServerConfiguration.Normal;
            }
            else if (radioButtonRedirect.IsChecked == true)
            {
                Configurator.Instance.ServConfig = ServerConfiguration.Redirect;
            }
            else if (radioButtonLoadBalancer.IsChecked == true)
            {
                Configurator.Instance.ServConfig = ServerConfiguration.LoadBalancer;
            }

            Configurator.Instance.IsLoadBalancer = (bool)checkBoxLoadBalancer.IsChecked;

            // save data to xml file
            ConfiguratorSerializer.Save();

            _myHttpServer.Restart();
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select a root folder of your site.";
            dialog.ShowNewFolderButton = false;
            dialog.ShowDialog();
            textBoxRootDirectory.Text = dialog.SelectedPath;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            
            LoadBalancerSettingsWindow wnd = new LoadBalancerSettingsWindow();
            wnd.ShowDialog();
        }

        private void radioButtonLoadBalancer_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxLoadBalancer.IsChecked = true;
        }
    }
}
