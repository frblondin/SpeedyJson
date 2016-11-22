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
    internal class IntProvider : TypeInfoProvider
    {
        internal IntProvider() : base(typeof(int)) { }

        internal override Expression GetDeserializerExpression(ParameterExpression jsonReader, ParameterExpression settings)
        {
            return Expression.Call(
                ExpressionReflector.GetMethodInfo(() => Convert.ToInt32("", null)),
                Expression.Call(
                    jsonReader,
                    ExpressionReflector.GetMethodInfo((JsonReader r) => r.ReadString())),
                Expression.Constant(CultureInfo.InvariantCulture));
        }
    }
}
