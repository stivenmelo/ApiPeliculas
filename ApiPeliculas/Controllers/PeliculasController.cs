using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTO;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {
        private readonly IPeliculaRepositorio _peliculaRepositorio;
        private readonly IMapper _mapper;

        public PeliculasController(IPeliculaRepositorio peliculaRepositorio, IMapper mapper)
        {
            _peliculaRepositorio = peliculaRepositorio;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPelicula()
        {
            var listaPeliculas = _peliculaRepositorio.GetPeliculas();

            List<PeliculaDTO> listaPeliculasDTO = new List<PeliculaDTO>();

            foreach (var Pelicula in listaPeliculas)
            {
                listaPeliculasDTO.Add(_mapper.Map<PeliculaDTO>(Pelicula));
            }

            return Ok(listaPeliculasDTO);
        }

        [AllowAnonymous]
        [HttpGet("{peliculaId:int}", Name = "GetPelicula")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPelicula(int peliculaId)
        {
            var itemPelicula = _peliculaRepositorio.GetPelicula(peliculaId);

            if (itemPelicula == null)
            {
                return NotFound();
            }

            PeliculaDTO itemPeliculaDTO = _mapper.Map<PeliculaDTO>(itemPelicula);

            return Ok(itemPeliculaDTO);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(PeliculaDTO))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult CrearPelicula([FromBody] PeliculaDTO nuevaPeliculaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (nuevaPeliculaDTO == null)
            {
                return BadRequest(ModelState);
            }

            if (_peliculaRepositorio.ExistePelicula(nuevaPeliculaDTO.Nombre))
            {
                ModelState.AddModelError("", "La Pelicula ya existe");
                return StatusCode(400, ModelState);
            }

            var PeliculaMap = _mapper.Map<Pelicula>(nuevaPeliculaDTO);

            if (!_peliculaRepositorio.CrearPelicula(PeliculaMap))
            {
                ModelState.AddModelError("", $"Algo salió mal guardando el registro {PeliculaMap.Nombre}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetPelicula", new { PeliculaId = PeliculaMap.Id }, PeliculaMap);
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("{peliculaId:int}", Name = "ActualizarPatchPelicula")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ActualizarPatchPelicula(int peliculaId, [FromBody] PeliculaDTO peliculaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (peliculaDTO == null || peliculaId != peliculaDTO.Id)
            {
                return BadRequest(ModelState);
            }

            var PeliculaMap = _mapper.Map<Pelicula>(peliculaDTO);

            if (!_peliculaRepositorio.ActualizarPelicula(PeliculaMap))
            {
                ModelState.AddModelError("", $"Algo salió mal actualizando el registro {PeliculaMap.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{peliculaId:int}", Name = "BorrarPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult BorrarPelicula(int peliculaId)
        {
            if (!_peliculaRepositorio.ExistePelicula(peliculaId))
            {
                return NotFound();
            }

            var pelicula = _peliculaRepositorio.GetPelicula(peliculaId);

            if (!_peliculaRepositorio.BorrarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salió mal borrando el registro{pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("GetPeliculasPorCategoria/{categoriaId:int}")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPeliculasPorCategoria(int categoriaId)
        {
            var listaPeliculas = _peliculaRepositorio.GetPeliculasPorCategoria(categoriaId);

            if (listaPeliculas == null)
            {
                return NotFound();
            }

            var itemPelicula = new List<PeliculaDTO>();

            foreach (var Pelicula in listaPeliculas)
            {
                itemPelicula.Add(_mapper.Map<PeliculaDTO>(Pelicula));
            }

            return Ok(itemPelicula);
        }


        [AllowAnonymous]
        [HttpGet("GetPeliculasPorNombre")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPeliculasPorNombre(string nombre)
        {


            try
            {
                var resultado = _peliculaRepositorio.GetPeliculasPorNombre(nombre.Trim());
                if (resultado.Any())
                {
                    return Ok(resultado);
                }
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erorr recuperando datos");
            }
        }
    }
}
