using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Alura.ListaLeitura.HttpClients;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.WebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        //private readonly IRepository<Livro> _repo;
        private readonly LivroApiClient _apiClient;

        public HomeController(LivroApiClient livroApiClient)
        {
            //_repo = repository;
            _apiClient = livroApiClient;
        }

        private async Task<IEnumerable<LivroApi>> ListaDoTipo(TipoListaLeitura tipo)
        {
            /*
            return _repo.All
                .Where(l => l.Lista == tipo)
                .Select(l => l.ToApi())
                .ToList();
            */

            var lista = await _apiClient.GetListaLeituraAsync(tipo);
            return lista.Livros;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel
            {
                ParaLer = await ListaDoTipo(TipoListaLeitura.ParaLer),
                Lendo = await ListaDoTipo(TipoListaLeitura.Lendo),
                Lidos = await ListaDoTipo(TipoListaLeitura.Lidos)
            };
            return View(model);
        }
    }
}