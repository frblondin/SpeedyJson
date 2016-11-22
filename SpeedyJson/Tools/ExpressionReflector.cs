using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson.Tools
{
    /// <summary>
    /// Set of static methods retrieving information from expressions, typically
    /// the property or the method information.
    /// </summary>
    public static class ExpressionReflector
    {
        /// <summary>
        /// Extracts the <see cref="System.Reflection.ConstructorInfo" /> from an expression containing the
        /// constructor to be extracted.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcExpression">The func expression.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static ConstructorInfo GetConstructorInfo<T>(Expression<Func<T>> funcExpression)
        {
            var expression = funcExpression.Body;
            NewExpression constructorCall = null;
            if (expression is NewExpression)
            {
                constructorCall = (NewExpression)expression;
            }
            else if (expression is UnaryExpression)
            {
                constructorCall = ((UnaryExpression)expression).Operand as NewExpression;
            }
            if (constructorCall == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' doesn't refer to a constructor.",
                    expression.ToString()));
            }
            return constructorCall != null ? constructorCall.Constructor : null;
        }

        /// <summary>
        /// Extracts the <see cref="System.Reflection.MethodInfo" /> from an action expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="returnGenericDefinition">if set to <c>true</c> returns the generic definition if the method is generic.</param>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo(Expression<Action> expression, bool returnGenericDefinition = false)
        {
            return GetMethodInfoFromBody(expression.Body, returnGenericDefinition);
        }

        /// <summary>
        /// Extracts the <see cref="System.Reflection.MethodInfo" /> from an action expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="returnGenericDefinition">if set to <c>true</c> returns the generic definition if the method is generic.</param>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression, bool returnGenericDefinition = false)
        {
            return GetMethodInfoFromBody(expression.Body, returnGenericDefinition);
        }

        private static MethodInfo GetMethodInfoFromBody(Expression expression, bool returnGenericDefinition = false)
        {
            var invocation = expression as InvocationExpression;
            if (invocation != null) expression = invocation.Expression;

            LambdaExpression lambdaExpression;
            while ((lambdaExpression = expression as LambdaExpression) != null)
                expression = lambdaExpression.Body;

            MethodCallExpression methodCall = null;
            if (expression is MethodCallExpression)
            {
                methodCall = (MethodCallExpression)expression;
            }
            else if (expression is UnaryExpression)
            {
                methodCall = ((UnaryExpression)expression).Operand as MethodCallExpression;
            }
            if (methodCall == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' doesn't refer to a method.",
                    expression.ToString()));
            }
            if (methodCall != null && returnGenericDefinition && methodCall.Method.IsGenericMethod)
            {
                // Get the first method with the same name (no type matching for now...)
                return methodCall.Method.GetGenericMethodDefinition();//.DeclaringType.GetMethod(methodCall.Method.Name);
            }
            return methodCall != null ? methodCall.Method : null;
        }

        /// <summary>
        /// Extracts the property info from an expression.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyLambda">The property lambda.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var propInfo = GetMemberInfo(propertyLambda) as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            return propInfo;
        }

        /// <summary>
        /// Extracts the field info from an expression.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="fieldLambda">The field lambda.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static FieldInfo GetFieldInfo<TSource, TField>(Expression<Func<TSource, TField>> fieldLambda)
        {
            var fieldInfo = GetMemberInfo(fieldLambda) as FieldInfo;
            if (fieldInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property, not a field.",
                    fieldLambda.ToString()));

            return fieldInfo;
        }

        /// <summary>
        /// Extracts the member info from an expression.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TMember">The type of the member.</typeparam>
        /// <param name="memberLambda">The member lambda.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static MemberInfo GetMemberInfo<TSource, TMember>(Expression<Func<TSource, TMember>> memberLambda)
        {
            Expression expression = memberLambda;
            var invocation = expression as InvocationExpression;
            if (invocation != null) expression = invocation.Expression;

            LambdaExpression lambdaExpression;
            while ((lambdaExpression = expression as LambdaExpression) != null)
                expression = lambdaExpression.Body;

            var type = typeof(TSource);

            var member = expression as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property or field.",
                    memberLambda.ToString()));

            var memberInfo = member.Member;

            if (type != memberInfo.ReflectedType && !type.IsSubclassOf(memberInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field that is not from type {1}.",
                    memberLambda.ToString(),
                    type));

            return memberInfo;
        }

        /// <summary>
        /// Extracts the property info from an expression.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyLambda">The property lambda.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static PropertyInfo GetPropertyInfo<TProperty>(Expression<Func<TProperty>> propertyLambda)
        {
            var propInfo = GetMemberInfo(propertyLambda) as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            return propInfo;
        }

        /// <summary>
        /// Extracts the field info from an expression.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="fieldLambda">The field lambda.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static FieldInfo GetFieldInfo<TField>(Expression<Func<TField>> fieldLambda)
        {
            var fieldInfo = GetMemberInfo(fieldLambda) as FieldInfo;
            if (fieldInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property, not a field.",
                    fieldLambda.ToString()));

            return fieldInfo;
        }

        /// <summary>
        /// Extracts the member info from an expression.
        /// </summary>
        /// <typeparam name="TMember">The type of the member.</typeparam>
        /// <param name="memberLambda">The member lambda.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static MemberInfo GetMemberInfo<TMember>(Expression<Func<TMember>> memberLambda)
        {
            Expression expression = memberLambda;
            var invocation = expression as InvocationExpression;
            if (invocation != null) expression = invocation.Expression;

            LambdaExpression lambdaExpression;
            while ((lambdaExpression = expression as LambdaExpression) != null)
                expression = lambdaExpression.Body;

            var member = expression as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property or field.",
                    memberLambda.ToString()));

            return member.Member;
        }
    }
}
