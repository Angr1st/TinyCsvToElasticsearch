using System;
using System.Collections.Generic;
using System.Text;

namespace CSVToESLib.Types
{
    public class TypeNames
    {
        public string TypeName { get; }

        public string TypeMappingName { get; }

        public TypeNames(string typeName, string typeMappingName)
        {
            TypeName = typeName;
            TypeMappingName = typeMappingName;
        }
    }
}
