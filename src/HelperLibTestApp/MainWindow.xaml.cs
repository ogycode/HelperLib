using Verloka.HelperLib.Settings;
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
using Verloka.HelperLib.Update;
using Microsoft.Win32;
using System.IO;

namespace HelperLibTestApp
{
    public partial class MainWindow : Window
    {
        RegSettings regSettings;
        UpdateClient update;

        public MainWindow()
        {
            InitializeComponent();
        }
        //Settings
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
            lbSettings.Items.Clear();
        }
        private void btnAddingClick(object sender, RoutedEventArgs e)
        {
            regSettings[tbKey.Text] = tbValue.Text;
            tbKey.Text = "";
            tbValue.Text = "";

            lbSettings.Items.Clear();
            foreach (dynamic item in regSettings)
                lbSettings.Items.Add($"{item.Key} = {item.Value}");
        }
        private void btnGetClick(object sender, RoutedEventArgs e)
        {
            tbGetResonse.Text = $"Value = {regSettings.GetValue<object>(tbGetKey.Text)}";
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

            regSettings.SetValue(tbSetStructName.Text, ts);

            lbSettings.Items.Clear();
            foreach (dynamic item in regSettings)
                lbSettings.Items.Add($"{item.Key} = {item.Value}");

            tbSetStructName.Text = "";
            tbSetStruct1.Text = "";
            tbSetStruct2.Text = "";
            tbSetStruct3.Text = "";
            tbSetStruct4.Text = "";
        }
        private void btnDeleteClick(object sender, RoutedEventArgs e)
        {
            regSettings.DeleteValue(tbDeleteValue.Text);
            tbDeleteValue.Text = "";

            lbSettings.Items.Clear();
            foreach (dynamic item in regSettings)
                lbSettings.Items.Add($"{item.Key} = {item.Value}");
        }
        private void sValueValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            regSettings[tbChngeValueName.Text] = sValue.Value;
            lbSettings.Items.Clear();
            foreach (dynamic item in regSettings)
                lbSettings.Items.Add($"{item.Key} = {item.Value}");
        }
        private void btnClearClick(object sender, RoutedEventArgs e)
        {
            regSettings.Clear();

            foreach (dynamic item in regSettings)
                lbSettings.Items.Add($"{item.Key} = {item.Value}");
        }
        //Update
        private void btnInitUpdateClick(object sender, RoutedEventArgs e)
        {
            update = new UpdateClient(tbUpdateUrl.Text);
            update.NewVersion += UpdateNewVersion;

            update.Check(new Verloka.HelperLib.Update.Version(1,1,1,0));
        }
        private void UpdateNewVersion(UpdateItem obj)
        {

        }
        private void btnAddUpdateFileClick(object sender, RoutedEventArgs e)
        {
            lbUpdateFiles.Items.Add(tbUpdateFile.Text);
            tbUpdateFile.Text = "";
        }
        private void btnCreateUpdateFileClick(object sender, RoutedEventArgs e)
        {
            UpdateItem item = new UpdateItem();
            item.Title = tbUpdateTitle.Text;
            item.ChangeNote = tbUpdateChangeLog.Text;
            item.Date = DateTime.Now;
            item.VersionNumber = new Verloka.HelperLib.Update.Version(tbUpdateVersionMajor.Text,
                                                                      tbUpdateVersionMinor.Text,
                                                                      tbUpdateVersionRevision.Text,
                                                                      tbUpdateVersionBuild.Text);

            foreach (var url in lbUpdateFiles.Items)
                item.Files.Add(url as string);

            string str = UpdateClient.Serialize(item);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Json file (*.json)|*.json";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, str, Encoding.UTF8);
        }
    }
}
