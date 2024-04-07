using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.OpenApi.Models;
using Scriban;
using System.Linq;
using System.Text;

namespace OpenApi.Generator
{
    internal class ControllerGenerator
    {
        private readonly OpenApiDocument _document;

        public ControllerGenerator(OpenApiDocument document)
        {
            _document = document;
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var templateRaw = Templates.Get("ControllerTemplate");
            var template = Template.Parse(templateRaw);

            foreach (var path in _document.Paths)
            {
                var controllerName = GetControllerName(path.Key);

                var operations = path.Value.Operations
                    .Select(kv => new
                    {
                        method = kv.Key.ToString(),
                        action_name = GetActionName(kv.Key)
                    })
                    .ToList();

                var model = new
                {
                    controllerName,
                    operations
                };

                var result = template.Render(model, member => member.Name);

                context.AddSource($"{controllerName}Controller.g.cs", SourceText.From(result, Encoding.UTF8));
            }
        }

        private string GetControllerName(string path)
        {
            return path.Trim('/').Split('/')[0];
        }

        private string GetActionName(OperationType operationType)
        {
            return operationType switch
            {
                OperationType.Get => "Get",
                OperationType.Post => "Create",
                OperationType.Put => "Update",
                OperationType.Patch => "Patch",
                OperationType.Delete => "Delete",
                _ => "Action"
            };
        }
    }
}
