using System.Collections.Generic;
using System.Reflection;

namespace California_ORM.Internal;

internal class PropertyContainer
{
    public PropertyContainer(PropertyInfo? keyProperty, IEnumerable<PropertyInfo> otherProperties, IEnumerable<PropertyInfo> allProperties)
    {
        KeyProperty = keyProperty;
        OtherProperties = otherProperties;
        AllProperties = allProperties;
    }
    internal PropertyInfo? KeyProperty { get; }
    internal IEnumerable<PropertyInfo> OtherProperties { get; }
    internal IEnumerable<PropertyInfo> AllProperties { get; }
    
}