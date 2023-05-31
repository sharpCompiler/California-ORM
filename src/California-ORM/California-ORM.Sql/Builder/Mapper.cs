using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace California_ORM.Sql.Builder;

public static class Mapper
{
    private static Dictionary<string, object> _dictionary = new();

    public static Configurator<T> MapClass<T>() where T : class
    {
        var configuration = new Configurator<T>(typeof(T).Name, "");
        _dictionary.Add(typeof(T).FullName, configuration);
        return configuration;
    }
    public static Configurator<T> MapClass<T>(string schemaName) where T : class
    {
        var configuration = new Configurator<T>(typeof(T).Name, schemaName);
        _dictionary.Add(typeof(T).FullName, configuration);
        return configuration;
    }

    public static int Insert<T>(this IDbConnection connection, T entity, IDbTransaction? transaction = null)
    {
        if (!_dictionary.ContainsKey(typeof(T).FullName))
            throw new MappingNotFoundException(
                $"Mapping is not found for type: {typeof(T).FullName}. User Mapper to define the mapping for any type");

        var configuration = (Configurator<T>)_dictionary[typeof(T).FullName];
        //var ignoredProperties = configuration.GetMappings().OfType<PropertyIgnored>();
        var mappedProperties = configuration.GetMappings().OfType<PropertyMapped>();

        var sql = "INSERT INTO [{0}].[{1}] ([{2}]) VALUES ({3})";
        var schemaName = configuration.SchemaName;
        var tableName = configuration.TableName;

        var columnNames = mappedProperties.Select(x => x.ColumnName);
        var parameterName = mappedProperties.Select(x => "@" + x.ColumnName);
        var columnNamesJoin = string.Join("], [", columnNames);
        var parameterNameJoin = string.Join(", ", parameterName);

        var insertSql = string.Format(sql, schemaName, tableName, columnNamesJoin, parameterNameJoin);

        using var cmd = new SqlCommand(insertSql, (SqlConnection)connection, (SqlTransaction)transaction);
        foreach (var pn in mappedProperties)
        {
            var value = typeof(T).GetProperty(pn.PropertyInfo.Name).GetValue(entity);
            if (pn.PropertyInfo.PropertyType == typeof(DateOnly))
            {
                value = ((DateOnly)value).ToDateTime(TimeOnly.MinValue);
            }
            var prm = new SqlParameter(pn.PropertyInfo.Name, value);
            cmd.Parameters.Add(prm);
        }

        cmd.ExecuteNonQuery();

        return 1;
    }

}