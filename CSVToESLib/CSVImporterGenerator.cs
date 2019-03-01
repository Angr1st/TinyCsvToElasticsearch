﻿using Elasticsearch.Net;
using LamarCompiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;

namespace CSVToESLib
{
    public static class CsvImporterGenerator
    {
        private static int AssemblyNumber = 0;
        private static readonly string[] Usings = new string[] { "System.Threading.Tasks", "Elasticsearch.Net", "System", "System.Linq", "TinyCsvParser.Mapping", "TinyCsvParser" };


        private static ICsvImporter CreateBulkPriceImporterType(string[] fields, int version)
        {
            var generator = new AssemblyGenerator();

            generator.ReferenceAssembly(typeof(ICsvImporter).Assembly);
            generator.ReferenceAssembly(typeof(ParallelQuery<>).Assembly);
            generator.ReferenceAssembly(typeof(CsvMappingResult<>).Assembly);
            generator.ReferenceAssembly(typeof(CsvMapping<>).Assembly);
            generator.ReferenceAssembly(typeof(System.Text.Encoding).Assembly);
            generator.ReferenceAssembly(typeof(ConnectionConfiguration).Assembly);
            generator.ReferenceAssembly(typeof(ElasticLowLevelClient).Assembly);
            generator.ReferenceAssembly(typeof(StringResponse).Assembly);
            generator.ReferenceAssembly(typeof(Task<>).Assembly);
            generator.ReferenceAssembly(typeof(PostData).Assembly);

            var assembly = generator.Generate(x =>
            {
                var writer = new SourceWriter(x)
                .WriteUsingStatements(CreateUsings, Usings)
                .WriteNamespace(CreateNamespace, AssemblyNumber)
                .WriteClass((y) => y.Write(FirstLine(CsvImportClassNames.CsvClient)))
                .WriteMethod(CreateParse)
                .FinishBlock()
                .WriteClass(y => y.Write(FirstLine($"{CsvImportClassNames.CsvBulkPriceMapping} : {CsvImportClassNames.CsvMapping}<{CsvImportClassNames.BulkPrice}>")))
                .WriteMethod(CreatePocoMappingCtor, fields)
                .FinishBlock()
                .WriteClass(y => y.Write(FirstLine(CsvImportClassNames.BulkPrice)))
                .WriteFields(CreateBulkPriceImportPOCOFields, (fields, version))
                .FinishBlock()
                .WriteClass(y => y.Write(FirstLine(CsvImportClassNames.ElasticsearchClient)))
                .WriteFields(y => y.Write("ElasticLowLevelClient ElasticLowLevelClient { get; set; }"))
                .WriteMethod(CreateElasticsearchClientCtor)
                .WriteMethod(CreateAsyncBulkInsert)
                .FinishBlock()
                .WriteClass(y => y.Write(FirstLine(CsvImportClassNames.Index)))
                .WriteFields(CreateIndexFields)
                .WriteMethod(CreateIndexCtor)
                .FinishBlock()
                .WriteClass(y => y.Write(FirstLine(CsvImportClassNames.InnerIndex)))
                .WriteFields(CreateInnerIndexFields)
                .WriteMethod(CreateInnerIndexCtor)
                .FinishBlock(true);

                AssemblyNumber++;
            });

            assembly.GetExportedTypes().Single();
            return assembly.CreateInstance(CsvImportClassNames.CsvImporter) as ICsvImporter;
        }

        private static string FirstLine(string content, bool isClass = true)
        {
            if (isClass)
                content = "class " + content;
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
            sourceWriter.Namespace($"CsvToEsLib{AssemblyNumber}".AddBlock());
        }

        private static void CreateParse(ISourceWriter sourceWriter)
        {
            sourceWriter.Write(FirstLine($"ParallelQuery<CsvMappingResult<{CsvImportClassNames.BulkPrice}>> Parse(string filePath", false));
            sourceWriter.Write("CsvParserOptions csvParserOptions = new CsvParserOptions(true, ';');");
            sourceWriter.Write($"{CsvImportClassNames.CsvBulkPriceMapping} csvMapper = new {CsvImportClassNames.CsvBulkPriceMapping}();");
            sourceWriter.Write($"CsvParser<{CsvImportClassNames.BulkPrice}> csvParser = new CsvParser<{CsvImportClassNames.BulkPrice}>(csvParserOptions, csvMapper);");
            sourceWriter.Write("return csvParser.ReadFromFile(filePath, System.Text.Encoding.UTF8);");
        }

        private static void CreateBulkPriceImportPOCOFields(ISourceWriter sourceWriter, (string[] fields, int version) data)
        {
            foreach (var item in data.fields)
            {
                sourceWriter.Write($"public string {item};)");
            }
            sourceWriter.Write($"public string Version = {data.version};");
        }

        private static void CreatePocoMappingCtor(ISourceWriter sourceWriter, string[] fields)
        {
            sourceWriter.Write(FirstLine($"{CsvImportClassNames.CsvBulkPriceMapping}() : base()", false));
            for (int i = 0; i < fields.Length - 1; i++)
            {
                sourceWriter.Write($"MapProperty({i}, x => x.{fields[i]};");
            }
        }

        private static void CreateElasticsearchClientCtor(ISourceWriter sourceWriter)
        {
            sourceWriter.Write(FirstLine($"{CsvImportClassNames.ElasticsearchClient}(ConnectionConfiguration configuration)",false));
            sourceWriter.Write("ElasticLowLevelClient = new ElasticLowLevelClient(configuration);");
        }

        private static void CreateAsyncBulkInsert(ISourceWriter sourceWriter)
        {
            sourceWriter.Write(FirstLine($"async Task<StringResponse> BulkInsert(ParallelQuery<CsvMappingResult<{CsvImportClassNames.BulkPrice}>> results)", false));
            sourceWriter.Write("return await ElasticLowLevelClient.BulkAsync<StringResponse>(PostData.MultiJson(results.Where(result => result.IsValid).Select(result => new Object[] { new Index(result.Result) }).Aggregate((newValue, oldValue) => oldValue.Concat(newValue).ToArray())));");
        }

        private static void CreateIndexFields(ISourceWriter sourceWriter)
        {
            sourceWriter.Write($"public {CsvImportClassNames.InnerIndex} {CsvImportClassNames.InnerIndex};");
            sourceWriter.Write($"public {CsvImportClassNames.BulkPrice} {CsvImportClassNames.BulkPrice};");
        }

        private static void CreateIndexCtor(ISourceWriter sourceWriter)
        {
            sourceWriter.Write(FirstLine($"{CsvImportClassNames.Index}()", false));
            sourceWriter.Write($"{CsvImportClassNames.BulkPrice} = {CsvImportClassNames.BulkPrice};");
            sourceWriter.Write($"{CsvImportClassNames.InnerIndex} = {CsvImportClassNames.InnerIndex};");
        }

        private static void CreateInnerIndexFields(ISourceWriter sourceWriter)
        {
            sourceWriter.Write($"public {CsvImportClassNames._index} {CsvImportClassNames._index};");
            sourceWriter.Write($"public {CsvImportClassNames._type} {CsvImportClassNames._type};");
        }

        private static void CreateInnerIndexCtor(ISourceWriter sourceWriter)
        {
            sourceWriter.Write(FirstLine($"{CsvImportClassNames.InnerIndex}(string index = \"bulkprices\", string type = \"bulkprice\")", false));
            sourceWriter.Write($"{CsvImportClassNames._index} = index;");
            sourceWriter.Write($"{CsvImportClassNames._type} = type;");
        }
    }
}