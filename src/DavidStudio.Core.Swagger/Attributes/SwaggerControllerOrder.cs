using System.Reflection;

namespace DavidStudio.Core.Swagger.Attributes;

internal class SwaggerControllerOrder<T>
{
    private readonly Dictionary<string, uint> _orders;

    public SwaggerControllerOrder(Assembly assembly)
        : this(GetFromAssembly<T>(assembly))
    {
    }

    public SwaggerControllerOrder(IEnumerable<Type> controllers)
    {
        _orders = new Dictionary<string, uint>(controllers
            .Where(c => c.GetCustomAttributes<SwaggerControllerOrderAttribute>().Any())
            .Select(c => new
            {
                Name = ResolveControllerName(c.Name),
                c.GetCustomAttribute<SwaggerControllerOrderAttribute>()!.Order
            })
            .ToDictionary(v => v.Name, v => v.Order), StringComparer.OrdinalIgnoreCase);
    }

    public static IEnumerable<Type> GetFromAssembly<TController>(Assembly assembly)
    {
        return assembly.GetTypes().Where(c => typeof(TController).IsAssignableFrom(c));
    }

    private static string ResolveControllerName(string name)
    {
        const string suffix = "Controller";

        return name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)
            ? name.Substring(0, name.Length - suffix.Length)
            : name;
    }

    private string OrderKey(string? controller)
    {
        return controller is not null
            ? _orders.GetValueOrDefault(controller, uint.MaxValue).ToString("D10")
            : uint.MaxValue.ToString("D10");
    }

    public string SortKey(string? controller) => $"{OrderKey(controller)}_{controller}";
}