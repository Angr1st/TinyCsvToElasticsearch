using LamarCompiler;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSVToESLib
{
    internal class SourceWriter
    {
        private const string BLOCK = "BLOCK:";
        internal ISourceWriter Writer { get; private set; }

        internal SourceWriter(ISourceWriter sourceWriter) => Writer = sourceWriter;

        internal SourceWriter Write<T>(Action<ISourceWriter,T> action, T parameter, bool IsBlock = true)
        {
            if (IsBlock)
            {
                Writer.Write(BLOCK);
            }
            action(Writer,parameter);
            if (IsBlock)
            {
                Writer.FinishBlock();
            }
            return this;
        }

        internal SourceWriter Write(Action<ISourceWriter> action, bool IsBlock = true)
        {
            if (IsBlock)
            {
                Writer.Write(BLOCK);
            }
            action(Writer);
            if (IsBlock)
            {
                Writer.FinishBlock();
            }
            return this;
        }


    }
}
