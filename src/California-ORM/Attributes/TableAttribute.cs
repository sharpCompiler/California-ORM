using System;

namespace California_ORM.Attributes;


[AttributeUsage(AttributeTargets.Class)]
public class Table: Attribute
{
    public string Name { get; }
    public string Schema { get; }

    public Table(string name, string schema = "dbo")
    {
        Name = name;
        Schema = schema;
    }
}
