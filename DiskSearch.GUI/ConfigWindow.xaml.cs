using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;

namespace DiskSearch.GUI
{
    public partial class ConfigWindow : Window
    {
        private readonly Blacklist _blacklist;
        private readonly Config _config;

        public ConfigWindow()
        {
            InitializeComponent();
            var basePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DiskSearch"
            );

            _config = new Config(Path.Combine(basePath, "config.json"));
            _blacklist = new Blacklist(Path.Combine(basePath, "blacklist.json"));

            PathTextBox.Text = _config.SearchPath;
            foreach (var item in _blacklist.List) BlackListBox.Items.Add(item);
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
            _blacklist.Save();

            Close();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                PathTextBox.Text = folderDialog.SelectedPath;
        }

        private void BlackListButton_Click(object sender, RoutedEventArgs e)
        {
            var str = BlackListTextBox.Text;
            if (str == "") return;
            BlackListBox.Items.Add(str);
            _blacklist.List.Add(str);
            BlackListTextBox.Text = "";
        }

        private void BlackListBox_Delete_Click(object sender, RoutedEventArgs e)
        {
            var item = BlackListBox.SelectedItem;
            if (item == null) return;
            BlackListBox.Items.Remove(item);
            _blacklist.List.Remove((string) item);
        }
    }
}