using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Reflection;

namespace CSVToESLib
{
    public class CSVImporter
    {
        CSVClient CSVClient = new CSVClient();
        ElasticsearchClient ElasticsearchClient = new ElasticsearchClient();

        public async Task<bool> ImportCSV(string filePath)
        {
            var result = await ElasticsearchClient.BulkInsert(CSVClient.Parse(filePath));
            return result.Success;
        }

        public Type CreateIL(string[] fields)
        {
            AssemblyName assemblyName = new AssemblyName(NameGenerator());
            var assembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);

            var module = assembly.DefineDynamicModule("dynamicModule");

            var personType = module.DefineType("Person", TypeAttributes.Public);

            foreach (var item in fields)
            {
                personType.DefineField(item, typeof(string), FieldAttributes.Public);
            }


            return personType.CreateType();

        }

        public string NameGenerator() => new Guid().ToString();
    }
}
