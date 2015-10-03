using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;

namespace ExpressionsConsole
{
    public static class TableHelper
    {
        static readonly MethodInfo String_CompareTo = typeof(string).GetMethod("CompareTo", new[] { typeof(string) });

        public static TElement Retrieve<TElement>(this CloudTable table, string partitionKey, string rowKey) where TElement : ITableEntity
        {
            if (table == null) throw new ArgumentNullException("table");

            var retrieve = TableOperation.Retrieve<TElement>(partitionKey, rowKey);
            var result = table.Execute(retrieve);
            return (TElement)result.Result;
        }

        public static TableQuery<TElement> Select<TElement>(this TableQuery<TElement> query, Expression<Func<TElement, object>> selector)
        {
            if (query == null) throw new ArgumentNullException("query");
            if (selector == null) throw new ArgumentNullException("selector");

            var @new = selector.Body as NewExpression;
            if (@new == null) throw new InvalidOperationException();

            query.SelectColumns = @new.Constructor.GetParameters().Select(p => p.Name).ToArray();
            return query;
        }

        public static TableQuery<TElement> Where<TElement>(this TableQuery<TElement> query, Expression<Func<TElement, bool>> predicate)
        {
            if (query == null) throw new ArgumentNullException("query");
            if (predicate == null) throw new ArgumentNullException("predicate");

            var binary = predicate.Body as BinaryExpression;
            if (binary == null) throw new InvalidOperationException();

            var filter = GenerateFilter(binary);
            query.FilterString = string.IsNullOrWhiteSpace(query.FilterString) ? filter : TableQuery.CombineFilters(query.FilterString, TableOperators.And, filter);
            return query;
        }

        static string GenerateFilter(BinaryExpression binary)
        {
            switch (binary.NodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    return CombineFilters(binary);
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    return (binary.Left is MethodCallExpression) ? GenerateFilterConditionForMethodCall(binary) : GenerateFilterCondition(binary);
                default:
                    throw new InvalidOperationException();
            }
        }

        static string CombineFilters(BinaryExpression binary)
        {
            var left = binary.Left as BinaryExpression;
            if (left == null) throw new InvalidOperationException();

            var right = binary.Right as BinaryExpression;
            if (right == null) throw new InvalidOperationException();

            var op = ToCombinationOperator(binary.NodeType);

            return TableQuery.CombineFilters(GenerateFilter(left), op, GenerateFilter(right));
        }

        static string GenerateFilterCondition(BinaryExpression binary)
        {
            var left = binary.Left as MemberExpression;
            if (left == null) throw new InvalidOperationException();

            var op = ToComparisonOperator(binary.NodeType);
            var rightValue = binary.Right.Invoke();

            return
                left.Type == typeof(byte[]) ? TableQuery.GenerateFilterConditionForBinary(left.Member.Name, op, (byte[])rightValue) :
                left.Type == typeof(bool) ? TableQuery.GenerateFilterConditionForBool(left.Member.Name, op, (bool)rightValue) :
                left.Type == typeof(DateTime) ? TableQuery.GenerateFilterConditionForDate(left.Member.Name, op, (DateTime)rightValue) :
                left.Type == typeof(DateTimeOffset) ? TableQuery.GenerateFilterConditionForDate(left.Member.Name, op, (DateTimeOffset)rightValue) :
                left.Type == typeof(double) ? TableQuery.GenerateFilterConditionForDouble(left.Member.Name, op, (double)rightValue) :
                left.Type == typeof(Guid) ? TableQuery.GenerateFilterConditionForGuid(left.Member.Name, op, (Guid)rightValue) :
                left.Type == typeof(int) ? TableQuery.GenerateFilterConditionForInt(left.Member.Name, op, (int)rightValue) :
                left.Type == typeof(long) ? TableQuery.GenerateFilterConditionForLong(left.Member.Name, op, (long)rightValue) :
                TableQuery.GenerateFilterCondition(left.Member.Name, op, rightValue.To<string>());
        }

        static string GenerateFilterConditionForMethodCall(BinaryExpression binary)
        {
            var methodCall = binary.Left as MethodCallExpression;
            if (methodCall == null) throw new InvalidOperationException();
            if (methodCall.Method != String_CompareTo) throw new InvalidOperationException();

            var left = methodCall.Object as MemberExpression;
            if (left == null) throw new InvalidOperationException();

            var op = ToComparisonOperator(binary.NodeType);
            var rightValue = methodCall.Arguments[0].Invoke();

            return TableQuery.GenerateFilterCondition(left.Member.Name, op, rightValue.To<string>());
        }

        static string ToCombinationOperator(ExpressionType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionType.AndAlso:
                    return TableOperators.And;
                case ExpressionType.OrElse:
                    return TableOperators.Or;
                default:
                    throw new InvalidOperationException();
            }
        }

        static string ToComparisonOperator(ExpressionType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    return (string)typeof(QueryComparisons).GetField(nodeType.ToString()).GetValue(null);
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
