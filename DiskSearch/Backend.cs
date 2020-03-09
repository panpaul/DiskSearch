﻿using System;
using System.Collections.Generic;
using System.IO;
using Database;
using DocReader;

namespace DiskSearch
{
    internal class Backend
    {
        private readonly Engine _index;

        public Backend(string path)
        {
            _index = new Engine(path);
        }

        public void Close()
        {
            _index.Close();
        }

        public void Walk(string path)
        {
            try
            {
                WalkDir(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void WalkDir(string root)
        {
            var dirs = new Stack<string>(10000);

            if (!Directory.Exists(root)) throw new ArgumentException();

            dirs.Push(root);

            while (dirs.Count > 0)
            {
                var currentDir = dirs.Pop();
                string[] subDirs;
                try
                {
                    subDirs = Directory.GetDirectories(currentDir);
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                string[] files;
                try
                {
                    files = Directory.GetFiles(currentDir);
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                foreach (var file in files)
                    try
                    {
                        var fi = new FileInfo(file);
                        var content = Doc.Read(fi);
                        var doc = Engine.GenerateDocument(fi.FullName, content);
                        _index.Add(doc);
                        Console.WriteLine("Index Added: {0}: {1}", fi.FullName, fi.Length);
                    }
                    catch (FileNotFoundException e)
                    {
                        Console.WriteLine(e.Message);
                    }

                _index.Flush();

                foreach (var str in subDirs)
                    dirs.Push(str);
            }
        }

        public void Watch(string path)
        {
            try
            {
                var fsWatcher = new FileSystemWatcher(path)
                {
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = true
                };

                fsWatcher.Created += Handler;
                fsWatcher.Changed += Handler;
                fsWatcher.Deleted += Handler;
                fsWatcher.Renamed += RenameHandler;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void RenameHandler(object source, RenamedEventArgs e)
        {
            try
            {
                _index.Delete(e.OldFullPath);
                Console.WriteLine("\nIndex Deleted: {0}", e.FullPath);

                var fi = new FileInfo(e.FullPath);
                var content = Doc.Read(fi);
                var doc = Engine.GenerateDocument(fi.FullName, content);
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
            try
            {
                switch (e.ChangeType)
                {
                    case WatcherChangeTypes.Deleted:
                        _index.Delete(e.FullPath);
                        Console.WriteLine("\nIndex Deleted: {0}", e.FullPath);
                        return;
                    case WatcherChangeTypes.Changed:
                    {
                        var fi = new FileInfo(e.FullPath);
                        var content = Doc.Read(fi);
                        var doc = Engine.GenerateDocument(fi.FullName, content);
                        _index.Add(doc);
                        Console.WriteLine("\nIndex Updated: {0}: {1}", fi.FullName, fi.Length);
                        return;
                    }
                    case WatcherChangeTypes.Created:
                    {
                        var fi = new FileInfo(e.FullPath);
                        var content = Doc.Read(fi);
                        var doc = Engine.GenerateDocument(fi.FullName, content);
                        _index.Add(doc);
                        Console.WriteLine("\nIndex Added: {0}: {1}", fi.FullName, fi.Length);
                        break;
                    }
                    default:
                        Console.WriteLine("\nError Occured");
                        break;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            
        }

        public void Prompt()
        {
            while (true)
            {
                Console.Write("Search for What ? >");
                var word = Console.ReadLine();
                if (word == null || word.Equals("QUIT"))
                {
                    break;
                }

                Console.WriteLine("==== Searching for : " + word + " ====");
                var schemes = _index.Search(word);
                foreach (var scheme in schemes) Console.WriteLine(scheme.Path);
                Console.WriteLine("==== End Search ====");
            }
        }
    }
}