using System;

namespace DiskSearch
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var backend = new Backend("./index/");
            AppDomain.CurrentDomain.ProcessExit += (s, e) => backend.Close();
            if (args.Length != 1)
            {
                backend.Prompt();
            }
            else
            {
                Console.WriteLine("Indexing...");
                backend.Walk(args[0]);
                backend.Watch(args[0]);
                backend.Prompt();
            }
        }
    }
}