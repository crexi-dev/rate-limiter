using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Example;

public class CustomHeaderSwaggerAttribute : IOperationFilter
{
     
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();
 
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "client-id",
            In = ParameterLocation.Header,
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = "string" 
            }
        });
        
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "region",
            In = ParameterLocation.Header,
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = "string" 
            }
        });
    }
 
}