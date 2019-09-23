using System;
using System.Collections.Generic;
using System.Text;

namespace CSVToESLib.Types
{
    public class Result<T, U>
    {
        public T Left { get; private set; }

        public U Right { get; private set; }

        public bool IsSuccess
        {
            get => IsSucceded(Left,Right);
        }

        public Result(T left)
        {
            Left = left;
        }

        public Result(U right)
        {
            Right = right;
        }

        private static bool IsSucceded(T left, U error)
        {
            return !CheckDefaultOrNull(left) && CheckDefaultOrNull(error);
        }

        private static bool CheckDefaultOrNull<Y>(Y variable)
        {
            if (IsNullable(typeof(Y)))
            {
                return variable == null;
            }

            return default(Y).Equals(variable);
        }

        private static bool IsNullable(Type type) => Nullable.GetUnderlyingType(type) != null || type.IsClass || type.IsInterface;
    }
}
