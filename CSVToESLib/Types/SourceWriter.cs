using LamarCodeGeneration;
using System;

namespace CSVToESLib.Types
{
    internal class SourceWriter
    {
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
            action(Writer, parameter);
            IndentationLevel++;

            return this;
        }

        internal SourceWriter WriteClass(Action<ISourceWriter> action)
        {
            action(Writer);
            IndentationLevel++;

            return this;
        }

        internal SourceWriter WriteFields<T>(Action<ISourceWriter, T> action, T parameter)
        {
            action(Writer, parameter);
            return this;
        }

        internal SourceWriter WriteFields(Action<ISourceWriter> action)
        {
            action(Writer);
            return this;
        }

        internal SourceWriter WriteMethod<T>(Action<ISourceWriter, T> action, T parameter)
        {
            action(Writer, parameter);
            Writer.FinishBlock();

            return this;
        }

        internal SourceWriter WriteMethod(Action<ISourceWriter> action, bool expressionBody = false)
        {
            action(Writer);
            if (!expressionBody)
            {
                Writer.FinishBlock();
            }


            return this;
        }

        internal SourceWriter WriteTry(Action<ISourceWriter> action)
        {
            action(Writer);
            Writer.FinishBlock();

            return this;
        }

        internal SourceWriter FinishBlock(bool complete = false)
        {
            if (IndentationLevel < 1)
                return this;


            if (complete)
            {
                for (int i = 0; i < IndentationLevel; i++)
                {
                    Writer.FinishBlock();
                }
                IndentationLevel = 0;
            }
            else
            {
                Writer.FinishBlock();
                IndentationLevel--;
            }
            return this;
        }

    }
}
