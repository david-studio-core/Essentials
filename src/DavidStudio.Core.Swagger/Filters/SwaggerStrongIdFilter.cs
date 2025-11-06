using System.ComponentModel;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DavidStudio.Core.Swagger.Filters;

public class SwaggerStrongIdFilter : ISchemaFilter
{
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