using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace DiskSearch.GUI
{
    public partial class ConfigWindow : Window
    {
        private readonly Config _config;

        public ConfigWindow()
        {
            InitializeComponent();
            var basePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DiskSearch"
            );
            _config = new Config(Path.Combine(basePath, "config.json"));
            PathTextBox.Text = _config.SearchPath;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var newPath = PathTextBox.Text;
            if (!Directory.Exists(newPath))
            {
                MessageBox.Show(this, "Directory Not Existed", "ERROR");
                return;
            }

            _config.SearchPath = newPath;
            _config.Save();
            Close();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PathTextBox.Text = folderDialog.SelectedPath;
            }
        }
    }
}