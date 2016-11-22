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
    internal abstract class OneDimensionContainerProvider : TypeInfoProvider
    {
        internal delegate Expression ListToContainer(Expression list, Type elementType);

        internal Type ElementType { get; }
        internal ListToContainer ListToResult { get; }

        internal OneDimensionContainerProvider(Type type, Type elementType, ListToContainer listToResult) : base(type)
        {
            ElementType = elementType;
            ListToResult = listToResult;
        }

        internal override Expression GetDeserializerExpression(ParameterExpression jsonReaderParam, ParameterExpression settingsParam)
        {
            var resultVar = Expression.Variable(typeof(List<>).MakeGenericType(ElementType), "result");
            var info = GetOrCreate(ElementType);

            return Expression.Block(
                new[] { resultVar },
                MakeSureBlockStartsWith(jsonReaderParam, '['),
                Expression.Call(jsonReaderParam, ExpressionReflector.GetMethodInfo<JsonReader>(r => r.ReadNextChar())),
                Expression.Assign(resultVar, Expression.New(resultVar.Type)),
                    CreateValueLoop(jsonReaderParam, settingsParam, resultVar, info),
                    ListToResult(resultVar, ElementType));
        }

        private WhileExpression CreateValueLoop(ParameterExpression jsonReaderParam, ParameterExpression settingsParam, ParameterExpression resultVar, TypeInfoProvider info)
        {
            var breakLabel = Expression.Label("break");
            var continueLabel = Expression.Label("continue");

            return new WhileExpression(
                Expression.NotEqual(
                    Expression.Call(jsonReaderParam, nameof(JsonReader.CurrentChar), Type.EmptyTypes),
                    Expression.Constant(']')),
                CreateValueLoopBlock(jsonReaderParam, settingsParam, resultVar, info, breakLabel, continueLabel),
                breakLabel,
                continueLabel);
        }

        private Expression CreateValueLoopBlock(ParameterExpression jsonReaderParam, ParameterExpression settingsParam, ParameterExpression resultVar, TypeInfoProvider info, LabelTarget breakTarget, LabelTarget continueTarget)
        {
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
