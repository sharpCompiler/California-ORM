using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Returns effected rows in the table
    /// </summary>
    /// <param name="connection">Db connection is expected to be in Open State</param>
    /// <param name="entity">Entity to be Added</param>
    /// <param name="transaction">SQL Transaction</param>
    /// <param name="extraFields">Extra files like Foreign Key, or something than is not a part of the model</param>
    /// <returns></returns>
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

    /// <summary>
    /// Deleting entity from the table
    /// </summary>
    /// <param name="primaryKeyValue">Value of the primary key</param>
    /// <param name="transaction">SQL Transaction</param>
    /// <returns>Returns number effected in the table</returns>
    public static Task<int> DeleteAsync<T>(this SqlConnection connection, object entityId, SqlTransaction? transaction = null)
    {
        var sql = "DELETE FROM [{0}].[{1}] WHERE [{2}] = '{3}'";

        var tableName = Entity.GetEntityName(typeof(T));
        var primaryKeyField = Entity.GetProperties<T>().KeyProperty;
        if (primaryKeyField == null)
        {
            throw new Exception("Primary key filed is missing. Use [PrimaryKey] to define the primary key on you class");
        }

        var deleteSql = string.Format(sql, tableName.Schema, tableName.TableName, primaryKeyField.Name, entityId);

        var cmd = connection.CreateCommand();
        cmd.CommandText = deleteSql;
        cmd.Transaction = transaction;
        return cmd.ExecuteNonQueryAsync();
    }


    public static async Task<int> UpdateAsync<T>(this SqlConnection connection, T entity, SqlTransaction transaction = null)
    {
        var sql = @"UPDATE [{0}].[{1}]
                   SET 
                    {2}
                     WHERE [{3}] = @entityId";

        var tableName = Entity.GetEntityName(typeof(T));
        var entityProperties = Entity.GetProperties<T>();

        var fields = entityProperties.OtherProperties;
        var primaryKeyFields = entityProperties.KeyProperty;

        var updateFiledAndValue = string.Join(", \n", fields.Select(x => x.Name + " = @" + x.Name));

        var updateSql = string.Format(sql, tableName.Schema, tableName.TableName, updateFiledAndValue, primaryKeyFields.Name);

        var cmd = connection.CreateCommand();
        cmd.CommandText = updateSql;
        cmd.Transaction = transaction;

        foreach (var field in fields)
        {
            var value = field.GetValue(entity);
            cmd.Parameters.AddWithValue(field.Name, value);
        }

        var pkValue = entityProperties.KeyProperty.GetValue(entity);

        cmd.Parameters.AddWithValue("entityId", pkValue);

        return await cmd.ExecuteNonQueryAsync();
    }

    public static async IAsyncEnumerable<T> QueryAsync<T>(this SqlConnection connection, string sql, SqlTransaction? transaction = null, params ExtraField[] parameters) where T : class
    {
        var entityProperties = Entity.GetProperties<T>();

        var cmd = connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.Transaction = transaction;
        var reader = await cmd.ExecuteReaderAsync();

        while (reader.Read())
        {
            var instance = Activator.CreateInstance<T>();
            foreach (var field in entityProperties.AllProperties)
            {
                var value = reader[field.Name];
                typeof(T).GetProperties().First(x => x.Name == field.Name).SetValue(instance, value);
            }

            yield return instance;
        }
    }

    public static async Task<T?> GetAsync<T>(this SqlConnection connection, object entityId, SqlTransaction? transaction = null) where T : class
    {
        var sql = "SELECT  [{0}]  FROM [{1}].[{2}] WHERE [{3}] = @entityId";
        var tableName = Entity.GetEntityName(typeof(T));

        var entityProperties = Entity.GetProperties<T>();

        var primaryKey = entityProperties.KeyProperty;

        var allSelectFields = entityProperties.AllProperties;
        var fieldsJoin = string.Join("], [", allSelectFields.Select(x => x.Name));

        var getSql = string.Format(sql, fieldsJoin, tableName.Schema, tableName.TableName, primaryKey.Name);

        var cmd = connection.CreateCommand();
        cmd.CommandText = getSql;
        cmd.Transaction = transaction;
        cmd.Parameters.AddWithValue("entityId", entityId);
        var reader = await cmd.ExecuteReaderAsync();
        if (reader.Read())
        {
            var instance = Activator.CreateInstance<T>();
            foreach (var field in allSelectFields)
            {
                var value = reader[field.Name];
                typeof(T).GetProperties().First(x => x.Name == field.Name).SetValue(instance, value);
            }

            return instance;
        }

        return null;
    }
}