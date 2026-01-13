using System;
using System.Collections.Generic;

namespace CalciAI.Helpers
{
    public class GenericCompare<T> : IEqualityComparer<T> where T : class
    {
        private Func<T, object> Expr { get; set; }

        public GenericCompare(Func<T, object> expr)
        {
            Expr = expr;
        }

        public bool Equals(T x, T y)
        {
            var first = Expr.Invoke(x);
            var sec = Expr.Invoke(y);
            if (first != null && first.Equals(sec))
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
