using Microsoft.OpenApi.Models;

namespace OpenApi.Generator
{
    internal static class OpenApiExtensions
    {
        public static string ToCsharpType(this OpenApiSchema schema)
        {
            if (schema.Reference != null)
            {
                return schema.Reference.Id;
            }

            return schema.Type switch
            {
                "string" => "string",
                "integer" => schema.Format == "int64" ? "long" : "int",
                "boolean" => "bool",
                "array" => $"List<{ToCsharpType(schema.Items)}>",
                _ => "object",
            };
        }
    }
}
