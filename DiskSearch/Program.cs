﻿using System;

namespace DiskSearch
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var walker = new Walker("./index/");
            walker.Walk("C:\\development\\source");
        }
    }
}