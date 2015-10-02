using System;

namespace ExpressionsConsole
{
    public static class ConvertHelper
    {
        public static T To<T>(this object value)
        {
            return (T)value.To(typeof(T));
        }

        public static object To(this object value, Type type)
        {
            return type.IsConstructedGenericType ? value.To_Generic(type) : value.To_NonGeneric(type);
        }

        static object To_NonGeneric(this object value, Type type)
        {
            if (type.IsEnum)
            {
                if (value == null) throw new ArgumentNullException("value");
                return Enum.Parse(type, value.ToString());
            }
            else if (type == typeof(DateTimeOffset))
            {
                if (value == null) throw new ArgumentNullException("value");
                return DateTimeOffset.Parse(value.ToString());
            }
            else if (type == typeof(Guid))
            {
                if (value == null) throw new ArgumentNullException("value");
                return Guid.Parse(value.ToString());
            }
            else if (type == typeof(TimeSpan))
            {
                if (value == null) throw new ArgumentNullException("value");
                return TimeSpan.Parse(value.ToString());
            }
            else
            {
                return Convert.ChangeType(value, type);
            }
        }

        static object To_Generic(this object value, Type type)
        {
            var definition = type.GetGenericTypeDefinition();
            if (definition == typeof(Nullable<>))
            {
                return value == null ? null : value.To_NonGeneric(type.GenericTypeArguments[0]);
            }
            else
            {
                throw new InvalidCastException("The target type is not supported.");
            }
        }
    }
}
