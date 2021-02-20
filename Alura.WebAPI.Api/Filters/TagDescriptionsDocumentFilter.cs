using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.WebAPI.Api.Filters
{
    public class TagDescriptionsDocumentFilter : IDocumentFilter
    {

        /*
         E quando precisamos customizar a documentação gerada pelo SwaggerGen para adicionar ou modificar alguma informação?
         Para isso existe a interface IDocumentFilter. Vamos colocar uma descrição genérica em cada tag usando esse filtro.
         */
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Tags = new[] {
            new Tag { Name = "Livros", Description = "Consulta e mantém os livros." },
            new Tag { Name = "Listas", Description = "Consulta as listas de leitura." }
        };
        }
    }
}
