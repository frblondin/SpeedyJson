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
    internal class DictionaryProvider : TypeInfoProvider
    {
        internal Type KeyType { get; }
        internal Type ValueType { get; }

        internal DictionaryProvider(Type type, Type keyType, Type valueType) : base(type)
        {
            KeyType = keyType;
            ValueType = valueType;
        }

        protected virtual Expression DictionaryToResult(Expression dictionary)
        {
            return dictionary;
        }

        internal override Expression GetDeserializerExpression(ParameterExpression jsonReaderParam, ParameterExpression settingsParam)
        {
            var resultVar = Expression.Variable(typeof(Dictionary<,>).MakeGenericType(KeyType, ValueType), "result");
            return Expression.Block(
                new[] { resultVar },
                MakeSureBlockStartsWith(jsonReaderParam, '{'),
                Expression.Call(jsonReaderParam, ExpressionReflector.GetMethodInfo<JsonReader>(r => r.ReadNextChar())),
                Expression.Assign(resultVar, Expression.New(resultVar.Type)),
                    CreateValueLoop(jsonReaderParam, settingsParam, resultVar),
                    DictionaryToResult(resultVar));
        }

        private WhileExpression CreateValueLoop(ParameterExpression jsonReaderParam, ParameterExpression settingsParam, ParameterExpression resultVar)
        {
            var breakLabel = Expression.Label("break");
            var continueLabel = Expression.Label("continue");

            return new WhileExpression(
                Expression.NotEqual(
                    Expression.Call(jsonReaderParam, nameof(JsonReader.CurrentChar), Type.EmptyTypes),
                    Expression.Constant('}')),
                CreateValueLoopBlock(jsonReaderParam, settingsParam, resultVar, breakLabel, continueLabel),
                breakLabel,
                continueLabel);
        }

        private Expression CreateValueLoopBlock(ParameterExpression jsonReaderParam, ParameterExpression settingsParam, ParameterExpression resultVar, LabelTarget breakTarget, LabelTarget continueTarget)
        {
            var keyInfoProvider = GetOrCreate(KeyType);
            var valueInfoProvider = GetOrCreate(ValueType);

            var keyVar = Expression.Variable(KeyType, "key");
            var valueVar = Expression.Variable(ValueType, "value");
            var key = keyInfoProvider.GetDeserializerExpression(jsonReaderParam, settingsParam);
            var value = valueInfoProvider.GetDeserializerExpression(jsonReaderParam, settingsParam);
            return Expression.Block(
                new[] { keyVar, valueVar },
                Expression.Assign(
                    keyVar,
                    key.Type == KeyType ? key : Expression.Convert(key, KeyType)),
                Expression.Call(jsonReaderParam, nameof(JsonReader.ReadAssignment), Type.EmptyTypes),
                Expression.Assign(
                    valueVar,
                    value.Type == ValueType ? value : Expression.Convert(value, ValueType)),
                Expression.Call(resultVar, "Add", Type.EmptyTypes, keyVar, valueVar),
                ManageEndOfBlockOrNextElement(jsonReaderParam, breakTarget, continueTarget, '}'));
        }
    }
}
