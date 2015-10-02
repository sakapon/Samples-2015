using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionsConsole
{
    public static class ExpressionHelper
    {
        public static object Invoke(this Expression expression)
        {
            var lambda = Expression.Lambda(expression);
            var func = lambda.Compile();
            return func.DynamicInvoke();
        }

        public static object Invoke(this Expression expression, ParameterExpression[] parameters, object[] args)
        {
            var lambda = Expression.Lambda(expression, parameters);
            var func = lambda.Compile();
            return func.DynamicInvoke(args);
        }
    }
}
