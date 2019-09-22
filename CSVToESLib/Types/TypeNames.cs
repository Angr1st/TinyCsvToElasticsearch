using System;
using System.Collections.Generic;
using System.Text;

namespace CSVToESLib.Types
{
    public class TypeNames
    {
        public string TypeName { get; }

        public string TypeMappingName { get; }

        public TypeNames(string typeName)
        {
            TypeName = typeName;
            TypeMappingName = $"Csv{TypeName}Mapping";
        }
    }
}
