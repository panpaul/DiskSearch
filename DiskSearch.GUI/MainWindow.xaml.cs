using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DiskSearch.Worker.Services;
using Grpc.Net.Client;
using Sentry;
using Sentry.Protocol;

namespace DiskSearch.GUI
{
    public partial class MainWindow
    {
        private readonly Config _config;
        private readonly BindingList<Results> _resultList;
        private readonly DispatcherTimer _timer;
        private GrpcChannel _channel;
        private Search.SearchClient _client;


        public MainWindow()
        {
            /* Sentry init */
            SentrySdk.Init("https://e9bae2c6285e48ea814087d78c9a40f1@sentry.io/4202655");
            SentrySdk.ConfigureScope(scope =>
            {
                scope.User = new User
                {
                    Id = MachineCode.MachineCode.GetMachineCode()
                };
            });

            /* UI init */
            InitializeComponent();
            TagSelector.SelectedItem = TagSelector;

            /* Config file init */
            var basePath =
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "DiskSearch"
                );
            _config = new Config(Path.Combine(basePath, "config.json"));

            /* Result list init */
            _resultList = new BindingList<Results>();
            ResultListView.ItemsSource = _resultList;

            /* RPC */
            SetupRemote();

            /* Timer */
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // every 1 seconds
            };
            _timer.Tick += OnTimedEvent;

            Closed += OnClosedEvent;
        }

        #region Search Related

        private void OnTimedEvent(object sender, EventArgs e)
        {
            _timer.Stop();
            var word = SearchKeyword.Text;
            var tag = TagSelector.Text;
            SearchHint.Content = $"Searching for: {word} with tag: {tag}.";
            Task.Run(() => { DoSearch(word, tag); });
        }

        private void SearchKeyword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_timer.IsEnabled)
            {
                // restart the timer
                _timer.Stop();
                _timer.Start();
            }
            else
            {
                // just start it
                _timer.Start();
            }
        }

        private void TagSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchKeyword.Text.Equals("")) return;
            var tag = ((ComboBoxItem) TagSelector.SelectedItem).Content.ToString();
            DoSearch(SearchKeyword.Text, tag);
        }

        private void RebuildIndex_Click(object sender, RoutedEventArgs e)
        {
            //RebuildIndex.Content = "Rebuilding...";
            //BlockInput();

            _client.Control(new CommandRequest
            {
                Cmd = CommandRequest.Types.Command.ReloadPath,
                Path = _config.SearchPath
            });

            //RebuildIndex.Content = "Rebuild Index";
            //RestoreInput();
        }

        private void Config_Click(object sender, RoutedEventArgs e)
        {
            var oldPath = _config.SearchPath;
            var configWindow = new ConfigWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            configWindow.ShowDialog();

            _config.Read();
            _client.Control(new CommandRequest {Cmd = CommandRequest.Types.Command.ReloadBlackList});

            if (oldPath != _config.SearchPath)
                RebuildIndex_Click(sender, e);
        }

        /// <summary>
        ///     Search files through worker
        /// </summary>
        /// <param name="word">keyword</param>
        /// <param name="tag">file type tag</param>
        private void DoSearch(string word, string tag)
        {
            var results = _client.DoSearch(new SearchRequest {Tag = tag, Word = word});
            Dispatcher.BeginInvoke((Action) delegate
            {
                if (SearchKeyword.Text != word || TagSelector.Text != tag) return;
                _resultList.Clear();
                foreach (var scheme in results.Results)
                {
                    if (scheme.Path.Equals("null")) continue;
                    _resultList.Add(new Results(scheme));
                }
            });
        }

        /// <summary>
        ///     Connect to DiskSearch.Worker
        /// </summary>
        private void SetupRemote()
        {
            _channel = GrpcChannel.ForAddress("https://localhost:5001");
            _client = new Search.SearchClient(_channel);

            RestoreInput();
        }

        #endregion

        #region Menu Item

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

            _client.Control(new CommandRequest
            {
                Cmd = CommandRequest.Types.Command.DeletePath,
                Path = item.Path
            });
            _resultList.Remove(item);
            // TODO: Remove in list
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

        #endregion

        #region Window & Tray

        /// <summary>
        ///     Minimize
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnStateChanged(object sender, EventArgs e)
        {
            ShowInTaskbar = WindowState != WindowState.Minimized;
        }

        /// <summary>
        ///     Shutdown method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosedEvent(object sender, EventArgs e)
        {
            TaskBar.Dispose();
            _channel.Dispose();
        }

        /// <summary>
        ///     Tray Open/Hide button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        ///     Tray Exit button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tray_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     Restore UI interaction
        /// </summary>
        private void BlockInput()
        {
            SearchKeyword.IsEnabled = false;
            RebuildIndex.IsEnabled = false;
            Config.IsEnabled = false;
        }

        /// <summary>
        ///     Block interaction on UI
        /// </summary>
        private void RestoreInput()
        {
            SearchKeyword.IsEnabled = true;
            RebuildIndex.IsEnabled = true;
            Config.IsEnabled = true;
        }

        #endregion
    }

    internal class Results
    {
        public Results(Scheme scheme)
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