namespace OnionWebApi.Application.Extensions;
public static class ExpandoExtensions
{
    public static ExpandoObject ToExpandoObject(this object obj)
    {
        IDictionary<string, object> expando = new ExpandoObject();
        foreach (var property in obj.GetType().GetProperties())
        {
            expando[property.Name] = property.GetValue(obj);
        }
        return (ExpandoObject)expando;
    }
}
