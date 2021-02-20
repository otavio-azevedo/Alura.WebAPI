using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Alura.WebAPI.Api.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.ListaLeitura.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ListasLeituraController : ControllerBase
    {
        private readonly IRepository<Livro> _repo;

        public ListasLeituraController(IRepository<Livro> repository)
        {
            _repo = repository;
        }

        private Lista CriaLista(TipoListaLeitura tipo)
        {
            return new Lista
            {
                Tipo = tipo.ParaString(),
                Livros = _repo.All
                        .Where(x => x.Lista == tipo)
                        .Select(x=>x.ToApi())
                        .ToList()
            };

        }

        [HttpGet]
        [ProducesResponseType(statusCode: 200, Type = typeof(List<Lista>))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        [SwaggerOperation(
            Summary = "Retorna todas listas de leitura.",
            Produces = new[] { "application/json", "application/xml" },
            Tags = new[] { "Listas" }
        )]
        public IActionResult TodasListas()
        {
            Lista paraLer = CriaLista(TipoListaLeitura.ParaLer);
            Lista lendo = CriaLista(TipoListaLeitura.Lendo);
            Lista lidos = CriaLista(TipoListaLeitura.Lidos);

            return Ok(new List<Lista> { paraLer, lendo, lidos });
        }

        [HttpGet("{tipo}")]
        [ProducesResponseType(statusCode: 200, Type = typeof(Lista))]
        [ProducesResponseType(statusCode: 404)]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        [SwaggerOperation(
            Summary = "Retorna uma lista de leitura identificada pelo tipo.",
            Produces = new[] { "application/json", "application/xml" },
            Tags = new[] { "Listas" }
        )]
        public IActionResult Recuperar([FromRoute][SwaggerParameter("Tipo da lista a ser obtida.")] TipoListaLeitura tipo)
        {
            var lista = CriaLista(tipo);
            return Ok(lista);
        }
    }
}
