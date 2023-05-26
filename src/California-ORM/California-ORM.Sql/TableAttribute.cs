using System;

namespace California_ORM.Sql;

[AttributeUsage(AttributeTargets.Class)]
public class Table: Attribute
{
    public string Name { get; }

    public Table(string name)
    {
        Name = name;
    }
}
