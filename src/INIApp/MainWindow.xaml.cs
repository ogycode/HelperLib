using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;

namespace INIApp
{
    public partial class MainWindow : Window
    {
        Verloka.HelperLib.INI.INIFile iniFile;

        public MainWindow()
        {
            InitializeComponent();
        }

        void SetList()
        {
            if (iniFile == null)
                return;
  
            lvItems.ItemsSource = iniFile?.ToDictionary();
        }

        private void btnBrowseClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.InitialDirectory = "c:\\";
            ofd.Filter = "ini files (*.ini)|*.ini";
            ofd.FilterIndex = 2;
            ofd.RestoreDirectory = true;
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == true)
            {
                try
                {
                    iniFile = new Verloka.HelperLib.INI.INIFile(ofd.FileName);
                    tbFilePath.Text = ofd.FileName;
                    SetList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
        private void btnDeleteClick(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(tbDeleteName.Text))
            {
                lblStatus.Content = "Wrong name, for removing enter correct name to text box";
                return;
            }

            if (iniFile.Remove(tbDeleteName.Text))
            {
                lblStatus.Content = $"\'{tbDeleteName.Text}\' is removing";
                SetList();
            }
            else
                lblStatus.Content = $"\'{tbDeleteName.Text}\' is not found in file";

            tbDeleteName.Text = "";
        }
        private void btnAddClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbAddKey.Text))
            {
                lblStatus.Content = "Wrong key, for adding enter correct key to text box";
                return;
            }
            if (string.IsNullOrWhiteSpace(tbAddValue.Text))
            {
                lblStatus.Content = "Wrong value, for adding enter correct value to text box";
                return;
            }

            if(iniFile.Write(tbAddKey.Text, tbAddValue.Text))
                lblStatus.Content = $"\'{tbAddKey.Text}\' is added";
            else
                lblStatus.Content = $"\'{tbAddKey.Text}\' is edited";

            SetList();
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

                if(parts.Last() != "ini")
                {
                    lblStatus.Content = "The file must be *.ini";
                    return;
                }

                try
                {
                    iniFile = new Verloka.HelperLib.INI.INIFile(files[0]);
                    tbFilePath.Text = files[0];
                    SetList();
                    lblStatus.Content = $"File \'{files[0]}\' is loaded";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
    }
}
