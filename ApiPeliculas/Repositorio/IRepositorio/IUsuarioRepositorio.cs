using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTO;

namespace ApiPeliculas.Repositorio.IRepositorio
{
    public interface IUsuarioRepositorio
    {
        ICollection<Usuario> GetUsuarios();
        Usuario GetUsuario(int usuarioId);
        bool EsUnicoUsuario(string nombre);
        Task<UsuarioLoginRespuestaDTO> Login(UsuarioLoginDTO usuarioLoginDTO);
        Task<Usuario> Registro(UsuarioRegistroDTO usuarioRegistroDTO);
        
    }
}
