using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.OpenApi.Models;
using Scriban;
using System;
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
            var templateRaw = TemplateExtensions.Get("ControllerTemplate");
            var template = Template.Parse(templateRaw);

            var groupedPaths = _document.Paths
                .GroupBy(p => GetControllerName(p.Key))
                .ToList();

            foreach (var group in groupedPaths)
            {
                var controllerName = group.Key.ToPascalCase();
                var operations = group
                    .SelectMany(g => g.Value.Operations, (path, op) => new
                    {
                        Method = op.Key.ToString(),
                        ActionName = GetActionName(op.Key, op.Value.Parameters.Any()),
                        Parameters = op.Value.Parameters.Select(p => new { p.Name, Type = p.Schema.ToCsharpType() }).ToList(),
                        RequestBodyType = ExtractRequestBodyType(op.Value),
                        ResponseType = ExtractResponseType(op.Value),
                        Path = path.Key
                    })
                    .ToList();

                var model = new
                {
                    ControllerName = controllerName,
                    Operations = operations
                };

                var result = template.Render(model, member => member.Name);

                context.AddSource($"{controllerName}Controller.g.cs", SourceText.From(result, Encoding.UTF8));
            }
        }

        private string GetControllerName(string path)
        {
            return path.Trim('/').Split('/')[0];
        }

        private string ExtractRequestBodyType(OpenApiOperation op)
        {
            return op.RequestBody?.Content?.Values?.FirstOrDefault()?.Schema?.Reference?.Id?.Split('/')?.Last();
        }

        private string ExtractResponseType(OpenApiOperation op)
        {
            if (op.Responses.TryGetValue("200", out var response))
            {
                var contentValue = response.Content?.Values?.FirstOrDefault();

                if (contentValue != null)
                {
                    return contentValue.Schema?.Items?.Reference?.Id ?? contentValue.Schema?.Reference?.Id;
                }
            }

            return null;
        }

        private string GetActionName(OperationType operationType, bool hasParameters)
        {
            return operationType switch
            {
                OperationType.Get => hasParameters ? "GetById" : "GetAll",
                OperationType.Post => "Create",
                OperationType.Put => "Update",
                OperationType.Patch => "Patch",
                OperationType.Delete => "Delete",
                _ => throw new ArgumentOutOfRangeException(nameof(operationType), $"Not expected operationType value: {operationType}"),
            };
        }
    }
}