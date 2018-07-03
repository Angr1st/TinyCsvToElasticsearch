using System;
using CSVToESLib;
using static System.Console;

namespace TinyCSVToES
{
    class Program
    {
        static void Main(string[] args)
        {
            CSVToESLib.CSVImporter cSVImporter = new CSVImporter();
            var newType = cSVImporter.CreateIL();
            Object helloWorld = Activator.CreateInstance(newType);
            helloWorld
            WriteLine("This is a test message.");
            ReadLine();
        }
    }
}
