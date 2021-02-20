using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.WebAPI.Api.Filters
{
    public class AuthResponsesOperationFilter : IOperationFilter
    {
        //SWAGGER
        //Isso fará com que a documentação de cada operação tenha mais uma resposta 401 com a descrição "Unauthorized".
        public void Apply(Operation operation, OperationFilterContext context)
        {
            operation.Responses.Add(
                "401",
                new Response { Description = "Unauthorized" }
            );
        }
    }
}
