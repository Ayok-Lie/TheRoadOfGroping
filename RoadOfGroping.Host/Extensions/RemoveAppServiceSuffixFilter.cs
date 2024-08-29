using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class RemoveAppSuffixFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // 去掉控制器分类中的 "App" 后缀
        foreach (var path in swaggerDoc.Paths.Values)
        {
            foreach (var operation in path.Operations.Values)
            {
                var tags = operation.Tags.Select(tag => new OpenApiTag
                {
                    Name = tag.Name.Replace("AppService", "", StringComparison.OrdinalIgnoreCase),
                    Description = tag.Description
                }).ToList();

                operation.Tags.Clear();
                foreach (var tag in tags)
                {
                    operation.Tags.Add(tag);
                }
            }
        }
    }
}
