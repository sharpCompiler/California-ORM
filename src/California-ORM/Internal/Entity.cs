using California_ORM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace California_ORM.Internal;


static class Entity
{
    /// <summary>
    /// Members of the model
    /// </summary>
    /// <returns>Returns all the properties except those marked with [IgnoreMember] attribute</returns>
    internal static PropertyContainer GetProperties<T>()
    {
        var allProperties = typeof(T).GetProperties().Where(x => !x.GetCustomAttributes<IgnoreMember>().Any());
        var keyProperty = allProperties.FirstOrDefault(predicate: x => x.GetCustomAttributes<PrimaryKey>().Any());
        var otherProperties = allProperties.Where(predicate: x => !x.GetCustomAttributes<PrimaryKey>().Any());

        return new PropertyContainer(keyProperty, otherProperties, allProperties);
    }

    internal static TableSchemaName GetEntityName(Type entityType)
    {
        var attributes = entityType.GetCustomAttributes<Table>();
        if (attributes.Any())
        {
            var table = attributes.First();
            return new TableSchemaName(table.Name, table.Schema);
        }

        return new TableSchemaName(entityType.Name, "dbo");
    }

    internal static string GetPropertyName(MemberInfo propertyInfo)
    {
        return propertyInfo.Name;
    }

    internal static Dictionary<string, object> GetPropertiesWithValues<T>(T entity, Func<PropertyInfo, bool> expression)
    {
        var properties = new Dictionary<string, object>();
        var propertyInfos = typeof(T).GetProperties().Where(expression);

        foreach (var propertyInfo in propertyInfos)
        {
            var propertyName = GetPropertyName(propertyInfo);
            var propertyValue = propertyInfo.GetValue(entity);
            properties.Add(propertyName, propertyValue);
        }

        return properties;
    }


}