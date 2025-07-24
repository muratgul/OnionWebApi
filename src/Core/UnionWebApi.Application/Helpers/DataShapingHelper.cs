using System.Dynamic;

namespace UnionWebApi.Application.Helpers;
public static class DataShapingHelper
{
    public static IEnumerable<ExpandoObject> ShapeDataList<TSource>(this IEnumerable<TSource> source, string fields)
    {
        var expandoObjectList = new List<ExpandoObject>();
        var propertyInfos = typeof(TSource).GetProperties();
        var fieldList = fields.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach (var sourceObject in source)
        {
            var dataShapedObject = new ExpandoObject();
            foreach (var field in fieldList)
            {
                var propertyInfo = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));
                if (propertyInfo != null)
                {
                    var fieldValue = propertyInfo.GetValue(sourceObject);
                    (dataShapedObject as IDictionary<string, object>).Add(propertyInfo.Name, fieldValue);
                }
            }

            expandoObjectList.Add(dataShapedObject);
        }

        return expandoObjectList;
    }

    public static ExpandoObject ShapeData<TSource>(this TSource source, string fields)
    {
        var dataShapedObject = new ExpandoObject();
        var propertyInfos = typeof(TSource).GetProperties();
        var fieldList = fields.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach (var field in fieldList)
        {
            var propertyInfo = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));
            if (propertyInfo != null)
            {
                var fieldValue = propertyInfo.GetValue(source);
                (dataShapedObject as IDictionary<string, object>).Add(propertyInfo.Name, fieldValue);
            }
        }

        return dataShapedObject;
    }
}

