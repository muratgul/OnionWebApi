namespace OnionWebApi.Application.Extensions;
public static class EnumExtensions
{
    public static IEnumerable<T> GetEnumValues<T>(this T input) where T : Enum => Enum.GetValues(typeof(T)).Cast<T>();
    public static IEnumerable<T> GetEnumFlags<T>(this T input) where T : Enum
    {
        foreach (var value in Enum.GetValues(input.GetType()))
        {
            if(input.HasFlag((Enum)value))
            {
                yield return (T)value;
            }
        }
    }
    public static string ToDisplay(this Enum value, DisplayProperty property = DisplayProperty.Name)
    {
        ArgumentNullException.ThrowIfNull(value);

        var attribute = value.GetType().GetField(value.ToString())!.GetCustomAttributes<DisplayAttribute>(false).FirstOrDefault();

        if (attribute is null)
        {
            return value.ToString();
        }

        var propValue = attribute.GetType().GetProperty(property.ToString())?.GetValue(attribute, null);
        return propValue?.ToString() ?? value.ToString();
    }
    public static Dictionary<int, string> ToDictionary<T>(this Enum value) where T : Enum
    {
        return Enum.GetValues(value.GetType()).Cast<Enum>().ToDictionary(p => Convert.ToInt32(p), q => ToDisplay(q));
    }
    public enum DisplayProperty
    {
        Description,
        GroupName,
        Name,
        Prompt,
        ShortName,
        Order
    }
}
