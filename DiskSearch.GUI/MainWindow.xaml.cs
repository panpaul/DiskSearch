using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Database;
using Sentry;
using Sentry.Protocol;

namespace DiskSearch.GUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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

            _config = new Config(Path.Combine(_basePath, "config.json"));

            SetupIndex();
            _backend.Watch(_config.SearchPath);
        }

        private void SetupIndex()
        {
            _backend = new Backend(_basePath);
            AppDomain.CurrentDomain.ProcessExit += (s, e) => _backend.Close();
            RefreshIndex.IsEnabled = true;
        }

        private async void RefreshIndex_Click(object sender, RoutedEventArgs e)
        {
            SearchKeyword.IsEnabled = false;
            RefreshIndex.Content = "Refreshing...";
            RefreshIndex.IsEnabled = false;
            await Task.Run(() => { _backend.Walk(_config.SearchPath); });

            SearchKeyword.IsEnabled = true;
            RefreshIndex.Content = "Refresh Index";
            RefreshIndex.IsEnabled = true;
        }

        private void SearchKeyword_TextChanged(object sender, TextChangedEventArgs e)
        {
            var schemes = _backend.Search(SearchKeyword.Text);
            _resultList.Clear();
            foreach (var scheme in schemes) _resultList.Add(new Results(scheme));
        }
    }

    internal class Results
    {
        public Results(Engine.Scheme scheme)
        {
            Path = scheme.Path;
            Filename = System.IO.Path.GetFileName(Path);
        }

        public string Filename { get; }
        public string Path { get; }
    }
}