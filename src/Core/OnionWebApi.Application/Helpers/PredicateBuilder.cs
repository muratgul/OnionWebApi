namespace OnionWebApi.Application.Helpers;
public class PredicateBuilder<T>
{

    public static Expression<Func<T, bool>> BuildPredicateFromJson(string json)
    {
        var jArray = JsonConvert.DeserializeObject<JArray>(json);

        if (jArray == null || jArray.Count == 0)
        {
            throw new ArgumentException("Invalid JSON format");
        }

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression combinedExpression = null;

        void AddCondition(Expression condition)
        {
            combinedExpression = combinedExpression == null
                ? condition
                : Expression.AndAlso(combinedExpression, condition);
        }

        // Check if the first element is a JArray or not
        if (jArray[0] is JArray)
        {
            for (int i = 0; i < jArray.Count; i++)
            {
                if (jArray[i] is JArray condition)
                {
                    var propertyName = condition[0].ToString();
                    var operation = condition[1].ToString();
                    var value = condition[2].ToString();

                    var property = Expression.Property(parameter, propertyName);
                    //var propertyValue = Convert.ChangeType(value, property.Type);
                    var constant = Expression.Constant(value);

                    Expression comparison = operation switch
                    {
                        "contains" => Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) }), constant),
                        "notcontains" => Expression.Not(Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) }), constant)),
                        "startswith" => Expression.Call(property, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), constant),
                        "endswith" => Expression.Call(property, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), constant),
                        "equals" => Expression.Equal(property, constant),
                        "=" => Expression.Equal(property, constant),
                        "<>" => Expression.NotEqual(property, constant),
                        ">" => Expression.GreaterThan(property, constant),
                        ">=" => Expression.GreaterThanOrEqual(property, constant),
                        "<" => Expression.LessThan(property, constant),
                        "<=" => Expression.LessThanOrEqual(property, constant),
                        _ => throw new ArgumentException("Unsupported operation: " + operation),
                    };

                    AddCondition(comparison);
                }
            }
        }
        else
        {
            var propertyName = jArray[0].ToString();
            var operation = jArray[1].ToString();
            var value = jArray[2].ToString();

            var property = Expression.Property(parameter, propertyName);
            //var propertyValue = Convert.ChangeType(value, property.Type);
            var constant = Expression.Constant(value);

            Expression comparison = operation switch
            {
                "contains" => Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) }), constant),
                "notcontains" => Expression.Not(Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) }), constant)),
                "startswith" => Expression.Call(property, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), constant),
                "endswith" => Expression.Call(property, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), constant),
                "equals" => Expression.Equal(property, constant),
                "=" => Expression.Equal(property, constant),
                "<>" => Expression.NotEqual(property, constant),
                ">" => Expression.GreaterThan(property, constant),
                ">=" => Expression.GreaterThanOrEqual(property, constant),
                "<" => Expression.LessThan(property, constant),
                "<=" => Expression.LessThanOrEqual(property, constant),
                _ => throw new ArgumentException("Unsupported operation: " + operation),
            };

            AddCondition(comparison);
        }

        return Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
    }


    /*
    public static Expression<Func<T, bool>> BuildPredicateFromJson(string json)
    {
        var jArray = JsonConvert.DeserializeObject<JArray>(json);

        if (jArray == null || jArray.Count == 0)
            throw new ArgumentException("Invalid JSON format");

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression combinedExpression = null;

        for (int i = 0; i < jArray.Count; i += 2)
        {
            var condition = jArray[i] as JArray;
            if (condition == null)
                throw new ArgumentException("Invalid condition format in JSON");

            var propertyName = condition[0].ToString();
            var operation = condition[1].ToString();
            var value = condition[2].ToString();

            var property = Expression.Property(parameter, propertyName);
            var propertyValue = Convert.ChangeType(value, property.Type);
            var constant = Expression.Constant(propertyValue);

            Expression comparison = operation switch
            {
                "contains" => Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) }), constant),
                "=" => Expression.Equal(property, constant),
                // Add more cases for other operations as needed
                _ => throw new ArgumentException("Unsupported operation: " + operation),
            };

            combinedExpression = combinedExpression == null
                ? comparison
                : Expression.AndAlso(combinedExpression, comparison);
        }

        return Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
    }
    */
    public static Expression<Func<T, bool>> BuildPredicateFromJsonx(string json)
    {
        var tokens = json.Trim('[', ']').Split(",", StringSplitOptions.RemoveEmptyEntries);

        //if (tokens.Length % 2 != 1)
        //    throw new ArgumentException("Invalid JSON format");

        return BuildPredicate(tokens);
    }

    private static Expression<Func<T, bool>> BuildPredicate(string[] tokens)
    {
        Expression<Func<T, bool>> predicate = null;

        for (int i = 0; i < tokens.Length; i += 2)
        {
            var propertyName = tokens[i].Trim('"');
            var operation = tokens[i + 1].Trim('"');
            var value = RemoveNonAlphanumeric(tokens[i + 2]);

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);
            var propertyValue = Convert.ChangeType(value, property.Type);
            var constant = Expression.Constant(propertyValue);

            Expression comparison;

            switch (operation)
            {
                case "contains":
                    comparison = Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) }), constant);
                    break;
                case "=":
                    comparison = Expression.Equal(property, constant);
                    break;
                // Add more cases for other operations as needed
                default:
                    throw new ArgumentException("Unsupported operation: " + operation);
            }

            var body = predicate != null ?
                Expression.AndAlso(predicate.Body, comparison) :
                comparison;

            predicate = Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        // If 'and' operator is present, combine predicates with AndAlso
        if (tokens.Contains("and"))
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var body = Expression.AndAlso(predicate.Body, predicate.Body);
            predicate = Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        return predicate;
    }

    private static string RemoveNonAlphanumeric(string input)
    {
        return new string(input.Where(char.IsLetterOrDigit).ToArray());
    }
}
