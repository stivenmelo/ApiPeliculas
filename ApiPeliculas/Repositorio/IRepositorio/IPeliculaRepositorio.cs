using ApiPeliculas.Modelos;

namespace ApiPeliculas.Repositorio.IRepositorio
{
    public interface IPeliculaRepositorio
    {
        ICollection<Pelicula> GetPeliculas();
        Pelicula GetPelicula(int peliculaId);
        bool ExistePelicula(string nombre);
        bool ExistePelicula(int peliculaId);
        bool CrearPelicula(Pelicula pelicula);
        bool ActualizarPelicula(Pelicula pelicula);
        bool BorrarPelicula(Pelicula pelicula);

        //Metodos para buscar Peliculas pot categoria y nombre
        ICollection<Pelicula> GetPeliculasPorCategoria(int CategoriaId);
        ICollection<Pelicula> GetPeliculasPorNombre(string Nombre);


        bool Guardar();
    }
}
