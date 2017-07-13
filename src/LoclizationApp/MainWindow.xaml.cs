using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
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
using Verloka.HelperLib.Localization;

namespace LoclizationApp
{
    public partial class MainWindow : Window
    {
        Manager lang;
        List<List<string>> data;

        public MainWindow()
        {
            InitializeComponent();

            data = new List<List<string>>();
            dgData.ItemsSource = data;
        }

        void OpenFile(string name)
        {
            lang = new Manager(name);
            lang.Load();

            lblStatus.Content = $"File \'{name}\' loaded ({DateTime.Now.ToLongTimeString()})";
            tbFilePath.Text = name;

            SetData(false);
        }
        void SetData(bool save)
        {
            if (lang == null)
                return;

            int columnNumber = 0;

            //for reload data need delete
            dgData.Columns.Clear();
            data.Clear();

            //keys
            DataGridTextColumn keyHead = new DataGridTextColumn();
            keyHead.Header = "Keys";
            keyHead.Binding = new Binding($"[{columnNumber.ToString()}]");
            keyHead.IsReadOnly = true;
            dgData.Columns.Add(keyHead);
            
            //languages
            foreach (var item in lang.AvailableLanguages)
            {
                columnNumber++;
                DataGridTextColumn head = new DataGridTextColumn();
                head.Header = item.Name;
                head.Binding = new Binding($"[{columnNumber.ToString()}]");
                dgData.Columns.Add(head);
            }

            List<string> keys = lang.Keys();

            for (int i = 0; i < keys.Count; i++)
            {
                List<string> rows = new List<string>();
                rows.Add(keys[i]);
                foreach (var item in lang.AvailableLanguages)
                    rows.Add(lang.GetValueByLanguage(item.Name, keys[i]));
                data.Add(rows);
            }

            dgData.Items.Refresh();
            AppendLocales();
            cbMainLocal.SelectedItem = lang.Current;

            cbLocales.SelectionChanged += CbLocalesSelectionChanged;
            cbMainLocal.SelectionChanged += CbMainLocalSelectionChanged;

            if (save)
                lang.Save();
        }
        void AppendLocales()
        {
            cbLocales.Items.Clear();
            cbMainLocal.Items.Clear();
            cbRemovingLocal.Items.Clear();

            cbLocales.Items.Add("--All--");

            foreach (var item in lang.AvailableLanguages)
            {
                cbLocales.Items.Add(item);
                cbMainLocal.Items.Add(item);
                cbRemovingLocal.Items.Add(item);
            }

            cbLocales.SelectedIndex = 0;
        }

        private void CbMainLocalSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbMainLocal.SelectedIndex == -1)
                return;

            lang.SetCurrent(((Language)cbMainLocal.SelectedItem).Code);
        }
        private void CbLocalesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbLocales.SelectedIndex == -1)
                return;

            if (cbLocales.SelectedIndex == 0)
            {
                foreach (var item in dgData.Columns)
                    item.Visibility = Visibility.Visible;
                return;
            }

            foreach (var item in dgData.Columns)
                if (item.Header.ToString() != "Keys")
                    item.Visibility = item.Header.ToString() != ((Language)cbLocales.SelectedItem).Name ? Visibility.Hidden : Visibility.Visible;
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
        private void dgDataCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //end edit
            TextBox t = e.EditingElement as TextBox; //value

            if(string.IsNullOrWhiteSpace(t.Text))
            {
                lblStatus.Content = $"Edited value can not be empty!";
                e.Cancel = true;
                return;
            }

            //lang.EditKey(e.Column.Header, e.Row.)
        }
        private void btnBrowseClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.InitialDirectory = "c:\\";
            ofd.Filter = "locale files (*.ini)|*.ini";
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
        private void btnAddNodeClick(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(tbNodeAddName.Text))
            {
                lblStatus.Content = $"Node name can not be empty!";
                return;
            }

            lang.AddKey(tbNodeAddName.Text);
            lblStatus.Content = $"Node \'{tbNodeAddName.Text}\' added";
            tbNodeAddName.Text = "";

            SetData(cbSave.IsChecked.Value);
        }
        private void btnRemoveLocalClick(object sender, RoutedEventArgs e)
        {
            if (cbRemovingLocal.SelectedIndex == -1)
                return;

            lblStatus.Content = lang.RemoveLocale(((Language)cbRemovingLocal.SelectedItem).Name) ?
                $"{((Language)cbRemovingLocal.SelectedItem).Name} is remove" :
                $"{((Language)cbRemovingLocal.SelectedItem).Name} can not be removing";

            SetData(cbSave.IsChecked.Value);
        }
        private void btnRemoveNodeClick(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(tbNodeRemoveName.Text))
            {
                lblStatus.Content = $"Key name for removing can not be empty!";
                return;
            }

            lblStatus.Content = lang.RemoveKey(tbNodeRemoveName.Text) ?
                $"\'{tbNodeRemoveName.Text}\' was removed" :
                $"\'{tbNodeRemoveName.Text}\' can not be removed";

            tbNodeRemoveName.Text = "";
            SetData(cbSave.IsChecked.Value);
        }
        private void btnSaveClick(object sender, RoutedEventArgs e)
        {
            SetData(true);
        }
    }
}
