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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UpdateApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnMakeArchiveClick(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string path = $"{fbd.SelectedPath}\\";
                    string save = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\archive.zip";
                    await Verloka.HelperLib.Update.Worker.Archive(path, save);

                    System.Windows.MessageBox.Show(this, $"Archive was created and saved on your desktop\nFROM: {path}\nTO: {save}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
