using System;
using System.Linq.Expressions;

namespace BetterExpertRarity.Utils
{
    public static class TypeUtils
    {
        public static Func<T, R> GetFieldAccessor<T, R>(string fieldName)
        {
            var param = Expression.Parameter(typeof(T), "arg");
            var member = Expression.Field(param, fieldName);
            var lambda = Expression.Lambda(typeof(Func<T, R>), member, param);

            return (Func<T, R>)lambda.Compile();
        }
    }
}