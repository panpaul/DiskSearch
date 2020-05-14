using System;
using Sentry;
using Sentry.Protocol;

namespace DiskSearch
{
    internal class Program
    {
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
            
            var backend = new Backend("./");
            AppDomain.CurrentDomain.ProcessExit += (s, e) => backend.Close();
            if (args.Length != 1)
            {
                backend.Prompt();
            }
            else
            {
                Console.WriteLine("Indexing...");
                backend.Walk(args[0]);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                backend.Watch(args[0]);
                backend.Prompt();
            }
        }
    }
}