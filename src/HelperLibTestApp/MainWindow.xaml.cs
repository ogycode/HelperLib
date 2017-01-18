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

            lbSettings.Items.Clear();
            foreach (dynamic item in regSettings)
                lbSettings.Items.Add($"{item.Key} = {item.Value}");
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
            lbSettings.Items.Add($"{tbKey.Text} = {tbValue.Text}");
            tbKey.Text = "";
            tbValue.Text = "";
        }
        private void btnGetClick(object sender, RoutedEventArgs e)
        {
            tbGetResonse.Text = $"Value = {regSettings[tbGetKey.Text]}";
            tbGetResonse.Visibility = Visibility.Visible;
        }
        private void btnGetIntClick(object sender, RoutedEventArgs e)
        {
            int a = regSettings.GetValue<int>(tbGetKeyInt.Text);

            tbGetResonseInt.Text = $"Value = {a.ToString()}";
            tbGetResonseInt.Visibility = Visibility.Visible;
        }
        private void btnGetDoubleClick(object sender, RoutedEventArgs e)
        {
            double a = regSettings.GetValue<double>(tbGetKeyDouble.Text);

            tbGetResonseDouble.Text = $"Value = {a.ToString()}";
            tbGetResonseDouble.Visibility = Visibility.Visible;
        }
        private void btnSetStructClick(object sender, RoutedEventArgs e)
        {
            TestStruct ts = new TestStruct();
            ts.a = TestStruct.getInt(tbSetStruct1.Text);
            ts.b = TestStruct.getInt(tbSetStruct2.Text);
            ts.c = TestStruct.getInt(tbSetStruct3.Text);
            ts.g = TestStruct.getInt(tbSetStruct4.Text);

            regSettings.SetValue("testStruct", ts);

            lbSettings.Items.Clear();
            foreach (dynamic item in regSettings)
                lbSettings.Items.Add($"{item.Key} = {item.Value}");
        }
    }
}
