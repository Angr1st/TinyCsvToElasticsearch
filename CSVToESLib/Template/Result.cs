using System;
using System.Collections.Generic;
using System.Text;

namespace CSVToESLib.Template
{
    public class Result<T, U>
    {
        public T Left { get; private set; }

        public U Right { get; private set; }

        public bool IsSuccess
        {
            get => Left != null ? true : false;
        }

        public Result(T left)
        {
            Left = left;
        }

        public Result(U right)
        {
            Right = right;
        }
    }
}
