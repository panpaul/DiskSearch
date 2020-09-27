using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Database;
using Sentry;
using Sentry.Protocol;

namespace DiskSearch.GUI
{
    public partial class MainWindow
    {
        private readonly string _basePath;
        private readonly Config _config;
        private readonly BindingList<Results> _resultList;

        private Backend _backend;

        public MainWindow()
        {
            SentrySdk.Init("https://e9bae2c6285e48ea814087d78c9a40f1@sentry.io/4202655");
            SentrySdk.ConfigureScope(scope =>
            {
                scope.User = new User
                {
                    Id = MachineCode.MachineCode.GetMachineCode()
                };
            });

            InitializeComponent();

            _basePath =
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "DiskSearch"
                );

            _resultList = new BindingList<Results>();
            ResultListView.ItemsSource = _resultList;

            TagSelector.SelectedItem = TagSelector;

            _config = new Config(Path.Combine(_basePath, "config.json"));

            SetupIndex();
            _backend.Watch(_config.SearchPath);

            Closed += OnClosedEvent;
        }

        private void OnClosedEvent(object sender, EventArgs e)
        {
            _backend.Close();
            TaskBar.Dispose();
        }

        private void MainWindow_OnStateChanged(object sender, EventArgs e)
        {
            ShowInTaskbar = WindowState != WindowState.Minimized;
        }

        private void SearchKeyword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_backend == null) return;

            var word = SearchKeyword.Text;
            var tag = TagSelector.Text;
            Task.Run(() => { DoSearch(word, tag); });
        }

        private void TagSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_backend == null || SearchKeyword.Text.Equals("")) return;
            var tag = ((ComboBoxItem) TagSelector.SelectedItem).Content.ToString();
            DoSearch(SearchKeyword.Text, tag);
        }

        private async void RefreshIndex_Click(object sender, RoutedEventArgs e)
        {
            BlockInput();
            RefreshIndex.Content = "Refreshing...";

            await Task.Run(() => { _backend.Walk(_config.SearchPath); });

            RestoreInput();
            RefreshIndex.Content = "Refresh Index";
        }

        private async void RebuildIndex_Click(object sender, RoutedEventArgs e)
        {
            RebuildIndex.Content = "Rebuilding...";
            BlockInput();

            _backend.DeleteAll();

            await Task.Run(() => { _backend.Walk(_config.SearchPath); });

            RebuildIndex.Content = "Rebuild Index";
            RestoreInput();
        }

        private void Config_Click(object sender, RoutedEventArgs e)
        {
            var configWindow = new ConfigWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            configWindow.ShowDialog();

            var oldPath = _config.SearchPath;
            _config.Read();
            if (oldPath != _config.SearchPath)
                RebuildIndex_Click(sender, e);
        }

        private void MenuItem_OpenDir_Click(object sender, RoutedEventArgs e)
        {
            var item = (Results) ResultListView.SelectedItem;

            if (item == null)
            {
                MessageBox.Show("No Files Selected!");
                return;
            }

            if (!File.Exists(item.Path)) return;

            var argument = "/select, \"" + item.Path + "\"";
            Process.Start("explorer.exe", argument);
        }

        private void MenuItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            var item = (Results) ResultListView.SelectedItem;

            if (item == null)
            {
                MessageBox.Show("No Files Selected!");
                return;
            }

            _backend.Delete(item.Path);
        }

        private void MenuItem_Copy_Click(object sender, RoutedEventArgs e)
        {
            var item = (Results) ResultListView.SelectedItem;

            if (item == null)
            {
                MessageBox.Show("No Files Selected!");
                return;
            }

            Clipboard.SetText(item.Path);
        }

        private void Tray_Open_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState switch
            {
                WindowState.Normal => WindowState.Minimized,
                WindowState.Maximized => WindowState.Minimized,
                WindowState.Minimized => WindowState.Normal,
                _ => WindowState
            };
        }

        private void Tray_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DoSearch(string word, string tag)
        {
            var schemes = _backend.Search(word, tag);
            Dispatcher.BeginInvoke((Action) delegate
            {
                if (SearchKeyword.Text != word || TagSelector.Text != tag) return;
                _resultList.Clear();
                foreach (var scheme in schemes) _resultList.Add(new Results(scheme));
            });
        }

        private void SetupIndex()
        {
            _backend = new Backend(_basePath);
            AppDomain.CurrentDomain.ProcessExit += (s, e) => _backend.Close();
            RefreshIndex.IsEnabled = true;
            RebuildIndex.IsEnabled = true;
            SearchKeyword.IsEnabled = true;
            Config.IsEnabled = true;
        }

        private void BlockInput()
        {
            SearchKeyword.IsEnabled = false;
            RefreshIndex.IsEnabled = false;
            RebuildIndex.IsEnabled = false;
            Config.IsEnabled = false;
        }

        private void RestoreInput()
        {
            SearchKeyword.IsEnabled = true;
            RefreshIndex.IsEnabled = true;
            RebuildIndex.IsEnabled = true;
            Config.IsEnabled = true;
        }
    }

    internal class Results
    {
        public Results(Engine.Scheme scheme)
        {
            Description = scheme.Description;
            Path = scheme.Path;
            Filename = System.IO.Path.GetFileName(Path);
        }

        public string Filename { get; }
        public string Path { get; }
        public string Description { get; }
    }
}