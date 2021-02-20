using Alura.ListaLeitura.Api.Formatters;
using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Alura.WebAPI.Api.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.WebAPI.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<LeituraContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("ListaLeitura"));
            });

            services.AddTransient<IRepository<Livro>, RepositorioBaseEF<Livro>>();

            services.AddMvc(options =>
            {
                //Adicionar os formatos customizados de resposta
                options.OutputFormatters.Add(new LivroCsvFormatter()); //Formato csv, basta solicitar no header da requisição
                options.Filters.Add(typeof(ErrorResponseFilter)); // Filtro de exceções não tratadas
            })
            .AddXmlSerializerFormatters(); //Recurso de serialização XML no framework, nativamente possui apenas JSON, , basta solicitar no header da requisição

            //Configuração da autenticação via jwt
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";

            }).AddJwtBearer("JwtBearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("alura-webapi-authentication-valid")),
                    ClockSkew = TimeSpan.FromMinutes(5),
                    ValidIssuer = "Alura.WebApp",
                    ValidAudience = "Postman"
                };
            });

            //Versionamento por QueryString e Header
            //services.AddApiVersioning(options => {
            //    options.ApiVersionReader = ApiVersionReader.Combine(
            //            new QueryStringApiVersionReader("api-version"), //Versionamento por querystring
            //            new HeaderApiVersionReader("api-version")//Versionamento pelo header
            //        );
            //});
            services.AddApiVersioning();
            /*
             Desabilita a validação automatica do ModelState das requisições,
             incluida pela notação [ApiController], retorna um 400 (Bad Request) sem precisar entrar na action,
              isso é feito com um filtro de action.
            */
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            //Configurações do Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Title = "Livros API",
                    Description = "Documentação API",
                    Version = "1.0"
                });
                options.SwaggerDoc("v2", new Info
                {
                    Title = "Livros API",
                    Description = "Documentação API",
                    Version = "2.0"
                });

                options.EnableAnnotations();

                //definição do esquema de segurança utilizado
                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey",
                    Description = "Autenticação Bearer via JWT"
                });

                //que operações usam o esquema acima - todas
                options.AddSecurityRequirement(
                    new Dictionary<string, IEnumerable<string>> {
                    { "Bearer", new string[] { } }
                });

                //descrevendo enumerados como strings
                options.DescribeAllEnumsAsStrings();
                options.DescribeStringEnumsInCamelCase();

                //adicionando o filtro para incluir respostas 401 nas operações
                options.OperationFilter<AuthResponsesOperationFilter>();

                //adicionando o filtro para incluir descrições nas tags
                options.DocumentFilter<TagDescriptionsDocumentFilter>();
            });



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "Versão 1.0");
                x.SwaggerEndpoint("/swagger/v2/swagger.json", "Versão 2.0");
            });

        }
    }
}
