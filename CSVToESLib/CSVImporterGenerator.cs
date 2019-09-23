using Elasticsearch.Net;
using LamarCodeGeneration;
using LamarCompiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;
using CSVToESLib.Extensions;
using CSVToESLib.Constants;
using CSVToESLib.Interfaces;
using SourceWriter = CSVToESLib.Types.SourceWriter;
using System.Text;
using Nest;
using CSVToESLib.Types;

namespace CSVToESLib
{
    public static class CsvImporterGenerator
    {
        private static int AssemblyNumber = 0;
        private static readonly string[] Usings = new string[] { "System.Threading.Tasks", "Nest", "System", "System.Linq", "TinyCsvParser.Mapping", "TinyCsvParser", "CSVToESLib.Interfaces", "CSVToESLib.Types", "System.Collections.Generic" };
        private static readonly Dictionary<string[], ICsvImporter> ImplementationStore = new Dictionary<string[], ICsvImporter>();
        private static readonly AssemblyGenerator Generator = new AssemblyGenerator();

        public static ICsvImporter CreateICsvImporterType(string[] fields, TypeNames typeNames, int chunkSize = 5000) 
        {
            if (ImplementationStore.TryGetValue(fields, out var csvImporter))
            {
                return csvImporter;
            }

            Generator.ReferenceAssembly(typeof(ICsvImporter).Assembly);
            Generator.ReferenceAssembly(typeof(ParallelQuery<>).Assembly);
            Generator.ReferenceAssembly(typeof(CsvMappingResult<>).Assembly);
            Generator.ReferenceAssembly(typeof(CsvMapping<>).Assembly);
            Generator.ReferenceAssembly(typeof(System.Text.Encoding).Assembly);
            Generator.ReferenceAssembly(typeof(Task<>).Assembly);
            Generator.ReferenceAssembly(typeof(ElasticClient).Assembly);

            var assembly = Generator.Generate(x =>
            {
                var writer = new SourceWriter(x)
                .WriteUsingStatements(CreateUsings, Usings)
                .WriteNamespace(CreateNamespace, AssemblyNumber)
                .CreateCsvClient(typeNames)
                .CreateCsvBulkPriceMapping(fields, typeNames)
                .CreateBulkPrice(fields, typeNames)
                .CreateElasticsearchClient()
                .CreateCsvImporter(chunkSize);

                AssemblyNumber++;
            });

            var types = assembly.GetExportedTypes();
            var type = types.FirstOrDefault(z => z.Name == CsvImportClassNames.CsvImporter);
            var csvImporter1 = type != null ? Activator.CreateInstance(type) as ICsvImporter : null;
            if (csvImporter1 != null)
            {
                ImplementationStore.Add(fields, csvImporter1);
            }

            return csvImporter1;
        }

        private static SourceWriter CreateCsvClient(this SourceWriter sourceWriter, TypeNames typeNames)
        {
            return sourceWriter.WriteClass((y) => y.Write(FirstLine(CsvImportClassNames.CsvClient)))
                .WriteMethod(CreateParse, typeNames)
                .FinishBlock();
        }

        private static SourceWriter CreateBulkPrice(this SourceWriter sourceWriter, string[] fields, TypeNames typeNames)
        {
            return sourceWriter.WriteClass(y => y.Write(FirstLine(typeNames.TypeName)))
                .WriteFields(CreatePOCOFields, fields)
                .WriteMethod(CreateToStringOverride, fields)
                .FinishBlock();
        }

        private static SourceWriter CreateElasticsearchClient(this SourceWriter sourceWriter)
        {
            return sourceWriter.WriteClass(y => y.Write(FirstLine(CsvImportClassNames.ElasticsearchClient)))
                .WriteFields(y => y.Write("private ElasticClient _elasticClient;"))
                .WriteMethod(CreateElasticsearchClientCtor,true)
                .WriteMethod(CreateAsyncBulkInsert)
                .FinishBlock();
        }

        private static SourceWriter CreateCsvImporter(this SourceWriter sourceWriter, int chunkSize)
        {
            return sourceWriter.WriteClass(y => y.Write(FirstLine($"{CsvImportClassNames.CsvImporter} : {CsvImportClassNames.ICsvImporter}")))
                .WriteMethod(CreateImportCsv, chunkSize)
                .FinishBlock(true); ;
        }

        private static SourceWriter CreateCsvBulkPriceMapping(this SourceWriter sourceWriter, string[] fields, TypeNames typeNames)
        {
            return sourceWriter.WriteClass(y => y.Write(FirstLine($"{typeNames.TypeMappingName} : {CsvImportClassNames.CsvMapping}<{typeNames.TypeName}>")))
                 .WriteMethod(CreatePocoMappingCtor, fields, typeNames)
                 .FinishBlock();
        }

