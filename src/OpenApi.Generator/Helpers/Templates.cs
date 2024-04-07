using System.IO;
using System.Reflection;
using System.Text;

namespace OpenApi.Generator
{
    internal static class Templates
    {
        internal static string Get(string templateName)
        {
            string resourcePath = $"OpenApi.Generator.Templates.{templateName}.scriban";
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
            if (stream == null) return null;

            using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }
}
