using LamarCompiler;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private static ICsvImporter CreatePersonType(string[] fields)
        {
            var generator = new AssemblyGenerator();

            //generator.ReferenceAssembly(typeof().Assembly);

            var assembly = generator.Generate(x =>
            {
                var writer = new SourceWriter(x);
                writer.Write((y) => y.Namespace($"CsvToEs{AssemblyNumber}"), false)
                .Write
                CreateClass(x, "CsvClient", null, CreateParse);
                x.FinishBlock();
                AssemblyNumber++;
            });

            assembly.GetExportedTypes().Single();
            assembly.CreateInstance();
        }

        private static void CreateParse(ISourceWriter sourceWriter)
        {
            sourceWriter.Write("public ParallelQuery<CsvMappingResult<Persion>> Parse(string filePath");
            sourceWriter.Write("CsvParserOptions csvParserOptions = new CsvParserOptions(true, ';');");
            sourceWriter.Write("CSVPersonMapping csvMapper = new CSVPersonMapping();");
            sourceWriter.Write("CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);");
            sourceWriter.Write("return csvParser.ReadFromFile(filePath, System.Text.Encoding.UTF8);");
        }

        private static void CreateBulkPriceImportPOCO(ISourceWriter sourceWriter, string[] fields)
        {
            foreach (var item in fields)
            {
                sourceWriter.Write($"public string {item};)");
            }
        }

        private static void CreatePocoMappingCtor(ISourceWriter sourceWriter, string[] fields)
        {

        }

        private static ISourceWriter CreateClass(ISourceWriter sourceWriter, string className, Type type, Action<ISourceWriter> action)
        {
            return CreateClass(sourceWriter, className, type, new List<Action<ISourceWriter>>() { action });
        }

        private static ISourceWriter CreateClass(ISourceWriter sourceWriter, string className, Type type, List<Action<ISourceWriter>> actions)
        {
            if (type == null)
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
