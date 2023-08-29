namespace California_ORM.Internal;

internal class TableSchemaName
{
    public string TableName { get; }
    public string Schema { get; }

    internal TableSchemaName(string tableName, string schema)
    {
        TableName = tableName;
        Schema = schema;
    }

}