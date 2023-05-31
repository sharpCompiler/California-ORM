using System.Reflection;

namespace California_ORM.Sql.Builder;

public record PropertyMapped(PropertyInfo PropertyInfo, string ColumnName);