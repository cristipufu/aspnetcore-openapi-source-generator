using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.OpenApi.Models;
using Scriban;
using System.Linq;
using System.Text;

namespace OpenApi.Generator
{
    internal class ModelGenerator
    {
        private readonly OpenApiDocument _document;

        public ModelGenerator(OpenApiDocument document)
        {
            _document = document;
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var templateRaw = Templates.Get("ModelTemplate");
            var template = Template.Parse(templateRaw);

            foreach (var schema in _document.Components.Schemas)
            {
                var className = schema.Key;
                var properties = schema.Value.Properties.Select(p => new
                {
                    name = p.Key,
                    type = ConvertType(p.Value)
                }).ToList();

                var model = new
                {
                    className,
                    properties
                };
                
                var result = template.Render(model, member => member.Name); 

                context.AddSource($"{className}.g.cs", SourceText.From(result, Encoding.UTF8));
            }
        }

        private string ConvertType(OpenApiSchema schema)
        {
            var csharpType = schema.Type switch
            {
                "string" => "string",
                "integer" => "int",
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
