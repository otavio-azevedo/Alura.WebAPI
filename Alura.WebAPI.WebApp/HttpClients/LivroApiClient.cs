using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.ListaLeitura.HttpClients
{
    public class LivroApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _accessor;

        public LivroApiClient(HttpClient httpClient, IHttpContextAccessor accessor)
        {
            _httpClient = httpClient;
            _accessor = accessor;
        }

        public async Task DeleteLivroAsync(int id)
        {
            AddBearerToken();
            HttpResponseMessage response = await _httpClient.DeleteAsync($"livros/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<Lista> GetListaLeituraAsync(TipoListaLeitura tipo)
        {
            AddBearerToken();
            HttpResponseMessage response = await _httpClient.GetAsync($"listasleitura/{tipo}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<Lista>();
        }

        public async Task<byte[]> GetCapaLivroAsync(int id)
        {
            AddBearerToken();
            HttpResponseMessage response = await _httpClient.GetAsync($"livros/{id}/capa");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<LivroApi> GetLivroAsync(int id)
        {
            AddBearerToken();
            HttpResponseMessage response = await _httpClient.GetAsync($"livros/{id}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<LivroApi>();
        }

        public async Task PostLivroAsync(LivroUpload model)
        {
            AddBearerToken();
            HttpContent content = CreateMultipartFormDataContent(model);
            HttpResponseMessage response = await _httpClient.PostAsync($"livros", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task PutLivroAsync(LivroUpload model)
        {
            AddBearerToken();
            HttpContent content = CreateMultipartFormDataContent(model);
            HttpResponseMessage response = await _httpClient.PutAsync($"livros", content);
            response.EnsureSuccessStatusCode();
        }

        private HttpContent CreateMultipartFormDataContent(LivroUpload model)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(model.Titulo), EnvolveComAspasDuplas(nameof(model.Titulo)));
            content.Add(new StringContent(model.Lista.ParaString()), EnvolveComAspasDuplas(nameof(model.Lista)));

            if (!string.IsNullOrEmpty(model.Subtitulo))
            {
                content.Add(new StringContent(model.Subtitulo), EnvolveComAspasDuplas(nameof(model.Subtitulo)));
            }

            if (!string.IsNullOrEmpty(model.Resumo))
            {
                content.Add(new StringContent(model.Resumo), EnvolveComAspasDuplas(nameof(model.Resumo)));
            }

            if (!string.IsNullOrEmpty(model.Autor))
            {
                content.Add(new StringContent(model.Autor), EnvolveComAspasDuplas(nameof(model.Autor)));
            }

            if (model.Id > 0)
            {
                content.Add(new StringContent(model.Id.ToString()), EnvolveComAspasDuplas(nameof(model.Id)));
            }

            if (model.Capa != null)
            {
                var imageContent = new ByteArrayContent(model.Capa.ConvertToBytes());
                imageContent.Headers.Add("content-type", "image/png");
                content.Add(
                    imageContent,
                    EnvolveComAspasDuplas("capa"),
                    EnvolveComAspasDuplas("nomeDaCapa.png")
                    );
            }

            return content;
        }

        private string EnvolveComAspasDuplas(string campo)
        {
            return $"\"{campo.ToLower()}\"";
        }

        /// <summary>
        /// Adicionar o token do contexto ao header da requisição
        /// </summary>
        private void AddBearerToken()
        {
            var token = _accessor.HttpContext.User.Claims.First(x => x.Type == "Token").Value;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
        }
    }
}
