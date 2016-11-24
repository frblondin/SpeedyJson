using SpeedyJson.Linq;
using SpeedyJson.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson.Providers
{
    internal class TypeInfoProvider
    {
        static TypeInfoProvider()
        {
            var jsonReader = Expression.Parameter(typeof(JsonReader));
            var settings = Expression.Parameter(typeof(JsonSettings));

            _cache[typeof(string)] = new StringProvider();
            _cache[typeof(int)] = new IntProvider(false);
            _cache[typeof(int?)] = new IntProvider(true);
            _cache[typeof(Guid)] = new GuidProvider(false);
            _cache[typeof(Guid?)] = new GuidProvider(true);
        }

        private static readonly IDictionary<Type, TypeInfoProvider> _cache = new Dictionary<Type, TypeInfoProvider>();
        internal static TypeInfoProvider GetOrCreate(Type type)
        {
            lock (_cache)
            {
                TypeInfoProvider result;
                if (!_cache.TryGetValue(type, out result)) _cache[type] = result = CreateInfoProvider(type);
                return result;
            }
        }

        private static TypeInfoProvider CreateInfoProvider(Type type)
        {
            if (type.IsArray)
            {
                if (type.GetArrayRank() != 1) throw new NotImplementedException("Arrays of rank different than 1 is not yet supported.");
                return new ArrayProvider(type.GetElementType());
            }
            if (type.IsGenericType &&
                (type.GetGenericTypeDefinition() == typeof(List<>) ||
                type.GetGenericTypeDefinition() == typeof(IList<>)))
            {
                return new ListProvider(type, type.GetGenericArguments()[0]);
            }
            if (type.IsGenericType &&
                (type.GetGenericTypeDefinition().FullName == "System.Collections.Immutable.ImmutableList`1" ||
                type.GetGenericTypeDefinition().FullName == "System.Collections.Immutable.IImmutableList`1"))
            {
                return new ImmutableListProvider(type.GetGenericArguments()[0], type);
            }
            if (type.IsEnum) return new EnumProvider(type);
            if (type.IsGenericType &&
                (type.GetGenericTypeDefinition() == typeof(Dictionary<,>) ||
                type.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
            {
                return new DictionaryProvider(type, type.GetGenericArguments()[0], type.GetGenericArguments()[1]);
            }

            return new TypeInfoProvider(type);
        }

        internal Type Type { get; }
        private readonly Lazy<Expression<Func<JsonReader, JsonSettings, object>>> _deserializerExpression;
        internal Expression<Func<JsonReader, JsonSettings, object>> DeserializerExpression => _deserializerExpression.Value;

        private readonly Lazy<Func<JsonReader, JsonSettings, object>> _deserializer;
        internal Func<JsonReader, JsonSettings, object> Deserializer => _deserializer.Value;

        protected TypeInfoProvider(Type type)
        {
            Type = type;
            _deserializerExpression = new Lazy<Expression<Func<JsonReader, JsonSettings, object>>>(CreateDeserializer);
            _deserializer = new Lazy<Func<JsonReader, JsonSettings, object>>(() => DeserializerExpression.Compile());
        }

        private Expression<Func<JsonReader, JsonSettings, object>> CreateDeserializer()
        {
            var jsonReaderParam = Expression.Parameter(typeof(JsonReader), "jsonReader");
            var settingsParam = Expression.Parameter(typeof(JsonSettings), "settings");
            var expression = GetDeserializerExpression(jsonReaderParam, settingsParam);
            if (expression.Type != typeof(object)) expression = Expression.Convert(expression, typeof(object));
            var lambda = Expression.Lambda<Func<JsonReader, JsonSettings, object>>(
                expression,
                true,
                jsonReaderParam, settingsParam);
            return lambda;
        }

        internal virtual Expression GetDeserializerExpression(ParameterExpression jsonReaderParam, ParameterExpression settingsParam)
        {
            var resultVar = Expression.Variable(Type, "result");
            var memberNameVar = Expression.Variable(typeof(StringChunk), "memberName");
            var comparerVar = Expression.Variable(typeof(IEqualityComparer<StringChunk>), "comparer");

            var breakLabel = Expression.Label("break");
            var continueLabel = Expression.Label("continue");

            return Expression.Block(
                new[] { resultVar, memberNameVar, comparerVar },
                MakeSureBlockStartsWith(jsonReaderParam, '{'),
                Expression.Call(jsonReaderParam, ExpressionReflector.GetMethodInfo<JsonReader>(r => r.ReadNextChar())),
                Expression.Assign(resultVar, CreateParameterLessFactory()),
                    Expression.Assign(comparerVar, Expression.Property(settingsParam, ExpressionReflector.GetPropertyInfo((JsonSettings s) => s.Comparer))),
                    new WhileExpression(
                        Expression.Equal(
                            Expression.Call(jsonReaderParam, nameof(JsonReader.CurrentChar), Type.EmptyTypes),
                            Expression.Constant('"')),
                        GetMemberExpressionLoopBlock(jsonReaderParam, jsonReaderParam, resultVar, memberNameVar, comparerVar, settingsParam, breakLabel, continueLabel),
                        breakLabel,
                        continueLabel),
                    resultVar);
        }

        private Expression CreateParameterLessFactory()
        {
            var constructor = Type.GetConstructor(BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            return constructor != null ?
                (Expression)Expression.New(constructor) :
                Expression.Convert(
                    Expression.Call(
                        ExpressionReflector.GetMethodInfo(() => FormatterServices.GetUninitializedObject(null)),
                        Expression.Constant(Type)),
                    Type);
        }

        protected ConditionalExpression MakeSureBlockStartsWith(ParameterExpression jsonReaderParam, char c)
        {
            return Expression.IfThen(
                Expression.NotEqual(
                    Expression.Call(jsonReaderParam, ExpressionReflector.GetMethodInfo<JsonReader>(r => r.CurrentChar())),
                    Expression.Constant(c)),
                Expression.Throw(
                    Expression.New(
                        ExpressionReflector.GetConstructorInfo(() => new JsonReadException(null, null)),
                        Expression.Constant($"Input is not a valid Json: '{c}' expected."),
                        Expression.Default(typeof(Exception)))));
        }

        private BlockExpression GetMemberExpressionLoopBlock(ParameterExpression jsonReaderParam, ParameterExpression jsonReaderVar, ParameterExpression resultVar, ParameterExpression memberNameVar, ParameterExpression comparerVar, ParameterExpression settingsParam, LabelTarget breakTarget, LabelTarget continueTarget)
        {
            var expressions = new List<Expression>();
            expressions.Add(Expression.Assign(
                memberNameVar,
                Expression.Call(
                    jsonReaderParam,
                    ExpressionReflector.GetMethodInfo<JsonReader>(r => r.ReadMemberName()))));
            expressions.Add(Expression.Call(jsonReaderVar, nameof(JsonReader.ReadAssignment), Type.EmptyTypes));
            var properties = Type.GetProperties().Where(p => p.GetSetMethod(true) != null);
            using (var enumerator = properties.GetEnumerator())
            {
                var updates = GenerateMemberSetters(jsonReaderParam, resultVar, memberNameVar, comparerVar, settingsParam, enumerator);
                expressions.Add(updates);
            }
            expressions.Add(ManageEndOfBlockOrNextElement(jsonReaderVar, breakTarget, continueTarget, '}'));
            return Expression.Block(expressions);
        }

        private static Expression GenerateMemberSetters(ParameterExpression jsonReaderParam, ParameterExpression resultVar, ParameterExpression memberNameVar, ParameterExpression comparerVar, ParameterExpression settingsParam, IEnumerator<PropertyInfo> enumerator)
        {
            if (enumerator.MoveNext())
            {
                var p = enumerator.Current;
                var setter = p.GetSetMethod(true);
                return Expression.IfThenElse(
                    Expression.Call(
                        comparerVar,
                        ExpressionReflector.GetMethodInfo<IEqualityComparer<StringChunk>>(c => c.Equals(default(StringChunk), default(StringChunk))),
                        memberNameVar,
                        Expression.Constant(new StringChunk(p.Name))),
                    Expression.Call(
                        resultVar,
                        setter,
                        GetMemberFactory(jsonReaderParam, settingsParam, p)),
                    GenerateMemberSetters(jsonReaderParam, resultVar, memberNameVar, comparerVar, settingsParam, enumerator));
            }
            else
            {
                // TODO
                return Expression.Default(typeof(void));
            }
        }

        protected Expression ManageEndOfBlockOrNextElement(ParameterExpression jsonReaderVar, LabelTarget breakTarget, LabelTarget continueTarget, char endOfBlock)
        {
            var nextCharVar = Expression.Variable(typeof(char), "nextChar");
            return Expression.Block(
                new[] { nextCharVar },
                Expression.Assign(nextCharVar, Expression.Call(jsonReaderVar, nameof(JsonReader.CurrentChar), Type.EmptyTypes)),
                Expression.Call(jsonReaderVar, ExpressionReflector.GetMethodInfo<JsonReader>(r => r.ReadNextChar())),
                Expression.IfThen(
                    Expression.Equal(nextCharVar, Expression.Constant(',')),
                    Expression.Goto(continueTarget)),
                Expression.IfThen(
                    Expression.Equal(nextCharVar, Expression.Constant(endOfBlock)),
                    Expression.Goto(breakTarget)),
                Expression.Call(
                    jsonReaderVar,
                    ExpressionReflector.GetMethodInfo<JsonReader>(r => r.ThrowError("")),
                    Expression.Constant("Input is not a valid Json: ',' or '}' expected at position {0}.")));
        }

        private static Expression GetMemberFactory(ParameterExpression jsonReaderParam, ParameterExpression settingsParam, PropertyInfo p)
        {
            var info = GetOrCreate(p.PropertyType);
            var value = info.GetDeserializerExpression(jsonReaderParam, settingsParam);
            return value.Type == p.PropertyType ? value : Expression.Convert(value, p.PropertyType);
        }
    }
}