        private static string FirstLine(string content, bool isClass = true)
        {
            if (isClass)
                content = "public class " + content;
            else
                content = "public " + content;

            return content.AddBlock();
        }

        private static void CreateUsings(ISourceWriter sourceWriter, string[] usings)
        {
            foreach (var item in usings)
            {
                sourceWriter.UsingNamespace(item);
            }
        }

        private static void CreateNamespace(ISourceWriter sourceWriter, int assemblyNumber)
        {
            sourceWriter.Namespace($"CsvToEsLib{AssemblyNumber}");
        }

        private static void CreateParse(ISourceWriter sourceWriter, TypeNames typeNames)
        {
            sourceWriter.Write(FirstLine($"ParallelQuery<CsvMappingResult<{typeNames.TypeName}>> Parse(string filePath)", false));
            sourceWriter.Write("CsvParserOptions csvParserOptions = new CsvParserOptions(true, ';');");
            sourceWriter.Write($"{typeNames.TypeMappingName} csvMapper = new {typeNames.TypeMappingName}();");
            sourceWriter.Write($"CsvParser<{typeNames.TypeName}> csvParser = new CsvParser<{typeNames.TypeName}>(csvParserOptions, csvMapper);");
            sourceWriter.Write("return csvParser.ReadFromFile(filePath, System.Text.Encoding.UTF8);");
        }

        private static void CreatePOCOFields(ISourceWriter sourceWriter, string[] fields)
        {
            foreach (var item in fields)
            {
                sourceWriter.Write($"public string {item} {{ get; set; }}");
            }
            sourceWriter.Write($"public int Version;");
        }

        private static void CreateToStringOverride(ISourceWriter sourceWriter, string[] fields)
        {
            sourceWriter.Write(FirstLine($"override string ToString()",false));
            var stringBuild = new StringBuilder("return $\"{{");
            foreach (var item in fields)
            {
                stringBuild.Append($"\\\"{item}\\\":\\\"{{{item}}}\\\",");
            }
            stringBuild.Append("\\\"Version\\\":\\\"{Version}\\\"}}\";");
            sourceWriter.Write(stringBuild.ToString());
        }

        private static void CreatePocoMappingCtor(ISourceWriter sourceWriter, string[] fields, TypeNames typeNames)
        {
            sourceWriter.Write(FirstLine($"{typeNames.TypeMappingName}() : base()", false));
            for (int i = 0; i < fields.Length; i++)
            {
                sourceWriter.Write($"MapProperty({i}, x => x.{fields[i]});");
            }
        }

        private static void CreateElasticsearchClientCtor(ISourceWriter sourceWriter)
        {
            sourceWriter.Write($"public {CsvImportClassNames.ElasticsearchClient}(IConnectionSettingsValues settings) => _elasticClient = new ElasticClient(settings);");
        }

        private static void CreateAsyncBulkInsert(ISourceWriter sourceWriter)
        {
            sourceWriter.Write(FirstLine($"async Task<Result<int, Exception>> BulkInsert<T>(ElasticsearchConnection con, IEnumerable<T> results) where T : class", false));
            sourceWriter.Write("try {");
            sourceWriter.Write("var bulkAllObservable = _elasticClient.BulkAll(results, b => b.Index(IndexName.From<T>()).Type(TypeName.From<T>()).RefreshOnCompleted().MaxDegreeOfParallelism(Environment.ProcessorCount).Size(5000));");
            sourceWriter.Write("bulkAllObservable.Subscribe(con.BulkAllObserver);");
            sourceWriter.Write("await con.WaitForCompletion();");
            sourceWriter.Write("return new Result<int, Exception>(con.RecordCount);");
            sourceWriter.Write("} catch (Exception e) {");
            sourceWriter.Write("return new Result<int, Exception>(e); }");
        }

        private static void CreateImportCsv(ISourceWriter sourceWriter, int chunkSize)
        {
            sourceWriter.Write(FirstLine($"async Task<Result<int, Exception>> ImportCsv(IConnectionSettingsValues settings, string filePath, int version, int chunkSize)", false));
            sourceWriter.Write($"var {CsvImportClassNames.CsvClient} = new {CsvImportClassNames.CsvClient}();");
            sourceWriter.Write($"var {CsvImportClassNames.ElasticsearchClient} = new {CsvImportClassNames.ElasticsearchClient}(settings);");
            sourceWriter.Write($"var results = {CsvImportClassNames.CsvClient}.Parse(filePath).Where(r => r.IsValid).Select(r => {{ r.Result.Version = version; return r.Result; }}); ");
            sourceWriter.Write($"var elasticsearchCon = new {nameof(ElasticsearchConnection)}({nameof(chunkSize)});");
            sourceWriter.Write($"return await {CsvImportClassNames.ElasticsearchClient}.BulkInsert(elasticsearchCon, results);");
        }
    }
}
