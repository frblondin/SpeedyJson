using SpeedyJson.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson.Providers
{
    internal class ImmutableListProvider : OneDimensionContainerProvider
    {
        internal static Expression ListToImmutable(Expression list, Type elementType, Type immutableType)
        {
            var nonGenericImmutableListType = immutableType.Assembly.GetType("System.Collections.Immutable.ImmutableList");
            if (nonGenericImmutableListType == null) throw new JsonReadException("Unable to find the non-generic System.Collections.Immutable.ImmutableList type.");

            var toImmutableListMethod = nonGenericImmutableListType.GetMethod("ToImmutableList");
            if (toImmutableListMethod == null) throw new JsonReadException("Unable to find the ToImmutableList<> generic method in type System.Collections.Immutable.ImmutableList.");

            return Expression.Call(toImmutableListMethod.MakeGenericMethod(elementType), list);
        }

        internal ImmutableListProvider(Type elementType, Type immutableType)
            : base(elementType.MakeArrayType(), elementType, (l, e) => ListToImmutable(l, e, immutableType))
        {
        }
    }
}
