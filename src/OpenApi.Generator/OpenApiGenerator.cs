using Microsoft.CodeAnalysis;
using Microsoft.OpenApi.Readers;
using System;
using System.Linq;

namespace OpenApi.Generator
{
    [Generator]
    public class OpenApiGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) { }

        public void Execute(GeneratorExecutionContext context)
        {
            var openApiFile = context.AdditionalFiles.FirstOrDefault(at => at.Path.EndsWith(".yaml"));

            if (openApiFile == null)
            {
                return;
            }

            var content = openApiFile.GetText(context.CancellationToken)?.ToString();

            if (content == null)
            {
                return;
            }

            try
            {
                var openApiDocument = new OpenApiStringReader().Read(content, out var diagnostic);

                if (diagnostic.Errors.Count > 0)
                {
                    foreach (var error in diagnostic.Errors)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                            "OpenAPIParseError",
                            "Error parsing OpenAPI specification",
                            error.Message,
                            "OpenApi",
                            DiagnosticSeverity.Error,
                            true),
                            location: null));
                    }
                    return;
                }

                new ModelGenerator(openApiDocument).Execute(context);
                new ControllerGenerator(openApiDocument).Execute(context);
            }
            catch (Exception ex)
            {
                HandleGeneratorException(context, ex);
            }
        }

        private static void HandleGeneratorException(GeneratorExecutionContext context, Exception exception)
        {
            var diagnosticError = new DiagnosticDescriptor(
                    id: "OAG",
                    title: "OpenApi Source Generator Exception",
                    messageFormat: "Error from source generator: {0}. Stack trace: {1}",
                    category: "Generator",
                    defaultSeverity: DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description: ""
                );

            context.ReportDiagnostic(Diagnostic.Create(diagnosticError, Location.None, exception.Message, exception.StackTrace));
        }
    }
}