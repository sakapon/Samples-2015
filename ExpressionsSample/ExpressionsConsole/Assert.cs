using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionsConsole
{
    [DebuggerNonUserCode]
    public static class Assert
    {
        public static void IsArgumentNotNull<T>(Expression<Func<T>> getValue) where T : class
        {
            var value = getValue.Compile()();
            if (value != null) return;

            var member = (MemberExpression)getValue.Body;
            throw new ArgumentNullException(member.Member.Name);
        }

        public static void IsArgumentNotEmpty(Expression<Func<string>> getValue)
        {
            var value = getValue.Compile()();
            if (value.Length > 0) return;

            var member = (MemberExpression)getValue.Body;
            throw new ArgumentException("The value must not be empty.", member.Member.Name);
        }
    }
}
