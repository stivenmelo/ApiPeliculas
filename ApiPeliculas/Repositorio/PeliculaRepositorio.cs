using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApiPeliculas.Repositorio
{
    public class PeliculaRepositorio : IPeliculaRepositorio
    {
        private readonly ApplicationDbContext _bd;

        public PeliculaRepositorio(ApplicationDbContext bd)
        {
            _bd = bd;
        }
        public bool ActualizarPelicula(Pelicula pelicula)
        {
            pelicula.FechaCreacion = DateTime.Now;
            _bd.Pelicula.Update(pelicula);
            return Guardar();
        }

        public bool BorrarPelicula(Pelicula pelicula)
        {
            _bd.Pelicula.Remove(pelicula);
            return Guardar();
        }

        public bool CrearPelicula(Pelicula pelicula)
        {
            pelicula.FechaCreacion = DateTime.Now;
            _bd.Pelicula.Add(pelicula);
            return Guardar();
        }

        public bool ExistePelicula(string nombre)
        {
            bool existe = _bd.Pelicula.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            return existe;
        }

        public bool ExistePelicula(int peliculaId)
        {
            bool existe = _bd.Pelicula.Any(c => c.Id == peliculaId);
            return existe;
        }

        public Pelicula GetPelicula(int peliculaId)
        {
            return _bd.Pelicula.FirstOrDefault(c => c.Id == peliculaId);
        }

        public ICollection<Pelicula> GetPeliculas()
        {
            return _bd.Pelicula.OrderBy(c => c.Nombre).ToList();
        }

        public ICollection<Pelicula> GetPeliculasPorCategoria(int categoriaId)
        {
            return _bd.Pelicula.Include(ca => ca.Categoria).Where(ca => ca.categoriaId == categoriaId).ToList();
        }

        public ICollection<Pelicula> GetPeliculasPorNombre(string nombre)
        {
            if (!string.IsNullOrEmpty(nombre))
            {
                return _bd.Pelicula.Where(x => x.Nombre.Contains(nombre) || x.Descripcion.Contains(nombre)).ToList();
            }

            return null;
        }

        public bool Guardar()
        {
            return _bd.SaveChanges() >= 0 ? true : false;
        }
    }
}
