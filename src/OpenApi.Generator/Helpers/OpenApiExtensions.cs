using Microsoft.OpenApi.Models;

namespace OpenApi.Generator
{
    internal static class OpenApiExtensions
    {
        public static string ToCsharpType(this OpenApiSchema schema)
        {
            var csharpType = schema.Type switch
            {
                "string" => "string",
                "integer" => schema.Format == "int64" ? "long" : "int",
                "boolean" => "bool",
                _ => "object",
            };

            if (schema.Nullable)
            {
                csharpType += "?";
            }

            return csharpType;
        }
    }
}
