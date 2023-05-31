using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace California_ORM.Sql.Builder;

public class Configurator<T> 
{
    private readonly List<object> list = new();

    public string TableName { get; }
    public string SchemaName { get; }

    public Configurator(string tableName, string schemaName)
    {
        TableName = tableName;
        SchemaName = string.IsNullOrEmpty(schemaName) ? "dbo" : schemaName;
    }

    public Configurator<T> ForMember<TP>(Expression<Func<T, TP>> expression, string columnName = "")
    {
        var propertyInfo = GetPropertyInfo(expression);
        var columnNameValue = string.IsNullOrEmpty(columnName) ? propertyInfo.Name : columnName;
        var propertyMapped = new PropertyMapped(propertyInfo, columnNameValue);
       
        list.Add(propertyMapped);
        return this;
    }
    public Configurator<T> IgnoreMember<TP>(Expression<Func<T, TP>> expression, string columnName)
    {
        var propertyInfo = GetPropertyInfo(expression);
        var propertyMapped = new PropertyMapped(propertyInfo, columnName);
        list.Add(propertyMapped);
        return this;
    }

    public IEnumerable<object> GetMappings()
    {
        return list;
    }

    private PropertyInfo GetPropertyInfo<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
    {
        if (propertyLambda.Body is not MemberExpression member)
        {
            throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");
        }

        if (member.Member is not PropertyInfo propInfo)
        {
            throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");
        }

        Type type = typeof(TSource);
        if (propInfo.ReflectedType != null && type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
        {
            throw new ArgumentException($"Expression '{propertyLambda}' refers to a property that is not from type {type}.");
        }

        return propInfo;
    }
}