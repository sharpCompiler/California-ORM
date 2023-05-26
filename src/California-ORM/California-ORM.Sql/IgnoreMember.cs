using System;

namespace California_ORM.Sql;

[AttributeUsage(AttributeTargets.Property)]
public class IgnoreMember : Attribute
{
}
