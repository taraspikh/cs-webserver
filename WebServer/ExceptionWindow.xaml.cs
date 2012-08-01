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
    /// Interaction logic for ExceptionWindow.xaml
    /// </summary>
    public partial class ExceptionWindow : Window
    {
        public ExceptionWindow()
        {
            InitializeComponent();
        }

        public ExceptionWindow(int oldPort):this()
        {
            textBoxNewPort.Text = oldPort.ToString();
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            Configurator.Instance.Port = Convert.ToInt32(textBoxNewPort.Text);
        }
    }
}
