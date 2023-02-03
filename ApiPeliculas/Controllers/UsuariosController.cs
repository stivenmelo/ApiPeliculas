using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTO;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;

namespace ApiPeliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IMapper _mapper;
        protected RespuestaAPI _respuestaAPI;

        public UsuariosController(IUsuarioRepositorio usuarioRepositorio, IMapper mapper)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _mapper = mapper;
            this._respuestaAPI = new RespuestaAPI();
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetUsuarios()
        {
            var listaUsuarios = _usuarioRepositorio.GetUsuarios();

            List<UsuarioDTO> listaUsuariosDTO = new List<UsuarioDTO>();

            foreach (var usuario in listaUsuarios)
            {
                listaUsuariosDTO.Add(_mapper.Map<UsuarioDTO>(usuario));
            }

            return Ok(listaUsuariosDTO);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{usuarioId:int}", Name = "GetUsuario")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetUsuario(int usuarioId)
        {
            var itemUsuario = _usuarioRepositorio.GetUsuario(usuarioId);

            if (itemUsuario == null)
            {
                return NotFound();
            }

            UsuarioDTO itemUsuarioDTO = _mapper.Map<UsuarioDTO>(itemUsuario);

            return Ok(itemUsuarioDTO);
        }

        [AllowAnonymous]
        [HttpPost("registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async  Task<IActionResult> Registro([FromBody] UsuarioRegistroDTO usuarioRegistroDTO)
        {
            bool validarNombreUsuarioEsUnico = _usuarioRepositorio.EsUnicoUsuario(usuarioRegistroDTO.NombreUsuario);

            if (!validarNombreUsuarioEsUnico)
            {
                _respuestaAPI.StatusCode = HttpStatusCode.BadRequest;
                _respuestaAPI.IsSuccess = false;
                _respuestaAPI.ErrorMessages.Add("el nombre del usuario ya existe");
                return BadRequest(_respuestaAPI);
            }

            var usuario = await _usuarioRepositorio.Registro(usuarioRegistroDTO);
            if (usuario == null)
            {
                _respuestaAPI.StatusCode = HttpStatusCode.BadRequest;
                _respuestaAPI.IsSuccess = false;
                _respuestaAPI.ErrorMessages.Add("Error en el registro");
                return BadRequest(_respuestaAPI);
            }

            _respuestaAPI.StatusCode = HttpStatusCode.OK;
            _respuestaAPI.IsSuccess = true;
            return Ok(_respuestaAPI);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDTO usuarioLoginDTO)
        {
            var respuestaLogin = await _usuarioRepositorio.Login(usuarioLoginDTO);

            if (respuestaLogin.Usuario == null || string.IsNullOrEmpty(respuestaLogin.Token))
            {
                _respuestaAPI.StatusCode = HttpStatusCode.BadRequest;
                _respuestaAPI.IsSuccess = false;
                _respuestaAPI.ErrorMessages.Add("el nombre de usuario o contraseña son incorrectos");
                return BadRequest(_respuestaAPI);
            }

            _respuestaAPI.StatusCode = HttpStatusCode.OK;
            _respuestaAPI.IsSuccess = true;
            _respuestaAPI.Result = respuestaLogin;
            return Ok(_respuestaAPI);
        }
    }
}
