using System;


namespace DiskSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Database.Index idx = new Database.Index("./index/");
            for (var i = 0; i < 100; i++)
            {
                var doc = Database.Index.GenerateDocument( "path "+i,"content "+i);
                idx.Add(doc);
            }

            var r = idx.Search("content");
            foreach (var s in r)
            {
                Console.WriteLine(s.Path);
            }

            Console.ReadKey();
            idx.Close();
        }
    }
}
