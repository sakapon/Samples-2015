using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionsConsole
{
    public static class EntityType
    {
        public static EntityType<TEntity> Create<TEntity>(TEntity baseObj)
        {
            var constructors = typeof(TEntity).GetConstructors();
            if (constructors.Length != 1) throw new InvalidOperationException("The number of the constructors must be 1.");

            return new EntityType<TEntity>(constructors[0]);
        }

        public static EntityType<TEntity> Create<TEntity>(Expression<Func<TEntity>> initializer)
        {
            if (initializer == null) throw new ArgumentNullException("initializer");

            var @new = initializer.Body as NewExpression;
            if (@new == null) throw new InvalidOperationException("The constructor must be specified.");

            return new EntityType<TEntity>(@new.Constructor);
        }
    }

    [DebuggerDisplay(@"{typeof(TEntity)}")]
    public class EntityType<TEntity>
    {
        public ConstructorInfo ConstructorInfo { get; private set; }

        internal EntityType(ConstructorInfo constructorInfo)
        {
            ConstructorInfo = constructorInfo;
        }

        public TEntity CreateEntity(params object[] parameters)
        {
            return (TEntity)ConstructorInfo.Invoke(parameters);
        }
    }

    [DebuggerDisplay(@"{typeof(TEntity)}")]
    public class EntityType2<TEntity>
    {
        public ConstructorInfo ConstructorInfo { get; private set; }

        Func<object[], TEntity> _constructor;

        internal EntityType2(ConstructorInfo constructorInfo)
        {
            ConstructorInfo = constructorInfo;

            _constructor = CompileConstructor(constructorInfo);
        }

        public TEntity CreateEntity(params object[] parameters)
        {
            return _constructor(parameters);
        }

        static Func<object[], TEntity> CompileConstructor(ConstructorInfo constructorInfo)
        {
            var parameterInfoes = constructorInfo.GetParameters();
            var p = Expression.Parameter(typeof(object[]), "p");
            var ctorExp = Expression.New(constructorInfo, parameterInfoes.Select(i => GetParameterValue(p, i)));

            // p => new TEntity((int)p[0], (string)p[1])
            var ctorLambda = Expression.Lambda<Func<object[], TEntity>>(ctorExp, p);
            return ctorLambda.Compile();
        }

        static Expression GetParameterValue(ParameterExpression p, ParameterInfo info)
        {
            var p_i = Expression.ArrayIndex(p, Expression.Constant(info.Position));
            return Expression.Convert(p_i, info.ParameterType);
        }
    }
}
