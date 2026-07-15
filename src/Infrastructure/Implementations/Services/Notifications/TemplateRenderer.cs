using System.Collections.Concurrent;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Infrastructure;

public static partial class TemplateRenderer
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new();

    public static string Render(string template, object data)
    {
        if (string.IsNullOrEmpty(template))
            return template;

        var properties = PropertyCache.GetOrAdd(
            data.GetType(),
            t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance));

        return PlaceholderRegex().Replace(template, match =>
        {
            var key = match.Groups[1].Value;
            var property = properties.FirstOrDefault(p =>
                string.Equals(p.Name, key, StringComparison.OrdinalIgnoreCase));

            if (property == null)
                return string.Empty;

            var value = property.GetValue(data);
            return value?.ToString() ?? string.Empty;
        });
    }

    [GeneratedRegex(@"\{\{(\w+)\}\}", RegexOptions.Compiled)]
    private static partial Regex PlaceholderRegex();
}