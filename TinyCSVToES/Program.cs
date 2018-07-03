using System;
using CSVToESLib;
using static System.Console;

namespace TinyCSVToES
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] nameArray = new string[] { "FirstFüeld", "SecondField" };
            CSVToESLib.CSVImporter cSVImporter = new CSVImporter();
            var newType = cSVImporter.CreateIL(nameArray);
            dynamic helloWorld = Activator.CreateInstance(newType);
            helloWorld.FirstFüeld = "Some Message!";
            helloWorld.SecondField = "Some Other Message!";
            WriteLine($"{helloWorld.FirstFüeld} {Environment.NewLine} {helloWorld.SecondField})");

            //var newType2 = cSVImporter.CreateIL();
            //dynamic dynamic = Activator.CreateInstance(newType2);
            //dynamic.FieldOne = "Lel this works no assemblyname conflict";
            //WriteLine(dynamic.FieldOne);
            ReadLine();
        }
    }
}
