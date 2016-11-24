using SpeedyJson.Linq;
using SpeedyJson.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson.Providers
{
    internal class ListProvider : TypeInfoProvider
    {
        internal Type ElementType { get; }

        internal ListProvider(Type type, Type elementType) : base(type)
        {
            ElementType = elementType;
        }

        protected virtual Expression ListToResult(Expression list)
        {
            return list;
        }

        internal override Expression GetDeserializerExpression(ParameterExpression jsonReaderParam, ParameterExpression settingsParam)
        {
            var resultVar = Expression.Variable(typeof(List<>).MakeGenericType(ElementType), "result");
            return Expression.Block(
                new[] { resultVar },
                MakeSureBlockStartsWith(jsonReaderParam, '['),
                Expression.Call(jsonReaderParam, ExpressionReflector.GetMethodInfo<JsonReader>(r => r.ReadNextChar())),
                Expression.Assign(resultVar, Expression.New(resultVar.Type)),
                    CreateValueLoop(jsonReaderParam, settingsParam, resultVar),
                    ListToResult(resultVar));
        }

        private WhileExpression CreateValueLoop(ParameterExpression jsonReaderParam, ParameterExpression settingsParam, ParameterExpression resultVar)
        {
            var breakLabel = Expression.Label("break");
            var continueLabel = Expression.Label("continue");

            return new WhileExpression(
                Expression.NotEqual(
                    Expression.Call(jsonReaderParam, nameof(JsonReader.CurrentChar), Type.EmptyTypes),
                    Expression.Constant(']')),
                CreateValueLoopBlock(jsonReaderParam, settingsParam, resultVar, breakLabel, continueLabel),
                breakLabel,
                continueLabel);
        }

        private Expression CreateValueLoopBlock(ParameterExpression jsonReaderParam, ParameterExpression settingsParam, ParameterExpression resultVar, LabelTarget breakTarget, LabelTarget continueTarget)
        {
            var info = GetOrCreate(ElementType);
            var value = info.GetDeserializerExpression(jsonReaderParam, settingsParam);
            return Expression.Block(
                Expression.Call(
                    resultVar,
                    nameof(List<object>.Add),
                    Type.EmptyTypes,
                    value.Type == ElementType ? value : Expression.Convert(value, ElementType)),
                ManageEndOfBlockOrNextElement(jsonReaderParam, breakTarget, continueTarget, ']'));
        }
    }
}
