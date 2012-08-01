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
using System.Windows.Shapes;
using WebServer.BusinessLogic;

namespace WebServer
{
    /// <summary>
    /// Interaction logic for LoadBalancerSettingsWindow.xaml
    /// </summary>
    public partial class LoadBalancerSettingsWindow : Window
    {
        public LoadBalancerSettingsWindow()
        {
            InitializeComponent();
            this.listBoxAddresses.ItemsSource = Configurator.Instance.LoadBalancerAddresses;
            textBoxLoadBalancerPort.Text = Configurator.Instance.LoadBalancerPort.ToString();
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            this.AddAddress(this.textBoxNewAddress.Text);
            textBoxNewAddress.Clear();
        }

        private void AddAddress(string address)
        {
            address = address.Trim();
            if (!address.StartsWith("http://"))
            {
                address = address.Insert(0, "http://");
            }
            if (!Configurator.Instance.LoadBalancerAddresses.Contains(address))
            {
            Configurator.Instance.LoadBalancerAddresses.Add(address);
            }
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxAddresses.SelectedIndex >= 0)
            {
                DeleteAddress();
            }
        }

        private void textBoxNewAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                this.AddAddress(textBoxNewAddress.Text);
                textBoxNewAddress.Clear();
            }
        }

        private void listBoxAddresses_KeyDown(object sender, KeyEventArgs e)
        {
            if (listBoxAddresses.SelectedItem != null && e.Key == Key.Delete)
            {
                DeleteAddress();
            }
        }

        private void DeleteAddress()
        {
                int tempIndex = listBoxAddresses.SelectedIndex;
                Configurator.Instance.LoadBalancerAddresses.Remove(this.listBoxAddresses.SelectedItem.ToString());
                listBoxAddresses.SelectedIndex = tempIndex == 0 ? tempIndex : tempIndex - 1;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            Configurator.Instance.LoadBalancerPort = Convert.ToInt32(textBoxLoadBalancerPort.Text);
            this.Close();
        }
    }
}
