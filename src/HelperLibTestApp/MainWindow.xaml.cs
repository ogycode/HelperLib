using HelperLib.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HelperLibTestApp
{
    public partial class MainWindow : Window
    {
        RegSettings regSettings;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnInitClick(object sender, RoutedEventArgs e)
        {
            regSettings = new RegSettings("Test App");
            btnDispose.IsEnabled = true;
            btnInit.IsEnabled = false;
        }
        private void btnDisposeClick(object sender, RoutedEventArgs e)
        {
            regSettings.Dispose();
            btnDispose.IsEnabled = false;
            btnInit.IsEnabled = true;
        }
        private void btnAddingClick(object sender, RoutedEventArgs e)
        {
            regSettings[tbKey.Text] = tbValue.Text;
        }
        private void btnGetClick(object sender, RoutedEventArgs e)
        {
            tbGetResonse.Text = $"Value = {regSettings[tbGetKey.Text].ToString()}";
            tbGetResonse.Visibility = Visibility.Visible;
        }
        private void btnLoadSettingClick(object sender, RoutedEventArgs e)
        {
            lbSettings.Items.Clear();
            foreach (var item in regSettings)
            {
                lbSettings.Items.Add($"{((KeyValuePair<string, object>)item).Key} = {((KeyValuePair<string, object>)item).Value}");
            }
        }
    }
}
