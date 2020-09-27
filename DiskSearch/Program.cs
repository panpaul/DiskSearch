using System;
using System.IO;
using Sentry;
using Sentry.Protocol;

namespace DiskSearch
{
    internal class Program
    {
        private static Backend _backend;

        private static void Main(string[] args)
        {
            SentrySdk.Init("https://e9bae2c6285e48ea814087d78c9a40f1@sentry.io/4202655");
            SentrySdk.ConfigureScope(scope =>
            {
                scope.User = new User
                {
                    Id = MachineCode.MachineCode.GetMachineCode()
                };
            });

            var basePath =
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "DiskSearch"
                );
            _backend = new Backend(basePath);

            AppDomain.CurrentDomain.ProcessExit += (s, e) => _backend.Close();
            if (args.Length != 1)
            {
                Prompt();
            }
            else
            {
                Console.WriteLine("Indexing...");
                _backend.Walk(args[0]);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                _backend.Watch(args[0]);
                Prompt();
            }
        }

        private static void Prompt()
        {
            while (true)
            {
                Console.Write("Search for What ? >");
                var word = Console.ReadLine();
                if (word == null || word.Equals("!QUIT")) break;
                Console.Clear();
                Console.WriteLine("==== Searching for : " + word + " ====");
                var schemes = _backend.Search(word, "");
                foreach (var scheme in schemes) Console.WriteLine(scheme.Path);
                //Console.WriteLine(scheme.Content);
                Console.WriteLine("==== End Search ====");
            }
        }
    }
}