using SpeedyJson.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson.Providers
{
    internal class ImmutableListProvider : ListProvider
    {
        internal ImmutableListProvider(Type elementType, Type immutableType) : base(immutableType, elementType)
        {
        }

        protected override Expression ListToResult(Expression list)
        {
            var nonGenericImmutableListType = Type.Assembly.GetType("System.Collections.Immutable.ImmutableList");
            if (nonGenericImmutableListType == null) throw new JsonReadException("Unable to find the non-generic System.Collections.Immutable.ImmutableList type.");

            var toImmutableListMethod = nonGenericImmutableListType.GetMethod("ToImmutableList");
            if (toImmutableListMethod == null) throw new JsonReadException("Unable to find the ToImmutableList<> generic method in type System.Collections.Immutable.ImmutableList.");

            return Expression.Call(toImmutableListMethod.MakeGenericMethod(ElementType), list);
        }
    }
}
