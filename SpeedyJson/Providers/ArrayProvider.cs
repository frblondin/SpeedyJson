using SpeedyJson.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson.Providers
{
    internal class ArrayProvider : ListProvider
    {
        internal ArrayProvider(Type elementType) : base(elementType.MakeArrayType(1), elementType)
        {
        }

        protected override Expression ListToResult(Expression list)
        {
            var method = ExpressionReflector.GetMethodInfo(() => Enumerable.ToArray<object>(null), true)
                .MakeGenericMethod(ElementType);
            return Expression.Call(method, list);
        }
    }
}
