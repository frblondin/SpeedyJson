using SpeedyJson.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson.Providers
{
    internal abstract class StructInfoProvider : TypeInfoProvider
    {
        internal StructInfoProvider(Type type) : base(type) { }

        internal override Expression GetDeserializerExpression(ParameterExpression jsonReader, ParameterExpression settings)
        {
            var rawValue = GetRawStringValue(jsonReader);
            if (!Type.IsGenericType || Type.GetGenericTypeDefinition() != typeof(Nullable<>))
            {
                return ParseRawValue(rawValue);
            }
            else
            {
                var stringValue = Expression.Variable(typeof(string));
                return Expression.Block(
                    new[] { stringValue },
                    Expression.Assign(stringValue, rawValue),
                    Expression.Condition(
                        Expression.Call(
                            ExpressionReflector.GetMethodInfo(() => string.IsNullOrWhiteSpace("")),
                            stringValue),
                        Expression.Default(Type),
                        Expression.Convert(ParseRawValue(stringValue), Type)));
            }
        }

        protected abstract Expression ParseRawValue(Expression rawValue);

        protected abstract Expression GetRawStringValue(ParameterExpression jsonReader);
    }
}