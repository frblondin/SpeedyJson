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
    internal class GuidProvider : StructInfoProvider
    {
        internal GuidProvider(bool nullable) : base(nullable ? typeof(Guid?) : typeof(Guid)) { }

        protected override Expression GetRawStringValue(ParameterExpression jsonReader)
        {
            return Expression.Call(
                jsonReader,
                ExpressionReflector.GetMethodInfo((JsonReader r) => r.ReadString()));
        }

        protected override Expression ParseRawValue(Expression rawValue)
        {
            return Expression.Call(
                ExpressionReflector.GetMethodInfo(() => Guid.Parse("")),
                rawValue);
        }
    }
}
