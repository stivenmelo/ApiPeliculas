using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTO;
using AutoMapper;

namespace ApiPeliculas.PeliculasMapper
{
    public class PeliculasMapper : Profile
    {
        public PeliculasMapper()
        {
            CreateMap<Categoria, CategoriaDTO>().ReverseMap();
            CreateMap<Categoria, CrearCategoriaDTO>().ReverseMap();
            CreateMap<Pelicula, PeliculaDTO>().ReverseMap();
            CreateMap<Usuario, UsuarioDTO>().ReverseMap();
            CreateMap<Usuario, UsuarioRegistroDTO>().ReverseMap();
            CreateMap<Usuario, UsuarioLoginDTO>().ReverseMap();
        }
    }
}
