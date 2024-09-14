
using NSwag;
using NSwag.CodeGeneration.CSharp;

public class NSwagGen
{
    public static void Gen(string path, string name, string nameSpace)
    {
        OpenApiDocument document = OpenApiDocument.FromFileAsync(path).Result;        
        string documentPath = string.Empty;

        var settings = new CSharpClientGeneratorSettings
        {
            ClassName = name,
            CSharpGeneratorSettings =
            {
                Namespace = nameSpace,
                JsonLibrary = NJsonSchema.CodeGeneration.CSharp.CSharpJsonLibrary.SystemTextJson,
            }
        };

        var generator = new CSharpClientGenerator(document, settings);
        var code = generator.GenerateFile();
        File.WriteAllText("ServiceClient.cs", code);
    }
}