﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

            SetData();
        }
        void SetData()
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

            cbLocales.SelectionChanged += CbLocalesSelectionChanged;
            cbLocalesRename.SelectionChanged += CbLocalesRenameSelectionChanged;
        }
        void AppendLocales()
        {
            cbMainLocal.SelectionChanged -= CbMainLocalSelectionChanged;

            cbLocales.Items.Clear();
            cbMainLocal.Items.Clear();
            cbRemovingLocal.Items.Clear();
            cbLocalesRename.Items.Clear();

            cbLocales.Items.Add("--All--");

            foreach (var item in lang.AvailableLanguages)
            {
                cbLocales.Items.Add(item);
                cbMainLocal.Items.Add(item);
                cbRemovingLocal.Items.Add(item);
                cbLocalesRename.Items.Add(item);
            }

            cbLocales.SelectedIndex = 0;
            cbMainLocal.SelectedItem = lang.Current;

            cbMainLocal.SelectionChanged += CbMainLocalSelectionChanged;
        }

        private void CbLocalesRenameSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbLocalesRename.SelectedIndex == -1)
                return;

            tbRename.Text = (cbLocalesRename.SelectedItem as Language).Name;
        }
        private void CbMainLocalSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbMainLocal.SelectedIndex == -1)
                return;

            lang.SetCurrent(((Language)cbMainLocal.SelectedItem).Code);

            if (cbSave.IsChecked.Value)
                lang.Save();
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

            if (string.IsNullOrWhiteSpace(t.Text))
            {
                lblStatus.Content = $"Edited value can not be empty!";
                t.Text = ((List<string>)e.Row.Item)[0];
            }

            lang.EditKey(e.Column.Header.ToString(), ((List<string>)e.Row.Item)[0], t.Text);
            
            if (cbSave.IsChecked.Value)
                lang.Save();
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
            if (string.IsNullOrWhiteSpace(tbNodeAddName.Text))
            {
                lblStatus.Content = $"Node name can not be empty!";
                return;
            }

            lang.AddKey(tbNodeAddName.Text);
            lblStatus.Content = $"Node \'{tbNodeAddName.Text}\' added";
            tbNodeAddName.Text = "";

            SetData();
            if (cbSave.IsChecked.Value)
                lang.Save();
        }
        private void btnRemoveLocalClick(object sender, RoutedEventArgs e)
        {
            if (cbRemovingLocal.SelectedIndex == -1)
                return;

            lblStatus.Content = lang.RemoveLocale(((Language)cbRemovingLocal.SelectedItem).Name) ?
                $"{((Language)cbRemovingLocal.SelectedItem).Name} is remove" :
                $"{((Language)cbRemovingLocal.SelectedItem).Name} can not be removing";

            SetData();

            if (cbSave.IsChecked.Value)
                lang.Save();
        }
        private void btnRemoveNodeClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbNodeRemoveName.Text))
            {
                lblStatus.Content = $"Key name for removing can not be empty!";
                return;
            }

            lblStatus.Content = lang.RemoveKey(tbNodeRemoveName.Text) ?
                $"\'{tbNodeRemoveName.Text}\' was removed" :
                $"\'{tbNodeRemoveName.Text}\' can not be removed";

            tbNodeRemoveName.Text = "";
            SetData();

            if (cbSave.IsChecked.Value)
                lang.Save();
        }
        private void btnSaveClick(object sender, RoutedEventArgs e)
        {
            lang.Save();
        }
        private void btnAddLocalClick(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(tbLocName.Text) || string.IsNullOrWhiteSpace(tbLocCode.Text))
            {
                lblStatus.Content = "Locale name and code can not be empty!";
                return;
            }

            lblStatus.Content = lang.AddLocale(tbLocName.Text, tbLocCode.Text) ?
                $"Locale {tbLocName.Text}({tbLocCode.Text}) added" :
                $"Locale {tbLocName.Text}({tbLocCode.Text}) can not be add";

            SetData();
            if (cbSave.IsChecked.Value)
                lang.Save();
        }
        private void btnRenameClick(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(tbRename.Text))
            {
                lblStatus.Content = "Name of language is empty!";
                return;
            }

            if (lang.RenameLocale((cbLocalesRename.SelectedItem as Language).Name, tbRename.Text))
            {
                lblStatus.Content = "New name apply";
                SetData();
                if (cbSave.IsChecked.Value)
                    lang.Save();

                tbRename.Text = "";
            }
            else
                lblStatus.Content = "New name not apply";
        }
    }
}
