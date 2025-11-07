using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace DavidStudio.Core.Swagger.Attributes;

/// <summary>
/// Provides ordering logic for Swagger controllers based on the <see cref="SwaggerControllerOrderAttribute"/>.
/// </summary>
/// <typeparam name="T">The base controller type, usually <see cref="ControllerBase"/>.</typeparam>
/// <remarks>
/// This class reads <see cref="SwaggerControllerOrderAttribute"/> from controller types and generates
/// a sortable key that can be used to order controllers consistently in Swagger UI.
/// </remarks>
internal class SwaggerControllerOrder<T>
{
    private readonly Dictionary<string, uint> _orders;

    /// <summary>
    /// Initializes a new instance of <see cref="SwaggerControllerOrder{T}"/> by scanning an assembly for controllers.
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly"/> to scan for controllers.</param>
    public SwaggerControllerOrder(Assembly assembly)
        : this(GetFromAssembly<T>(assembly)) { }

    /// <summary>
    /// Initializes a new instance of <see cref="SwaggerControllerOrder{T}"/> using a collection of controller types.
    /// </summary>
    /// <param name="controllers">The controller types to consider for ordering.</param>
    /// <remarks>
    /// Only controllers decorated with <see cref="SwaggerControllerOrderAttribute"/> are included in the ordering dictionary.
    /// The controller name is normalized by removing the "Controller" suffix (case-insensitive) before storing its order.
    /// </remarks>
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

    /// <summary>
    /// Retrieves all types assignable to the specified controller type <typeparamref name="TController"/> from the given assembly.
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly"/> to scan.</param>
    /// <returns>An <see cref="IEnumerable{Type}"/> of controller types assignable to <typeparamref name="TController"/>.</returns>
    private static IEnumerable<Type> GetFromAssembly<TController>(Assembly assembly)
    {
        return assembly.GetTypes().Where(c => typeof(TController).IsAssignableFrom(c));
    }

    /// <summary>
    /// Resolves a controller name by removing the "Controller" suffix, if present.
    /// </summary>
    /// <param name="name">The controller type name.</param>
    /// <returns>The normalized controller name.</returns>
    private static string ResolveControllerName(string name)
    {
        const string suffix = "Controller";

        return name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)
            ? name.Substring(0, name.Length - suffix.Length)
            : name;
    }

    /// <summary>
    /// Generates a zero-padded string key for the controller based on its order.
    /// </summary>
    /// <param name="controller">The normalized controller name.</param>
    /// <returns>A string representation of the order value, zero-padded to 10 digits.</returns>
    private string OrderKey(string? controller)
    {
        return controller is not null
            ? _orders.GetValueOrDefault(controller, uint.MaxValue).ToString("D10")
            : uint.MaxValue.ToString("D10");
    }

    /// <summary>
    /// Generates a sortable key for a controller that combines its order and name.
    /// </summary>
    /// <param name="controller">The normalized controller name.</param>
    /// <returns>A string key in the format "order_controllerName", suitable for sorting in Swagger UI.</returns>
    public string SortKey(string? controller) => $"{OrderKey(controller)}_{controller}";
}