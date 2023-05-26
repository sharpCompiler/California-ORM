using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace California_ORM.Sql;

public static class CaliforniaExtension
{

    public static void Insert<T>(this IDbConnection connection, T entity, IDbTransaction? transaction = null)
    {
        var sql = "INSERT INTO {0} ([{1}]) VALUES ('{2}')";
        var name = GetEntityName(entity);
        var properties = GetProperties(entity);
            
        var propertyNames = properties.Select(x => x.Key);
        var propertyValues = properties.Select(x => x.Value);

        var propertyNameJoin = string.Join("], [", propertyNames);
        var propertyValueJoin = string.Join("', '", propertyValues);
            

        var insertSql = string.Format(sql, name, propertyNameJoin, propertyValueJoin);

        var cmd = connection.CreateCommand();
        cmd.CommandText = insertSql;
        cmd.Transaction = transaction;
        cmd.ExecuteScalar();
    }


    private static Dictionary<string, object> GetProperties<T>(T entity)
    {
        var properties = new Dictionary<string, object>();
        var propertyInfos = typeof(T).GetProperties();

        foreach (var propertyInfo in propertyInfos)
        {
            var propertyName = GetPropertyName(propertyInfo);
            var propertyValue = GetPropertyValue(propertyInfo, entity);
            properties.Add(propertyName, propertyValue);
        }

        return properties;
    }

    private static string GetEntityName(object entity)
    {
        var attributes = entity.GetType().GetCustomAttributes<Table>();
        if (attributes.Any())
            return attributes.First().Name;
        
        return entity.GetType().Name;
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