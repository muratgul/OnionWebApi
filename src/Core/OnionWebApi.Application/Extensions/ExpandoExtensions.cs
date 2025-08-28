namespace OnionWebApi.Application.Extensions;
public static class ExpandoExtensions
{
    /// <summary>
    /// Herhangi bir nesneyi ExpandoObject'e dönüştürür
    /// </summary>
    public static ExpandoObject ToExpandoObject(this object obj)
    {
        if (obj is null)
        {
            return new ExpandoObject();
        }

        if (obj is ExpandoObject expandoObj)
        {
            return expandoObj;
        }

        IDictionary<string, object?> expando = new ExpandoObject();

        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.CanRead)
            {
                var value = property.GetValue(obj);

                if (value is not null)
                {
                    expando[property.Name] = value;
                }
                
            }

           
        }
        return (ExpandoObject)expando;
    }


    /// <summary>
    /// Nesneyi deep copy olarak ExpandoObject'e dönüştürür (nested objeler de dönüştürülür)
    /// </summary>
    public static ExpandoObject ToExpandoObjectDeep(this object obj)
    {
        if (obj == null)
        {
            return new ExpandoObject();
        }

        if (obj is ExpandoObject expandoObj)
        {
            return expandoObj;
        }

        IDictionary<string, object?> expando = new ExpandoObject();
        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (!property.CanRead)
            {
                continue;
            }

            var value = property.GetValue(obj);

            if (value == null)
            {
                expando[property.Name] = null;
            }
            else if (IsSimpleType(value.GetType()))
            {
                expando[property.Name] = value;
            }
            else if (value is IEnumerable enumerable && value is not string)
            {
                expando[property.Name] = enumerable.Cast<object>()
                    .Select(item => IsSimpleType(item.GetType()) ? item : item?.ToExpandoObjectDeep())
                    .ToList();
            }
            else
            {
                expando[property.Name] = value.ToExpandoObjectDeep();
            }
        }

        return (ExpandoObject)expando;
    }

    /// <summary>
    /// ExpandoObject'i Dictionary'e dönüştürür
    /// </summary>
    public static Dictionary<string, object?> ToDictionary(this ExpandoObject expandoObject)
    {
        return expandoObject.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }


    /// <summary>
    /// ExpandoObject'e property ekler
    /// </summary>
    public static ExpandoObject AddProperty(this ExpandoObject expandoObject, string name, object value)
    {
        var dict = (IDictionary<string, object?>)expandoObject;
        dict[name] = value;
        return expandoObject;
    }

    /// <summary>
    /// ExpandoObject'den property kaldırır
    /// </summary>
    public static ExpandoObject RemoveProperty(this ExpandoObject expandoObject, string name)
    {
        var dict = (IDictionary<string, object?>)expandoObject;
        dict.Remove(name);
        return expandoObject;
    }


    /// <summary>
    /// Basit tip kontrolü (primitive, string, DateTime, etc.)
    /// </summary>
    private static bool IsSimpleType(Type type)
    {
        if (type == null)
        {
            return true;
        }

        return type.IsPrimitive ||
               type.IsEnum ||
               type == typeof(string) ||
               type == typeof(decimal) ||
               type == typeof(DateTime) ||
               type == typeof(DateTimeOffset) ||
               type == typeof(TimeSpan) ||
               type == typeof(Guid) ||
               Nullable.GetUnderlyingType(type) != null;
    }
}
