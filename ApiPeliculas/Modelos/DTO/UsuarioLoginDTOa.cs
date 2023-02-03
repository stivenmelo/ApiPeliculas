using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.DTO
{
    public class UsuarioLoginDTO
    {
        [Required(ErrorMessage ="El usuario es obligatorio")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El password es obligatorio")]
        public string Password { get; set; }
    }
}
