using Alura.ListaLeitura.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Alura.WebAPI.Api.Modelos
{
    public class LivroOrdem
    {
        public string OrdernarPor { get; set; }

    }
        public static class LivroOrdemExtensions
        {
            public static IQueryable<Livro> AplicaOrdem(this IQueryable<Livro> query, LivroOrdem ordem)
            {
                if (!string.IsNullOrEmpty(ordem.OrdernarPor))
                {
                    query = query.OrderBy(ordem.OrdernarPor);
                }

                return query;
            }
        }
}
