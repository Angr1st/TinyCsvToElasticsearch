using LamarCompiler;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace CSVToESLib
{
    public class CSVImporter
    {
        private static int AssemblyNumber = 0;
        CSVClient CSVClient = new CSVClient();
        ElasticsearchClient ElasticsearchClient = new ElasticsearchClient();

        public async Task<bool> ImportCSV(string filePath)
        {
            var result = await ElasticsearchClient.BulkInsert(CSVClient.Parse(filePath));
            return result.Success;
        }

    private static Type CreatePersonType(string[] fields)
        {
            var generator = new AssemblyGenerator();

            //generator.ReferenceAssembly(typeof().Assembly);

            var assembly = generator.Generate(x =>
            {
                x.Namespace($"CsvToEs{AssemblyNumber}");
                AssemblyNumber++;

                CreateClass(x, "CsvClient", null, CreateParse);
                x.FinishBlock();
            });
        }

        private static void CreateParse(ISourceWriter sourceWriter)
        {
            sourceWriter.Write("BLOCK:public ParallelQuery<CsvMappingResult<Persion>> Parse(string filePath");
            sourceWriter.Write("CsvParserOptions csvParserOptions = new CsvParserOptions(true, ';');");
            sourceWriter.Write("CSVPersonMapping csvMapper = new CSVPersonMapping();");
            sourceWriter.Write("CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);");
            sourceWriter.Write("return csvParser.ReadFromFile(filePath, System.Text.Encoding.UTF8);");
        }

        private static void CreateBulkPriceImportPOCO(ISourceWriter sourceWriter)
        { }

        private static ISourceWriter CreateClass(ISourceWriter sourceWriter, string className, Type type, Action<ISourceWriter> action)
        {
           return CreateClass(sourceWriter, className, type, new List<Action<ISourceWriter>>() { action });
        }

        private static ISourceWriter CreateClass(ISourceWriter sourceWriter, string className, Type type, List<Action<ISourceWriter>> actions)
        {
            if(type == null)
            {
                sourceWriter.StartClass(className);
            }
            else
            {
                sourceWriter.StartClass(className, type);
            }

            foreach (var action in actions)
            {
                action(sourceWriter);
                sourceWriter.FinishBlock();
            }
            sourceWriter.FinishBlock();
            return sourceWriter;
        }


        public string NameGenerator() => new Guid().ToString();
    }
}
