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

        public MainWindow()
        {
            InitializeComponent();
        }

        void OpenFile(string name)
        {
            lang = new Manager(name);
            lang.Load();

            lblStatus.Content = $"File \'{name}\' loaded";

            SetData();
        }
        void SetData()
        {
            if (lang == null)
                return;

            //TODO CLEAR OLD DATA FROM TABLE

            int columnNumber = 0;

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

            List<List<string>> data = new List<List<string>>();

            for (int i = 0; i < keys.Count; i++)
            {
                List<string> rows = new List<string>();
                rows.Add(keys[i]);
                foreach (var item in lang.AvailableLanguages)
                    rows.Add(lang.GetValueByLanguage(item.Name, keys[i]));
                data.Add(rows);
            }

            dgData.ItemsSource = data;
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
        private void dgDataEditBegin(object sender, DataGridBeginningEditEventArgs e)
        {
            //start edit
        }
        private void dgDataCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //end edit
        }
    }
}
