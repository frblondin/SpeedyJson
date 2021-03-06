﻿using SpeedyJson.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson.Providers
{
    internal class IntProvider : StructInfoProvider
    {
        internal IntProvider(bool nullable) : base(nullable ? typeof(int?) : typeof(int)) { }

        protected override Expression GetRawStringValue(ParameterExpression jsonReader)
        {
            return Expression.Call(
                jsonReader,
                ExpressionReflector.GetMethodInfo((JsonReader r) => r.ReadUnquoted()));
        }

        protected override Expression ParseRawValue(Expression rawValue)
        {
            return Expression.Call(
                ExpressionReflector.GetMethodInfo(() => Convert.ToInt32("", null)),
                rawValue,
                Expression.Constant(CultureInfo.InvariantCulture));
        }
    }
}
