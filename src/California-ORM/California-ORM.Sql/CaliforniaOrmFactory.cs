using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace California_ORM.Sql;

public static class CaliforniaExtension
{
    private record TableSchemaName(string TableName, string Schema);

    public static void Insert<T>(this IDbConnection connection, T entity, IDbTransaction? transaction = null)
    {
        var sql = "INSERT INTO [{0}].[{1}] ([{2}]) VALUES ('{3}')";
        var tableName = GetEntityName(entity);
        var properties = GetProperties(entity, x => !x.GetCustomAttributes<IgnoreMember>().Any());
            
        var propertyNames = properties.Select(x => x.Key);
        var propertyValues = properties.Select(x => x.Value);

        var propertyNameJoin = string.Join("], [", propertyNames);
        var propertyValueJoin = string.Join("', '", propertyValues);
            
        var insertSql = string.Format(sql, tableName.Schema, tableName.TableName, propertyNameJoin, propertyValueJoin);

        var cmd = connection.CreateCommand();
        cmd.CommandText = insertSql;
        cmd.Transaction = transaction;
        cmd.ExecuteScalar();
    }

    private static Dictionary<string, object> GetProperties<T>(T entity, Func<PropertyInfo, bool> expression)
    {
        var properties = new Dictionary<string, object>();
        var propertyInfos = typeof(T).GetProperties().Where(expression);

        foreach (var propertyInfo in propertyInfos)
        {
            var propertyName = GetPropertyName(propertyInfo);
            var propertyValue = GetPropertyValue(propertyInfo, entity);
            properties.Add(propertyName, propertyValue);
        }

        return properties;
    }

    private static TableSchemaName GetEntityName(object entity)
    {
        var attributes = entity.GetType().GetCustomAttributes<Table>();
        if (attributes.Any())
        {
            var table = attributes.First();
            return new TableSchemaName(table.Name, table.Schema);
        }
        
        return new TableSchemaName(entity.GetType().Name, "dbo");
    }

    private static string GetPropertyName(MemberInfo propertyInfo)
    {
        return propertyInfo.Name;
    }

    private static object? GetPropertyValue(PropertyInfo propertyInfo, object? entity)
    {
        if (entity == null)
            return null;

        if (propertyInfo.PropertyType == typeof(DateTime))
        {
            var value = (DateTime)propertyInfo.GetValue(entity);
            return value.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        if (propertyInfo.PropertyType == typeof(DateOnly))
        {
            var value = (DateOnly)propertyInfo.GetValue(entity);
            return value.ToString("yyyy-MM-dd");
        }

        if (propertyInfo.PropertyType == typeof(string))
        {
            var value = (string)propertyInfo.GetValue(entity);
            return value.Replace("'", "''");
        }


        return propertyInfo.GetValue(entity);
    }

}