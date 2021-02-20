using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Alura.ListaLeitura.HttpClients;

namespace Alura.ListaLeitura.WebApp.Controllers
{
    [Authorize]
    public class LivroController : Controller
    {
        //private readonly IRepository<Livro> _repo;
        private readonly LivroApiClient _apiClient;

        public LivroController(LivroApiClient livroApiClient)
        {
            //_repo = repository;
            _apiClient = livroApiClient;
        }

        [HttpGet]
        public IActionResult Novo()
        {
            return View(new LivroUpload());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Novo(LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                // _repo.Incluir(model.ToLivro());
                await _apiClient.PostLivroAsync(model);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ImagemCapa(int id)
        {
            /*
            byte[] img = _repo.All
                .Where(l => l.Id == id)
                .Select(l => l.ImagemCapa)
                .FirstOrDefault();
            */

            //HttpClient httpClient = new HttpClient();
            //httpClient.BaseAddress = new System.Uri("http://localhost:6000/api/");
            //HttpResponseMessage response = await httpClient.GetAsync($"livros/{id}/capa");
            //response.EnsureSuccessStatusCode();//Se a resposta fora diferente de sucesso, lança exceção
            //byte[] img = await response.Content.ReadAsByteArrayAsync();

            byte[] img = await _apiClient.GetCapaLivroAsync(id);

            if (img != null)
            {
                return File(img, "image/png");
            }
            return File("~/images/capas/capa-vazia.png", "image/png");
        }

        [HttpGet]
        public async Task<IActionResult> Detalhes(int id)
        {
            //var model = _repo.Find(id);

            //HttpClient httpClient = new HttpClient();
            //httpClient.BaseAddress = new System.Uri("http://localhost:6000/api/");
            //HttpResponseMessage response = await httpClient.GetAsync($"livros/{id}");
            //response.EnsureSuccessStatusCode();//Se a resposta fora diferente de sucesso, lança exceção

            //var model = await response.Content.ReadAsAsync<LivroApi>();

            var model = await _apiClient.GetLivroAsync(id);

            if (model == null)
            {
                return NotFound();
            }
            return View(model.ToUpload());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Detalhes(LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                //var livro = model.ToLivro();
                //if (model.Capa == null)
                //{
                //    livro.ImagemCapa = _repo.All
                //        .Where(l => l.Id == livro.Id)
                //        .Select(l => l.ImagemCapa)
                //        .FirstOrDefault();
                //}
                //_repo.Alterar(livro);

                await _apiClient.PutLivroAsync(model);

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remover(int id)
        {
            //var model = _repo.Find(id);
            var model = _apiClient.GetLivroAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            //_repo.Excluir(model);
            await _apiClient.DeleteLivroAsync(id);

            return RedirectToAction("Index", "Home");
        }

        
    }
}