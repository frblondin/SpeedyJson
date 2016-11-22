using SpeedyJson.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson.Providers
{
    internal class StringProvider : TypeInfoProvider
    {
        internal StringProvider() : base(typeof(string)) { }

        internal override Expression GetDeserializerExpression(ParameterExpression jsonReader, ParameterExpression settings)
        {
            return Expression.Call(
                jsonReader,
                ExpressionReflector.GetMethodInfo((JsonReader r) => r.ReadString()));
        }
    }
}
