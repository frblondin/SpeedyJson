using EnumsNET;
using SpeedyJson.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson.Providers
{
    internal class EnumProvider : TypeInfoProvider
    {
        internal EnumProvider(Type type) : base(type) { }

        internal override Expression GetDeserializerExpression(ParameterExpression jsonReader, ParameterExpression settings)
        {
            return Expression.Call(
                ExpressionReflector.GetMethodInfo(() => Enums.Parse<StringComparison>(""), true).MakeGenericMethod(Type),
                Expression.Call(
                    jsonReader,
                    ExpressionReflector.GetMethodInfo((JsonReader r) => r.ReadUnquoted())));
        }
    }
}
