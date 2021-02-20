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
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class LivrosController : ControllerBase
    {
        #region Informações
        /*
         - O framework retorna json automagicamente;

         - ControllerBase, específico para API, nos permite utilizar os retornos Ok, NotFound e etc...

         - Padronização das rotas, o próprio método HTTP serve como base, evitando a criação de documentações para explicar como consumir a API;

         - Versionamento: por rota, querystring e header;
            Rota: [Route("api/v{version:apiVersion}/[controller]")], não dá flexibilidade de utilizar outros modos
            QueryString: http://localhost:6000/api/Livros/1?api-version=1.0
            QueryString e Header: podem ser utilizados simultaneamente
        */
        #endregion

        private readonly IRepository<Livro> _repo;

        public LivrosController(IRepository<Livro> repository)
        {
            _repo = repository;
        }


        [HttpGet("{id}")]
        [ProducesResponseType(statusCode: 200, Type = typeof(LivroApi))]
        [ProducesResponseType(statusCode: 404)]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        [SwaggerOperation(
            Summary = "Recupera o livro identificado pelo id.",
            Produces = new[] { "application/json", "application/xml" },
            Tags = new[] { "Livros" }
        )]
        public IActionResult Recuperar([FromRoute][SwaggerParameter("Id do livro a ser obtido.")] int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            //return Json(model.ToModel());
            return Ok(model.ToApi());
        }

        [HttpGet("{id}/capa")]
        [ProducesResponseType(statusCode: 200, Type = typeof(FileContentResult))]
        [ProducesResponseType(statusCode: 404)]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        [SwaggerOperation(
            Summary = "Recupera a capa do livro identificado pelo id.",
            Produces = new[] { "application/json", "application/xml" },
            Tags = new[] { "Livros" }
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
        [ProducesResponseType(statusCode: 200, Type = typeof(List<LivroApi>))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        [SwaggerOperation(
            Summary = "Recupera a lista de livros.",
            Produces = new[] { "application/json", "application/xml" },
            Tags = new[] { "Livros" }
        )]
        public IActionResult ListaDeLivros()
        {
            var lista = _repo.All.Select(x => x.ToApi()).ToList();
            return Ok(lista);
        }


        [HttpPost]
        [ProducesResponseType(statusCode: 200, Type = typeof(Livro))]
        [ProducesResponseType(statusCode: 400)]
        [SwaggerOperation(
            Summary = "Inclui novo livro.",
            Produces = new[] { "application/json", "application/xml" },
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
            return BadRequest();//400
        }

        [HttpPut]
        [ProducesResponseType(statusCode: 204)]
        [ProducesResponseType(statusCode: 400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(statusCode: 404)]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        [SwaggerOperation(
            Summary = "Altera o livro enviado no body da requisição.",
            Produces = new[] { "application/json", "application/xml" },
            Tags = new[] { "Livros" }
        )]
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
        [ProducesResponseType(statusCode: 204)]
        [ProducesResponseType(statusCode: 404)]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        [SwaggerOperation(
            Summary = "Exclui o livro identificado pelo id.",
            Produces = new[] { "application/json", "application/xml" },
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
