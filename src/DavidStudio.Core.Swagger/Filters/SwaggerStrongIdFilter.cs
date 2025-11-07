using System.ComponentModel;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DavidStudio.Core.Swagger.Filters;

/// <summary>
/// A Swagger <see cref="ISchemaFilter"/> that customizes the OpenAPI schema representation for "strongly-typed" ID types.
/// </summary>
/// <remarks>
/// <para>
/// This filter detects types whose names end with "Id" and that have a <see cref="TypeConverterAttribute"/>. 
/// If the type's converter can convert to <see cref="Guid"/>, <see cref="string"/>, <see cref="int"/>, or <see cref="long"/>, 
/// the OpenAPI schema type and example are automatically adjusted.
/// </para>
/// <para>
/// The filter logic is as follows:
/// <list type="bullet">
/// <item>Checks if the type name ends with "Id".</item>
/// <item>Ensures the type is a value type.</item>
/// <item>Retrieves the <see cref="TypeConverterAttribute"/> applied to the type.</item>
/// <item>Instantiates the type converter and checks if it can convert to <see cref="Guid"/>, <see cref="string"/>, <see cref="int"/>, or <see cref="long"/>.</item>
/// <item>If conversion is supported, updates schema with the appropriate OpenAPI type and example value.</item>
/// </list>
/// </para>
/// </remarks>
public class SwaggerStrongIdFilter : ISchemaFilter
{
    /// <summary>
    /// Applies the schema filter to modify the OpenAPI schema for supported "strongly-typed" ID types.
    /// </summary>
    /// <param name="schema">The <see cref="OpenApiSchema"/> representing the current type in the Swagger documentation.</param>
    /// <param name="context">The <see cref="SchemaFilterContext"/> providing contextual information about the type being processed.</param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.Name.EndsWith("Id") ||
            !context.Type.IsValueType ||
            context.Type.GetCustomAttribute(typeof(TypeConverterAttribute)) is not TypeConverterAttribute attr ||
            Type.GetType(attr.ConverterTypeName) is not { } type)
        {
            return;
        }

        if (Activator.CreateInstance(type) is not TypeConverter converter) return;

        if (converter.CanConvertTo(typeof(Guid)) || converter.CanConvertTo(typeof(string)))
        {
            schema.Type = "string";
            schema.Example = new OpenApiString(Guid.NewGuid().ToString());
        }
        else if (converter.CanConvertTo(typeof(int)) || converter.CanConvertTo(typeof(long)))
        {
            schema.Type = "integer";
            schema.Example = new OpenApiInteger(0);
        }
    }
}