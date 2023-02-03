using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTO;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ApiPeliculas.Controllers
{

    [Route("api/[Controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepositorio _categoriaRepositorio;
        private readonly IMapper _mapper;

        public CategoriasController(ICategoriaRepositorio categoriaRepositorio, IMapper mapper)
        {
            _categoriaRepositorio = categoriaRepositorio;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        //[ResponseCache(Duration = 20)]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategorias()
        {
            var listaCategotias = _categoriaRepositorio.GetCategorias();

            List<CategoriaDTO> listaCategotiasDTO = new List<CategoriaDTO>();

            foreach (var categoria in listaCategotias)
            {
                listaCategotiasDTO.Add(_mapper.Map<CategoriaDTO>(categoria));
            }

            return Ok(listaCategotiasDTO);
        }

        [AllowAnonymous]
        [HttpGet("{categoriaId:int}", Name = "GetCategoria")]
        //[ResponseCache(Duration = 30)]
        [ResponseCache(CacheProfileName = "PorDefecto20Seg")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCategoria(int categoriaId)
        {
            var itemCategoria = _categoriaRepositorio.GetCategoria(categoriaId);

            if (itemCategoria == null)
            {
                return NotFound();
            }

            CategoriaDTO itemCategoriaDTO = _mapper.Map<CategoriaDTO>(itemCategoria);

            return Ok(itemCategoriaDTO);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CategoriaDTO))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public IActionResult CrearCategoria([FromBody] CrearCategoriaDTO nuevaCategoriaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (nuevaCategoriaDTO == null)
            {
                return BadRequest(ModelState);
            }

            if (_categoriaRepositorio.ExisteCategoria(nuevaCategoriaDTO.Nombre))
            {
                ModelState.AddModelError("", "La categoria ya existe");
                return StatusCode(400, ModelState);
            }

            var categoriaMap = _mapper.Map<Categoria>(nuevaCategoriaDTO);

            if (!_categoriaRepositorio.CrearCategoria(categoriaMap))
            {
                ModelState.AddModelError("", $"Algo salió mal guardando el registro{categoriaMap.Nombre}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategoria", new { categoriaId = categoriaMap.Id }, categoriaMap);
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("{categoriaId:int}", Name = "ActualizarPatchCategoria")]
        [ProducesResponseType(201, Type = typeof(CategoriaDTO))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ActualizarPatchCategoria(int categoriaId, [FromBody] CategoriaDTO CategoriaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (CategoriaDTO == null || categoriaId != CategoriaDTO.Id)
            {
                return BadRequest(ModelState);
            }

            var categoriaMap = _mapper.Map<Categoria>(CategoriaDTO);

            if (!_categoriaRepositorio.ActualizarCategoria(categoriaMap))
            {
                ModelState.AddModelError("", $"Algo salió mal actualizando el registro{categoriaMap.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{categoriaId:int}", Name = "BorrarCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult BorrarCategoria(int categoriaId)
        {
            if (!_categoriaRepositorio.ExisteCategoria(categoriaId))
            {
                return NotFound();
            }

            var categoria = _categoriaRepositorio.GetCategoria(categoriaId);

            if (!_categoriaRepositorio.BorrarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salió mal borrando el registro{categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }




    }
}
