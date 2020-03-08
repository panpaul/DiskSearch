using System;
using System.Collections.Generic;
using System.IO;
using Database;
using DocReader;

namespace DiskSearch
{
    internal class Walker
    {
        private readonly Engine _index;

        public Walker(string path)
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
                        Console.WriteLine("Index Added: {0}: {1}, {2}", fi.FullName, fi.Length, fi.CreationTime);
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
    }
}