using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson.Providers
{
    internal class ListProvider : OneDimensionContainerProvider
    {
        internal static Expression ListToArray(Expression list, Type elementType)
        {
            return list;
        }

        internal ListProvider(Type elementType)
            : base(typeof(List<>).MakeGenericType(elementType), elementType, ListToArray)
        {
        }
    }
}
