using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using California_ORM.Internal;
using Microsoft.Data.SqlClient;

namespace California_ORM.Extensions;

public class ExtraField
{
    public string FieldName { get; }
    public object? Value { get; }

    public ExtraField(string fieldName, object? value)
    {
        FieldName = fieldName;
        Value = value;
    }
}
public static class CaliforniaExtension
{

    public static async Task<int> InsertAsync<T>(this SqlConnection connection, T entity, SqlTransaction? transaction = null, Dictionary<string, object?>? extraFields = null)
    {
        var sql = "INSERT INTO [{0}].[{1}] ([{2}]) VALUES ({3})";
        var tableName = Entity.GetEntityName(typeof(T));
        var properties = Entity.GetProperties<T>();

        var allProperties = properties.OtherProperties.Select(x => x.Name).ToList();
        if(properties.KeyProperty != null)
            allProperties.Add(properties.KeyProperty.Name);
        if (extraFields?.Count > 0)
        {
            allProperties.AddRange(extraFields.Select(x => x.Key));
        }

        var propertyNameJoin = string.Join("], [", allProperties);
        var propertyValueJoin = string.Join(", ", allProperties.Select(x => "@" + x));

        var insertSql = string.Format(sql, tableName.Schema, tableName.TableName, propertyNameJoin, propertyValueJoin);

        await using var cmd = new SqlCommand (insertSql,  connection, transaction);
        foreach (var property in allProperties)
        {
            if (extraFields?.ContainsKey(property) ?? false)
            {
                var extraFieldValue = extraFields[property];
                cmd.Parameters.AddWithValue(property, extraFieldValue ?? DBNull.Value);
                continue;
            }

            var value = typeof(T).GetProperty(property).GetValue(entity);

            cmd.Parameters.AddWithValue(property, value);
        }
    
        return await cmd.ExecuteNonQueryAsync();
    }


    //public static Task<int> DeleteAsync<T>(this SqlConnection connection, T entity, SqlTransaction? transaction = null)
    //{
    //    var sql = "DELETE FROM [{0}].[{1}] WHERE [{2}] = '{3}'";

    //    //var tableName = GetEntityName(typeof(T));
    //    //var primaryKeyFields = GetPropertiesWithValues(entity, x => x.GetCustomAttributes<PrimaryKey>().Any()).Single();

    //    //var deleteSql = string.Format(sql, tableName.Schema, tableName.TableName, primaryKeyFields.Key, primaryKeyFields.Value);

    //    var cmd = connection.CreateCommand();
    //    //cmd.CommandText = deleteSql;
    //    cmd.Transaction = transaction;
    //    return cmd.ExecuteNonQueryAsync();
    //}


    //public static int Update<T>(this SqlConnection connection, T entity, SqlTransaction transaction = null)
    //{
    //    var sql = @"UPDATE [{0}].[{1}]
    //               SET 
    //                {2}
    //                 WHERE [{3}] = '{4}'";

    //    var tableName = GetEntityName(typeof(T));
    //    var fields = GetPropertiesWithValues(entity, x => !x.GetCustomAttributes<IgnoreMember>().Any() && !x.GetCustomAttributes<PrimaryKey>().Any());
    //    var primaryKeyFields = GetPropertiesWithValues(entity, x => x.GetCustomAttributes<PrimaryKey>().Any()).Single();
    //    var updateFields = new List<string>();
    //    foreach (var f in fields)
    //    {
    //        var str= "[" + f.Key + "] = '" + f.Value + "'";
    //        updateFields.Add(str);
    //    }

    //    var updateFiledAndValue = string.Join(", \n", updateFields);

    //    var updateSql = string.Format(sql, tableName.Schema, tableName.TableName, updateFiledAndValue, primaryKeyFields.Key, primaryKeyFields.Value);

    //    var cmd = connection.CreateCommand();
    //    cmd.CommandText = updateSql;
    //    cmd.Transaction = transaction;
    //    return cmd.ExecuteNonQuery();
    //}

    //public static T? Get<T>(this SqlConnection connection, object entityId, SqlTransaction? transaction = null) where T : class
    //{
    //    var sql = "SELECT [{0}] FROM [{1}].[{2}] WHERE [{3}] = '{4}'";
    //    var tableName = GetEntityName(typeof(T));
    //    var primaryKey = typeof(T).GetProperties().Single(x => x.GetCustomAttributes<PrimaryKey>().Any());

    //    var fields = GetProperties<T>(x => !x.GetCustomAttributes<IgnoreMember>().Any());
    //    var fieldsJoin = string.Join("], [", fields);

    //    var getSql = string.Format(sql, fieldsJoin, tableName.Schema, tableName.TableName, primaryKey.Name, entityId);

    //    var cmd = connection.CreateCommand();
    //    cmd.CommandText = getSql;
    //    cmd.Transaction = transaction;
    //    var reader = cmd.ExecuteReader();
    //    if (reader.Read())
    //    {
    //        var instance = Activator.CreateInstance<T>();
    //        foreach (var field in fields)
    //        {
    //            var value = reader[field];
    //            typeof(T).GetProperties().First(x => x.Name == field).SetValue(instance, value);
    //        }

    //        return instance;
    //    }

    //    return null;
    //}
    

   

}