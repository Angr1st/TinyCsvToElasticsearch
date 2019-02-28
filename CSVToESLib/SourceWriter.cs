using LamarCompiler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSVToESLib
{
    internal class SourceWriter
    {
        private const string BLOCK = "BLOCK:";
        private int IndentationLevel = 0;
        internal ISourceWriter Writer { get; private set; }

        internal SourceWriter(ISourceWriter sourceWriter) => Writer = sourceWriter;

        internal SourceWriter WriteUsingStatements<T>(Action<ISourceWriter, T> action, T parameter)
        {
            action(Writer, parameter);
            return this;
        }

        internal SourceWriter WriteNamespace<T>(Action<ISourceWriter, T> action, T parameter)
        {
            action(Writer, parameter);
            IndentationLevel++;
            return this;
        }

        internal SourceWriter WriteClass<T>(Action<ISourceWriter, T> action, T parameter)
        {
            Writer.Write(BLOCK);
            action(Writer, parameter);

            return this;
        }

        internal SourceWriter WriteClass(Action<ISourceWriter> action)
        {
            Writer.Write(BLOCK);
            action(Writer);

            return this;
        }

        internal SourceWriter WriteMethod<T>(Action<ISourceWriter, T> action, T parameter)
        {
            Writer.Write(BLOCK);
            action(Writer, parameter);
            Writer.FinishBlock();

            return this;
        }

        internal SourceWriter WriteMethod(Action<ISourceWriter> action)
        {
            Writer.Write(BLOCK);
            action(Writer);
            Writer.FinishBlock();

            return this;
        }

        internal SourceWriter FinishBlock(bool complete)
        {
            if (complete)
            {
            for (int i = 0; i < IndentationLevel; i++)
            {
                Writer.FinishBlock();
            }
            }
            else
            {
                Writer.FinishBlock();
            }
            return this;
        }

    }
}
