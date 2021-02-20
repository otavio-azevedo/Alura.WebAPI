using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.Api.Formatters
{
    //Possibilita ao usuário o retorno dos recursos no formato csv
    public class LivroCsvFormatter : TextOutputFormatter
    {
        public LivroCsvFormatter()
        {
            var textCsvMediaType = MediaTypeHeaderValue.Parse("text/csv");
            var appCsvMediaType = MediaTypeHeaderValue.Parse("application/csv");
            SupportedMediaTypes.Add(textCsvMediaType);
            SupportedMediaTypes.Add(appCsvMediaType);
            SupportedEncodings.Add(Encoding.UTF8);
        }

        //Se o cliente enviar uma requisição solicitando um formato customizado para uma classe que não criamos
        //como por exemplo a ListaLeitura, a api retornará JSON
        protected override bool CanWriteType(Type type)
        {
                            //Tipo aceito
            return type == typeof(LivroApi);
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var livroEmCsv = "";

            if (context.Object is LivroApi)
            {
                var livro = context.Object as LivroApi;

                livroEmCsv = $"{livro.Titulo};{livro.Subtitulo};{livro.Autor};{livro.Lista}";
            }
            

            using(var escritor = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding))
            {
                return escritor.WriteAsync(livroEmCsv);
            }//escritor.Close() -> neste formato de using é chamado implicitamente
        }
    }
}
