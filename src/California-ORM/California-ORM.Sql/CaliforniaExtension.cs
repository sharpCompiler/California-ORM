//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Reflection;

//namespace California_ORM.Sql;

//public static class CaliforniaExtension
//{
//    private record TableSchemaName(string TableName, string Schema);

//    public static int Insert<T>(this IDbConnection connection, T entity, IDbTransaction? transaction = null)
//    {
//        var sql = "INSERT INTO [{0}].[{1}] ([{2}]) VALUES ('{3}')";
//        var tableName = GetEntityName(typeof(T));
//        var properties = GetPropertiesWithValues(entity, x => !x.GetCustomAttributes<IgnoreMember>().Any());

//        var propertyNames = properties.Select(x => x.Key);
//        var propertyParameters = properties.Select(x => "@" + x.Key);

//        var propertyNameJoin = string.Join("], [", propertyNames);
//        var propertyValueJoin = string.Join("', '", propertyParameters);

//        var insertSql = string.Format(sql, tableName.Schema, tableName.TableName, propertyNameJoin, propertyValueJoin);

//        var cmd = connection.CreateCommand();
//        cmd.CommandText = insertSql;
//        cmd.Transaction = transaction;
//        foreach (var property in properties)
//        {
//            var prm = cmd.CreateParameter();
//            prm.ParameterName = property.Key;
//            prm.Value = property.Value;
//        }

//        return cmd.ExecuteNonQuery();
//    }

//    public static int Delete<T>(this IDbConnection connection, T entity, IDbTransaction? transaction = null)
//    {
//        var sql = "DELETE FROM [{0}].[{1}] WHERE [{2}] = '{3}'";
//        var tableName = GetEntityName(typeof(T));
//        var primaryKeyFields = GetPropertiesWithValues(entity, x => x.GetCustomAttributes<PrimaryKey>().Any()).Single();
        
//        var deleteSql = string.Format(sql, tableName.Schema, tableName.TableName, primaryKeyFields.Key, primaryKeyFields.Value);

//        var cmd = connection.CreateCommand();
//        cmd.CommandText = deleteSql;
//        cmd.Transaction = transaction;



//        return cmd.ExecuteNonQuery();
//    }


//    public static int Update<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null)
//    {
//        var sql = @"UPDATE [{0}].[{1}]
//                   SET 
//                    {2}
//                     WHERE [{3}] = '{4}'";

//        var tableName = GetEntityName(typeof(T));
//        var fields = GetPropertiesWithValues(entity, x => !x.GetCustomAttributes<IgnoreMember>().Any() && !x.GetCustomAttributes<PrimaryKey>().Any());
//        var primaryKeyFields = GetPropertiesWithValues(entity, x => x.GetCustomAttributes<PrimaryKey>().Any()).Single();
//        var updateFields = new List<string>();
//        foreach (var f in fields)
//        {
//            var str= "[" + f.Key + "] = '" + f.Value + "'";
//            updateFields.Add(str);
//        }

//        var updateFiledAndValue = string.Join(", \n", updateFields);

//        var updateSql = string.Format(sql, tableName.Schema, tableName.TableName, updateFiledAndValue, primaryKeyFields.Key, primaryKeyFields.Value);

//        var cmd = connection.CreateCommand();
//        cmd.CommandText = updateSql;
//        cmd.Transaction = transaction;
//        return cmd.ExecuteNonQuery();
//    }

//    public static T? Get<T>(this IDbConnection connection, object entityId, IDbTransaction? transaction = null) where T : class
//    {
//        var sql = "SELECT [{0}] FROM [{1}].[{2}] WHERE [{3}] = '{4}'";
//        var tableName = GetEntityName(typeof(T));
//        var primaryKey = typeof(T).GetProperties().Single(x => x.GetCustomAttributes<PrimaryKey>().Any());

//        var fields = GetProperties<T>(x => !x.GetCustomAttributes<IgnoreMember>().Any());
//        var fieldsJoin = string.Join("], [", fields);

//        var getSql = string.Format(sql,fieldsJoin, tableName.Schema, tableName.TableName, primaryKey.Name, entityId);

//        var cmd = connection.CreateCommand();
//        cmd.CommandText = getSql;
//        cmd.Transaction = transaction;
//        var reader = cmd.ExecuteReader();
//        if (reader.Read())
//        {
//            var instance = Activator.CreateInstance<T>();
//            foreach (var field in fields)
//            {
//                var value = reader[field];
//                typeof(T).GetProperties().First(x => x.Name == field).SetValue(instance, value);
//            }

//            return instance;
//        }

//        return null;
//    }

//    private static List<string> GetProperties<T>(Func<PropertyInfo, bool> expression)
//    {
//        var properties = new List<string>(20);
//        var propertyInfos = typeof(T).GetProperties().Where(expression);

//        foreach (var propertyInfo in propertyInfos)
//        {
//            var propertyName = GetPropertyName(propertyInfo);
//            properties.Add(propertyName);
//        }

//        return properties;
//    }

//    private static Dictionary<string, object> GetPropertiesWithValues<T>(T entity, Func<PropertyInfo, bool> expression)
//    {
//        var properties = new Dictionary<string, object>();
//        var propertyInfos = typeof(T).GetProperties().Where(expression);

//        foreach (var propertyInfo in propertyInfos)
//        {
//            var propertyName = GetPropertyName(propertyInfo);
//            var propertyValue = GetPropertyValue(propertyInfo, entity);
//            properties.Add(propertyName, propertyValue);
//        }

//        return properties;
//    }

//    private static TableSchemaName GetEntityName(Type entityType)
//    {
//        var attributes = entityType.GetCustomAttributes<Table>();
//        if (attributes.Any())
//        {
//            var table = attributes.First();
//            return new TableSchemaName(table.Name, table.Schema);
//        }
        
//        return new TableSchemaName(entityType.Name, "dbo");
//    }

//    private static string GetPropertyName(MemberInfo propertyInfo)
//    {
//        return propertyInfo.Name;
//    }

//    private static object? GetPropertyValue(PropertyInfo propertyInfo, object? entity)
//    {
//        if (entity == null)
//            return null;

//        if (propertyInfo.PropertyType == typeof(DateTime))
//        {
//            var value = (DateTime)propertyInfo.GetValue(entity);
//            return value.ToString("yyyy-MM-dd HH:mm:ss.fff");
//        }

//        if (propertyInfo.PropertyType == typeof(DateOnly))
//        {
//            var value = (DateOnly)propertyInfo.GetValue(entity);
//            return value.ToString("yyyy-MM-dd");
//        }

//        if (propertyInfo.PropertyType == typeof(string))
//        {
//            var value = (string)propertyInfo.GetValue(entity);
//            return value.Replace("'", "''");
//        }


//        return propertyInfo.GetValue(entity);
//    }

//}