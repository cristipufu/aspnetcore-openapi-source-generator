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
            var templateRaw = TemplateExtensions.Get("ModelTemplate");
            var template = Template.Parse(templateRaw);

            foreach (var schema in _document.Components.Schemas)
            {
                var className = schema.Key;
                var properties = schema.Value.Properties.Select(p => new
                {
                    name = p.Key,
                    type = p.Value.ToCsharpType(),
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
    }
}
