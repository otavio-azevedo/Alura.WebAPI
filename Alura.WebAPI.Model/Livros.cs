using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Alura.ListaLeitura.Modelos
{
    public class Livro
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Resumo { get; set; }
        public byte[] ImagemCapa { get; set; }
        public string Autor { get; set; }
        public TipoListaLeitura Lista { get; set; }
    }

    //Possibilita alteração do nome do recurso XML evitando expor o nome da classe
    //se o cliente optar por retornar um XML
    [XmlType("Livro")]
    public class LivroApi
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Autor { get; set; }
        public string Resumo { get; set; }
        public string ImagemCapa { get; set; }
        public string Lista { get; set; }
    }

    public class LivroUpload
    {
        public int Id { get; set; }
        [Required]
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Autor { get; set; }
        public string Resumo { get; set; }
        public IFormFile Capa { get; set; }
        public TipoListaLeitura Lista { get; set; }
    }
}
