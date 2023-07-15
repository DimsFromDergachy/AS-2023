using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AS2023Env;

public class SwaggerTagFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach(ApiDescription contextApiDescription in context.ApiDescriptions)
        {
            if (!Constants.IsAdmin)
            {
                var actionDescriptor = (ControllerActionDescriptor) contextApiDescription.ActionDescriptor;
                SwaggerTagAttribute attribute = 
                    actionDescriptor.ControllerTypeInfo.GetCustomAttributes<SwaggerTagAttribute>().FirstOrDefault();

                bool hide = attribute == null 
                            || Constants.IsTest != (attribute.Description == Constants.TestControllerDescription);
                if (hide)
                {
                    string key = "/" + contextApiDescription.RelativePath!.TrimEnd('/');
                    swaggerDoc.Paths.Remove(key);
                }
            }
        }
    }
}