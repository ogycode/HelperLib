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
  
            lvItems.ItemsSource = iniFile.ToDictionary();
        }
        void OpenFile(string name)
        {
            if(string.IsNullOrWhiteSpace(tbSeparator.Text))
            {
                lblStatus.Content = "Separator can not be empty!";
                return;
            }
            if (string.IsNullOrWhiteSpace(tbComment.Text))
            {
                lblStatus.Content = "Comment can not be empty!";
                return;
            }
            if(string.IsNullOrWhiteSpace(tbLeftB.Text) || string.IsNullOrWhiteSpace(tbRightB.Text))
            {
                lblStatus.Content = "Bracket can not be empty!";
                return;
            }

            iniFile = new Verloka.HelperLib.INI.INIFile(name, tbSeparator.Text, tbComment.Text, tbLeftB.Text, tbRightB.Text);
            tbFilePath.Text = name;
            lblStatus.Content = $"File \'{name}\' is loaded";

            cbSections.Items?.Clear();
            cbSections.Items.Add("--All--");
            foreach (var item in iniFile.Sections)
                cbSections.Items.Add(item.Name);
            cbSections.SelectedIndex = 0;
            cbSections.SelectionChanged += CbSectionsSelectionChanged;

            SetList();
        }

        private void CbSectionsSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (cbSections.SelectedIndex)
            {
                case -1:
                    return;
                case 0:
                    SetList();
                    break;
                default:
                    lvItems.ItemsSource = iniFile[cbSections.SelectedItem.ToString()].GetPureContent();
                    break;
            }
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
                    OpenFile(ofd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
        private void btnDeleteClick(object sender, RoutedEventArgs e)
        {
            if(iniFile == null)
            {
                lblStatus.Content = "Need ini file for work!";
                return;
            }

            if(string.IsNullOrWhiteSpace(tbDeleteName.Text))
            {
                lblStatus.Content = "Wrong name, for removing enter correct name to text box";
                return;
            }

            if (iniFile.Remove(tbDeleteName.Text))
            {
                lblStatus.Content = $"\'{tbDeleteName.Text}\' is removing";
                SetList();
                iniFile.Save();
            }
            else
                lblStatus.Content = $"\'{tbDeleteName.Text}\' is not found in file";

            tbDeleteName.Text = "";
        }
        private void btnAddClick(object sender, RoutedEventArgs e)
        {
            if (iniFile == null)
            {
                lblStatus.Content = "Need ini file for work!";
                return;
            }

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
            iniFile.Save();
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
                    OpenFile(files[0]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
        private void btnShowClick(object sender, RoutedEventArgs e)
        {
            if (iniFile == null)
            {
                lblStatus.Content = "Need ini file for work!";
                return;
            }

            if(string.IsNullOrWhiteSpace(tbSectionName.Text))
            {
                lblStatus.Content = "Wrong section name, enter correct!";
                return;
            }

            if(string.IsNullOrWhiteSpace(tbValueName.Text))
            {
                string value1 = iniFile.Read<string>(tbSectionName.Text);
                lblStatus.Content = $"Value by name \'{tbSectionName.Text}\' = \'{value1}\'";
                MessageBox.Show($"Value by name \'{tbSectionName.Text}\' = \'{value1}\'", "INI Value", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string value = iniFile[tbSectionName.Text][tbValueName.Text]?.ToString();
            lblStatus.Content = $"Value by name \'{tbValueName.Text}\' = \'{value}\'";
            MessageBox.Show($"Value by name \'{tbSectionName.Text}\' = \'{value}\'", "INI Value", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
