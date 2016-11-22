using SpeedyJson.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson.Providers
{
    internal class ArrayProvider : OneDimensionContainerProvider
    {
        internal static Expression ListToArray(Expression list, Type elementType)
        {
            var method = ExpressionReflector.GetMethodInfo(() => Enumerable.ToArray<object>(null), true)
                .MakeGenericMethod(elementType);
            return Expression.Call(method, list);
        }

        internal ArrayProvider(Type elementType)
            : base(elementType.MakeArrayType(), elementType, ListToArray)
        {
        }
    }
}
