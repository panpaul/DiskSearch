using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Database;
using DocReader;

namespace DiskSearch
{
    public class Backend
    {
        private readonly string _basePath;
        private readonly Blacklist _blacklist;
        private readonly Engine _index;
        private FileSystemWatcher _fsWatcher;
        private bool _init;

        public Backend(string path)
        {
            try
            {
                _basePath = path;
                _blacklist = new Blacklist(Path.Combine(path, "blacklist.json"));
                _index = new Engine(Path.Combine(path, "index"));
                _init = true;
            }
            catch (Exception e)
            {
                _init = false;
                Console.WriteLine(e);
            }
        }

        #region Main Functions

        public IEnumerable<Engine.Scheme> Search(string word, string tag)
        {
            tag = tag.ToLower();
            if (tag.Equals("all")) tag = "";

            return _init ? _index.Search(word, tag) : null;
        }

        public void Delete(string filepath)
        {
            if (_init) _index.Delete(filepath);
        }

        public void DeleteAll()
        {
            if (_init) _index.DeleteAll();
        }

        #endregion

        #region Walk And Monitor

        public void Walk(string path)
        {
            if (!_init) return;

            try
            {
                // disallow querying while walking
                _init = false;
                WalkDir(path);
                _init = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void WalkDir(string root)
        {
            var dirs = new Stack<string>();

            if (!Directory.Exists(root)) throw new ArgumentException();

            dirs.Push(root);

            while (dirs.Count > 0)
            {
                var currentDir = dirs.Pop();
                try
                {
                    var subDirs = Directory.GetDirectories(currentDir);
                    foreach (var str in subDirs)
                        dirs.Push(str);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                string[] files;
                try
                {
                    files = Directory.GetFiles(currentDir);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                foreach (var file in files)
                    try
                    {
                        if (_blacklist.Judge(file)) continue;
                        var fi = new FileInfo(file);
                        var doc = Doc.Read(fi);
                        _index.Add(doc);
                        Console.WriteLine("Index Added/Updated: {0}: {1}", fi.FullName, fi.Length);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                //_index.Commit();
            }
        }

        public void Watch(string path)
        {
            if (!_init) return;
            try
            {
                _fsWatcher = new FileSystemWatcher(path)
                {
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = true
                };

                _fsWatcher.Created += Handler;
                _fsWatcher.Changed += Handler;
                _fsWatcher.Deleted += Handler;
                _fsWatcher.Renamed += RenameHandler;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void UnWatch()
        {
            if (!_init) return;
            _fsWatcher.Created -= Handler;
            _fsWatcher.Changed -= Handler;
            _fsWatcher.Deleted -= Handler;
            _fsWatcher.Renamed -= RenameHandler;
            _fsWatcher.Dispose();
        }

        private void RenameHandler(object source, RenamedEventArgs e)
        {
            if (!_init) return;
            try
            {
                _index.Delete(e.OldFullPath);
                Console.WriteLine("\nIndex Deleted: {0}", e.FullPath);
                if (_blacklist.Judge(e.FullPath)) return;
                var fi = new FileInfo(e.FullPath);
                var doc = Doc.Read(fi);
                _index.Add(doc);
                Console.WriteLine("\nIndex Added: {0}: {1}", fi.FullName, fi.Length);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Handler(object source, FileSystemEventArgs e)
        {
            if (!_init) return;
            try
            {
                string path;
                switch (e.ChangeType)
                {
                    case WatcherChangeTypes.Deleted:
                    {
                        _index.Delete(e.FullPath);
                        Console.WriteLine("\nIndex Deleted: {0}", e.FullPath);
                        return;
                    }
                    case WatcherChangeTypes.Changed:
                    {
                        path = e.FullPath;
                        break;
                    }
                    case WatcherChangeTypes.Created:
                    {
                        path = e.FullPath;
                        break;
                    }
                    default:
                        Console.WriteLine("\nError Occured");
                        return;
                }

                if (_blacklist.Judge(path)) return;
                var fi = new FileInfo(e.FullPath);
                var doc = Doc.Read(fi);
                _index.Add(doc);
                Console.WriteLine("\nIndex Updated: {0}: {1}", fi.FullName, fi.Length);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Status Control

        public void UpdateBlackList()
        {
            _blacklist.Update();
        }

        public void DefaultSetup()
        {
            try
            {
                var jsonString = File.ReadAllText(Path.Combine(_basePath, "config.json"));
                var config = JsonDocument.Parse(jsonString);
                var path = config.RootElement.GetProperty("SearchPath").GetString();
                Watch(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Close()
        {
            _init = false;
            _index?.Close();
        }

        public void Flush()
        {
            if (!_init) return;
            _index.Flush();
        }

        #endregion
    }
}