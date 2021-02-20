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

namespace Alura.ListaLeitura.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "v2")]
    //[Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/livros")]
    public class Livros2Controller : ControllerBase
    {
        #region Informações
        /*
         - O framework retorna json automagicamente;

         - ControllerBase, específico para API, nos permite utilizar os retornos Ok, NotFound e etc...

         - Padronização das rotas, o próprio método HTTP serve como base, evitando a criação de documentações para explicar como consumir a API;

         - Versionamento: por rota, querystring e header;
            Rota: [Route("api/v{version:apiVersion}/[controller]")], não dá flexibilidade de utilizar outros modos
            QueryString e Header: podem ser utilizados simultaneamente
        */
        #endregion

        private readonly IRepository<Livro> _repo;

        public Livros2Controller(IRepository<Livro> repository)
        {
            _repo = repository;
        }


        [HttpGet("{id}")]
        [ProducesResponseType(statusCode: 200, Type = typeof(LivroApi))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404)]
        [SwaggerOperation(
            Summary = "Recupera o livro identificado por seu {id}.",
            Tags = new[] { "Livros" },
            Produces = new[] { "application/json", "application/xml" }
        )]
        public IActionResult Recuperar(int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            //return Json(model.ToModel());
            return Ok(model);
        }

        [HttpGet("{id}/capa")]
        [SwaggerOperation(
            Summary = "Recupera a capa do livro identificado por seu {id}.",
            Tags = new[] { "Livros" },
            Produces = new[] { "image/png" }
        )]
        public IActionResult ImagemCapa(int id)
        {
            byte[] img = _repo.All
                .Where(l => l.Id == id)
                .Select(l => l.ImagemCapa)
                .FirstOrDefault();
            if (img != null)
            {
                return File(img, "image/png");
            }
            return File("~/images/capas/capa-vazia.png", "image/png");
        }

        [HttpGet]
        [ProducesResponseType(statusCode: 200, Type = typeof(LivroPaginado))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        [ProducesResponseType(statusCode: 404)]
        [SwaggerOperation(
            Summary = "Recupera uma coleção paginada de livros.",
            Tags = new[] { "Livros" }
        )]
        public IActionResult ListaDeLivros(
            [FromQuery] LivroFiltro filtro,
            [FromQuery] LivroOrdem ordem,
            [FromQuery] LivroPaginacao paginacao)
        {
            var livroPaginado = _repo.All
                    .AplicaFiltro(filtro)
                    .AplicaOrdem(ordem) // http://localhost:6000/api/v2.0/Livros?lista=ParaLer&OrdenarPor=id asc
                    .Select(x => x.ToApi())
                    .ToLivroPaginado(paginacao); // http://localhost:6000/api/v2.0/Livros?tamanho=3&paginacao=1
            return Ok(livroPaginado);
        }


        [HttpPost]
        [ProducesResponseType(statusCode: 201, Type = typeof(LivroApi))]
        [ProducesResponseType(statusCode: 400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        [SwaggerOperation(
            Summary = "Registra novo livro na base.",
            Tags = new[] { "Livros" }
            )]
        public IActionResult Incluir([FromForm] LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                _repo.Incluir(livro);

                var uri = Url.Action("Recuperar", new { id = livro.Id });
                return Created(uri, livro);//201
            }
            return BadRequest(ErrorResponse.FromModelState(ModelState));
        }

        [HttpPut]
        [ProducesResponseType(statusCode: 200)]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(500, Type = typeof(ErrorResponse))]
        [SwaggerOperation(
            Summary = "Modifica o livro na base.",
            Tags = new[] { "Livros" })]
        public IActionResult Alterar([FromForm] LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                if (model.Capa == null)
                {
                    livro.ImagemCapa = _repo.All
                        .Where(l => l.Id == livro.Id)
                        .Select(l => l.ImagemCapa)
                        .FirstOrDefault();
                }
                _repo.Alterar(livro);
                return Ok();//200
            }
            return BadRequest();//400
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        [ProducesResponseType(500, Type = typeof(ErrorResponse))]
        [SwaggerOperation(
            Summary = "Exclui o livro da base.",
            Tags = new[] { "Livros" }
        )]
        public IActionResult Remover(int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            _repo.Excluir(model);
            return NoContent();//204
        }

    }
}
