using Alura.ListaLeitura.Seguranca;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Alura.ListaLeitura.HttpClients;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Alura.ListaLeitura.WebApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //Necessário para acessar o contexto da requisição em classes fora do framework (no nosso caso, para acessar o token)
            services.AddHttpContextAccessor();

            //Configura autenticação via cookie
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                    options.LoginPath = "/Usuario/Login");


            //IHttpClientFactory
            //Define propriedades das classes dos clientes http customizados
            services.AddHttpClient<LivroApiClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:6001/api/v1.0/");
            });

            services.AddHttpClient<AuthApiClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");
            });

            services.AddMvc();


            //AddXmlSerializerFormatters
            //Adiciona o recurso de serialização XML no framework, nativamente possui apenas JSON
            //Possibilitando ao cliente trabalhar em XML, desde que envie o request com o header Accept - text/xml ou application/xml
            //services.AddMvc().AddXmlSerializerFormatters();

            //Adicionar os formatos customizados de resposta das requisições
            //Possibilitando ao cliente trabalhar em csv, desde que envie o request com o header Accept - text/csv ou application/csv
            //services.AddMvc(options =>
            //{
            //    options.OutputFormatters.Add(new LivroCsvFormatter());
            //})
            //        .AddXmlSerializerFormatters();




        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
