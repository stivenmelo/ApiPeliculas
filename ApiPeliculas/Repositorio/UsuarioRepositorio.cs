using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTO;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XSystem.Security.Cryptography;

namespace ApiPeliculas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _bd;
        private string claveSecreta;

        public UsuarioRepositorio(ApplicationDbContext bd,IConfiguration config)
        {
            _bd = bd;
            claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
        }

        public Usuario GetUsuario(int usuarioId)
        {
            return _bd.Usuario.FirstOrDefault(u => u.Id == usuarioId);
        }

        public ICollection<Usuario> GetUsuarios()
        {
            return _bd.Usuario.OrderBy(u => u.Nombre).ToList();
        }

        public bool EsUnicoUsuario(string nombre)
        {
            Usuario usuarioBd = _bd.Usuario.FirstOrDefault(u => u.NombreUsuario == nombre);
            if (usuarioBd == null)
            {
                return true;
            }
            return false;
        }

        public async Task<Usuario> Registro(UsuarioRegistroDTO usuarioRegistroDTO)
        {
            var passwordEncriptado = obtenetMD5(usuarioRegistroDTO.Password);

            Usuario usuario = new Usuario()
            {
                NombreUsuario = usuarioRegistroDTO.NombreUsuario,
                Password = passwordEncriptado,
                Nombre = usuarioRegistroDTO.Nombre,
                Role = usuarioRegistroDTO.Role,
            };

            _bd.Usuario.Add(usuario);
            await _bd.SaveChangesAsync();
            usuario.Password = passwordEncriptado;
            return usuario;
        }

        public async Task<UsuarioLoginRespuestaDTO> Login(UsuarioLoginDTO usuarioLoginDTO)
        {
            var passwordEncriptado = obtenetMD5(usuarioLoginDTO.Password);

            Usuario usuario = _bd.Usuario.FirstOrDefault(
                u => u.NombreUsuario.ToLower() == usuarioLoginDTO.NombreUsuario.ToLower()
                && u.Password == passwordEncriptado
                );

            //validamos si el usuario no existe con la combinación de usuario y contraseña correcta
            if (usuario == null)
            {
                return new UsuarioLoginRespuestaDTO() 
                {
                    Token = "",
                    Usuario = null
                };
            }

            // Aquí existe el usuario entonces podemos procesar el login

            var manejadorToken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(claveSecreta);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario.ToString()),
                    new Claim(ClaimTypes.Role, usuario.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = manejadorToken.CreateToken(tokenDescriptor);

            UsuarioLoginRespuestaDTO usuarioLoginRespuestaDTO = new UsuarioLoginRespuestaDTO()
            {
                Token = manejadorToken.WriteToken(token),
                Usuario = usuario
            };

            return usuarioLoginRespuestaDTO;

        }

        //Encriptacion
        public static string obtenetMD5(string valor)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);
            data= x.ComputeHash(data);
            string resp = "";
            for (int i = 0; i < data.Length; i++)
                resp+= data[i].ToString("x2").ToLower();
            return resp;
        }
    }
}
