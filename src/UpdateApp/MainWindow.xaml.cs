using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace UpdateApp
{
    public partial class MainWindow : Window
    {
        Manager update;
        UpdateElement newElement;

        public MainWindow()
        {
            InitializeComponent();

            SetEnable(false);
        }

        void OpenFile(string path)
        {
            update = new Manager(path);
            update.LoadFromPath();
            lvElements.ItemsSource = update.Elements;

            lblStatus.Content = $"File \'{path}\' loaded ({DateTime.Now.ToLongTimeString()})";
            tbFilePath.Text = path;

            UpdateData();
        }
        void UpdateData()
        {
            lvElements.Items.Refresh();
            lvElements.SelectedItem = update?.Last;
        }
        void SetEnable(bool mode)
        {
            tbTitle.IsEnabled = mode;
            tbChangnote.IsEnabled = mode;
            tbMajor.IsEnabled = mode;
            tbMinor.IsEnabled = mode;
            tbBuild.IsEnabled = mode;
            tbRevision.IsEnabled = mode;
            tbExeFile.IsEnabled = mode;
            tbZipFile.IsEnabled = mode;
            tbDate.IsEnabled = mode;
            btnDone.IsEnabled = mode;
            btnSetDateNow.IsEnabled = mode;
        }
        void SetupDate()
        {
            if (newElement == null)
            {
                lblStatus.Content = $"For setting date of update push button New update";
                return;
            }
            tbDate.Text = DateTime.Now.ToLongDateString();
            newElement.SetDate(DateTime.Now.ToOADate());
        }

        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            tbMajor.TextChanged += versionTextChanged;
            tbMinor.TextChanged += versionTextChanged;
            tbBuild.TextChanged += versionTextChanged;
            tbRevision.TextChanged += versionTextChanged;
        }
        private async void btnMakeArchiveClick(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string path = $"{fbd.SelectedPath}\\";
                    string save = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\archive.zip";
                    await Verloka.HelperLib.Update.Worker.Archive(path, save);

                    MessageBox.Show(this, $"Archive was created and saved on your desktop\nFROM: {path}\nTO: {save}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        private void windowFileDrow(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Count() != 1)
                {
                    lblStatus.Content = "Drag'n'Drop only single file";
                    return;
                }

                string[] parts = files[0].Split('.');

                if (parts.Last() != "ini")
                {
                    lblStatus.Content = "The file must be *.ini";
                    return;
                }

                try
                {
                    OpenFile(files[0]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
        private void btnNewClick(object sender, RoutedEventArgs e)
        {
            SetEnable(true);
            newElement = new UpdateElement();
            SetupDate();
        }
        private void versionTextChanged(object sender, TextChangedEventArgs e)
        {
            if (newElement == null)
            {
                return;
            }

            TextBox t = sender as TextBox;

            int.TryParse(t.Text, out int i);
            t.Text = i.ToString();

            string version = $"{tbMajor.Text}.{tbMinor.Text}.{tbBuild.Text}.{tbRevision.Text}";

            btnDone.Content = $"Draft update: {version}";
            newElement.SetVersionNumber(version);
        }
        private void btnSetDateNowClick(object sender, RoutedEventArgs e)
        {
            SetupDate();
        }
        private void btnDoneClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbTitle.Text))
            {
                lblStatus.Content = $"Title of update can not be empty";
                return;
            }
            else if (string.IsNullOrWhiteSpace(tbChangnote.Text))
            {
                lblStatus.Content = $"Changenote of update can not be empty";
                return;
            }
            else if (string.IsNullOrWhiteSpace(tbExeFile.Text))
            {
                lblStatus.Content = $"Path(url) to setup file of update can not be empty";
                return;
            }
            else if (string.IsNullOrWhiteSpace(tbZipFile.Text))
            {
                lblStatus.Content = $"Path(url) to zip file of update can not be empty";
                return;
            }

            newElement.SetTitle(tbTitle.Text);
            newElement.SetChangeNote(tbChangnote.Text);
            newElement.SetVersionNumber($"{tbMajor.Text}.{tbMinor.Text}.{tbBuild.Text}.{tbRevision.Text}");
            newElement.SetEXE(tbExeFile.Text);
            newElement.SetZIP(tbZipFile.Text);

            if(update.AddElement(newElement))
            {
                update.Save();
                UpdateData();
                lblStatus.Content = $"New update by version({tbMajor.Text}.{tbMinor.Text}.{tbBuild.Text}.{tbRevision.Text}) added!";
                newElement = null;
            }
            else
                lblStatus.Content = $"New update by version({tbMajor.Text}.{tbMinor.Text}.{tbBuild.Text}.{tbRevision.Text}) can not be add!";

            SetEnable(false);

            tbTitle.Text = "Title...";
            tbChangnote.Text = "Log...";
            tbMajor.Text = "1";
            tbMinor.Text = "0";
            tbBuild.Text = "0";
            tbRevision.Text = "0";
            tbExeFile.Text = "exe file path...";
            tbZipFile.Text = "zip file path";

            SetEnable(false);
        }
    }
}
