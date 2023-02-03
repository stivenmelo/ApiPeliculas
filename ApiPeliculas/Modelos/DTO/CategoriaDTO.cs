using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.DTO
{
    public class CategoriaDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El Nombre es obligatorio")]
        [MaxLength(100,ErrorMessage ="El numero maximo de caracteres es de 100!")]
        public string Nombre { get; set; }
    }
}
