using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Verloka.HelperLib.Update;

namespace UpdateApp
{
    public partial class MainWindow : Window
    {
        Manager update;
        UpdateElement newElement;
        public bool IsEdit
        {
            get => lblEditMode.Visibility == Visibility.Visible ? true : false;
            set => lblEditMode.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

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

            lvElements.IsEnabled = !mode;
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
        void NullTb()
        {
            tbTitle.Text = "Title";
            tbChangnote.Text = "Log";
            tbMajor.Text = "1";
            tbMinor.Text = "0";
            tbBuild.Text = "0";
            tbRevision.Text = "0";
            tbExeFile.Text = "setup.exe's url...";
            tbZipFile.Text = "archive.zip's url...";
        }
        void AddElement()
        {
            if (update.AddElement(newElement))
            {
                lblStatus.Content = $"New update by version({tbMajor.Text}.{tbMinor.Text}.{tbBuild.Text}.{tbRevision.Text}) added!";

                update.Save();
                UpdateData();
                newElement = null;
            }
            else
                lblStatus.Content = $"New update by version({tbMajor.Text}.{tbMinor.Text}.{tbBuild.Text}.{tbRevision.Text}) can not be edit!";
        }
        void EditElement()
        {
            if (lvElements.SelectedIndex == -1)
            {
                lblStatus.Content = "Update by can not be edit!";
                return;
            }

            if (update.RemoveElement(lvElements.SelectedItem as UpdateElement) && update.AddElement(newElement))
            {
                lblStatus.Content = $"Update by version({tbMajor.Text}.{tbMinor.Text}.{tbBuild.Text}.{tbRevision.Text}) edited!";
                NullTb();
                update.Save();
                UpdateData();
            }

            IsEdit = false;
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
                    await Worker.Archive(path, save);

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
            NullTb();
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

            SetupDate();

            if (IsEdit)
                EditElement();
            else
                AddElement();
            
            NullTb();
            SetEnable(false);
        }
        private void btnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (lvElements.SelectedIndex == -1)
                return;

            var ver = (lvElements.SelectedItem as UpdateElement).GetVersionNumber();
            if (update.RemoveElement(ver))
            {
                lblStatus.Content = $"Update by verison {ver} removed";
                update.Save();
                UpdateData();
            }
            else
                lblStatus.Content = $"Update by verison {ver} can not be remove";
        }
        private void btnEditClick(object sender, RoutedEventArgs e)
        {
            if (lvElements.SelectedIndex == -1)
                return;

            IsEdit = true;

            UpdateElement elem = lvElements.SelectedItem as UpdateElement;
            newElement = new UpdateElement();

            tbTitle.Text = elem.GetTitle();
            tbChangnote.Text = elem.GetChangeNote();
            tbExeFile.Text = elem.GetEXE();
            tbZipFile.Text = elem.GetZIP();
            tbMajor.Text = elem.GetVersionNumber().GetMajor().ToString();
            tbMinor.Text = elem.GetVersionNumber().GetMinor().ToString();
            tbBuild.Text = elem.GetVersionNumber().GetBuild().ToString();
            tbRevision.Text = elem.GetVersionNumber().GetRevision().ToString();
            tbDate.Text = DateTime.FromOADate(elem.GetDate()).ToLongDateString();

            SetEnable(true);
        }
        private void btnBrowseClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.InitialDirectory = "c:\\";
            ofd.Filter = "update files (*.ini)|*.ini";
            ofd.FilterIndex = 2;
            ofd.RestoreDirectory = true;
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == true)
            {
                try
                {
                    OpenFile(ofd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
        private void lvElementsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvElements.SelectedIndex == -1)
                return;

            UpdateElement elem = lvElements.SelectedItem as UpdateElement;
            tbTitle.Text = elem.GetTitle();
            tbChangnote.Text = elem.GetChangeNote();
            tbExeFile.Text = elem.GetEXE();
            tbZipFile.Text = elem.GetZIP();
            tbMajor.Text = elem.GetVersionNumber().GetMajor().ToString();
            tbMinor.Text = elem.GetVersionNumber().GetMinor().ToString();
            tbBuild.Text = elem.GetVersionNumber().GetBuild().ToString();
            tbRevision.Text = elem.GetVersionNumber().GetRevision().ToString();
            tbDate.Text = DateTime.FromOADate(elem.GetDate()).ToLongDateString();
        }
    }
}
