using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Ecommerce.Core.Enums.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Ecommerce.Core.Helpers.Extensions
{
    public static class EntityExtension
    {
        public static string ClearScapeChars(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return input.Replace("\"", "");
        }

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, bool IsValid) where T : class
        {
            return IsValid ? source.Where(predicate) : source;
        }
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, Func<T, bool> predicate, bool IsValid) where T : class
        {
            return IsValid ? source.Where(predicate) : source;
        }


        public static IQueryable<T> EqualJsonBy<T>(this DbSet<T> source, string PropretyName, string PropertyValue, JsonKeyEnum JsonKey) where T : class
        {
            var infrastructure = source as IInfrastructure<IServiceProvider>;
            var serviceProvider = infrastructure.Instance;
            var currentDbContext = serviceProvider.GetService(typeof(ICurrentDbContext)) as ICurrentDbContext;
            var dbContext = currentDbContext.Context;

            var entityType = dbContext.Model.FindEntityType(typeof(T));
            var schema = entityType.GetSchema() ?? entityType.GetViewSchema();
            var tableName = entityType.GetTableName() ?? entityType.GetViewName();
            var stringQuery = $"SELECT * FROM {schema}.{tableName} ";

            // ToDo-Sully: what hasSearch means for that.
            bool hasSearch = false;

            if (!string.IsNullOrEmpty(PropertyValue.ClearScapeChars()))
            {
                stringQuery += !hasSearch ? " WHERE " : " OR ";
                hasSearch = true;

                stringQuery += $" JSON_VALUE({PropretyName},'$.{JsonKey}') = N'{PropertyValue}'";
            }

            var sqlQuery = stringQuery.ClearScapeChars();
            var result = source.FromSqlRaw<T>(sqlQuery);
            return hasSearch ? result : source;
        }

        public static IQueryable<T> EqualJsonBy<T>(this DbSet<T> source, List<EqualModel> equals) where T : class
        {
            var infrastructure = source as IInfrastructure<IServiceProvider>;
            var serviceProvider = infrastructure.Instance;
            var currentDbContext = serviceProvider.GetService(typeof(ICurrentDbContext)) as ICurrentDbContext;
            var dbContext = currentDbContext.Context;

            var entityType = dbContext.Model.FindEntityType(typeof(T));
            var schema = entityType.GetSchema() ?? entityType.GetViewSchema();
            var tableName = entityType.GetTableName() ?? entityType.GetViewName();
            var stringQuery = $"SELECT * FROM {schema}.{tableName} ";

            bool hasSearch = false;
            for (int i = 0; i < equals.Count; i++)
            {
                if (!string.IsNullOrEmpty(equals[i].PropertyValue.ClearScapeChars()))
                {
                    stringQuery += !hasSearch ? " WHERE " : " OR ";
                    hasSearch = true;

                    stringQuery += $" JSON_VALUE({equals[i].PropertyName},'$.{equals[i].JsonKey.ToString().ToLower()}') = N'{equals[i].PropertyValue}'";
                }
            }

            var sqlQuery = stringQuery.ClearScapeChars();
            var result = source.FromSqlRaw<T>(sqlQuery);
            return hasSearch ? result : source;
        }
    }
}
