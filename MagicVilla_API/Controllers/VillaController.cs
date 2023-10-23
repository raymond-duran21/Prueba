using AutoMapper;
using MagicVilla_API.Controllers.Datos;
using MagicVilla_API.Controllers.Models;
using MagicVilla_API.Controllers.Models.DTO;
using MagicVilla_API.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;
        private readonly IVillaRepository _villaRepo;
        private readonly IMapper _mapper;
        protected APIResponse _apiResponse;

        public VillaController(ILogger<VillaController> logger, IVillaRepository villaRepo, IMapper mapper) 
        {
            _logger = logger;
            _villaRepo = villaRepo;
            _mapper = mapper;
            _apiResponse = new ();
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<APIResponse>> GetVillas ()
        {
            try 
            {
                _logger.LogInformation("Obtener las Villas");

                IEnumerable<Villa> villaList = await _villaRepo.ObtenerTodos();
                _apiResponse.Result = _mapper.Map<IEnumerable<VillaDTO>>(villaList);
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;

                return Ok(_apiResponse);
            }
            catch (Exception ex) 
            { 
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _apiResponse;
        }

        [HttpGet("id:int", Name ="GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<APIResponse>> GetVilla (int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Error al traer Villa con Id" + id);
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    return BadRequest(_apiResponse);
                }
                //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
                var villa = await _villaRepo.Obtener(v => v.Id == id);

                if (villa == null)
                {
                    _apiResponse.StatusCode =HttpStatusCode.NotFound;
                    _apiResponse.IsSuccess = false;
                    return NotFound(_apiResponse);
                }

                _apiResponse.Result = _mapper.Map<VillaDTO>(villa);
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _apiResponse;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<APIResponse>> CrearVilla([FromBody] VillaCreateDTO createDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _villaRepo.Obtener(v => v.Nombre.ToLower() == createDTO.Nombre.ToLower()) != null)
                {
                    ModelState.AddModelError("NombreExiste", "La villa con ese Nombre ya existe");
                    return BadRequest(ModelState);
                }

                if (createDTO == null)
                {
                    return BadRequest(createDTO);
                }

                Villa modelo = _mapper.Map<Villa>(createDTO);

                modelo.FechaCreacion = DateTime.Now;
                modelo.FechaActualizacion = DateTime.Now;
                await _villaRepo.Crear(modelo);
                _apiResponse.Result = modelo;
                _apiResponse.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetVilla", new { id = modelo.Id }, _apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _apiResponse;
            
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        public async Task<IActionResult> DeleteVilla(int id) 
        {
            try
            {
                if (id == 0)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;

                    return BadRequest(_apiResponse);
                }
                var villa = await _villaRepo.Obtener(v => v.Id == id);

                if (villa == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;

                    return NotFound(_apiResponse);
                }
                await _villaRepo.Remove(villa);

                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                return Ok(_apiResponse);
            }
            catch (Exception Ex)
            {

                _apiResponse.IsSuccess=false;
                _apiResponse.ErrorMessages = new List<string>() { Ex.ToString() };
            }
            return BadRequest(_apiResponse);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id,[FromBody] VillaUpdateDTO updateDTO)
        {
            if (updateDTO == null || id!= updateDTO.Id) 
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            
            }
            
            Villa modelo = _mapper.Map<Villa>(updateDTO);

            await _villaRepo.Actualizar(modelo);
            _apiResponse.StatusCode =HttpStatusCode.NoContent;

            return Ok(_apiResponse);

        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateParcialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                

            }
            var villa = await _villaRepo.Obtener(v => v.Id == id, tracked:false);

            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

            /*VillaUpdateDTO villaDTO = new()
            {
                Id = villa.Id,
                Nombre = villa.Nombre,
                Detalle = villa.Detalle,
                ImagenUrl = villa.ImagenUrl,
                Ocupantes = villa.Ocupantes,
                Tarifa = villa.Tarifa,
                MetrosCuadrados = villa.MetrosCuadrados,
                Amenidad = villa.Amenidad
            };*/

            if(villa == null) return BadRequest();

            patchDTO.ApplyTo(villaDTO, ModelState);

            if(!ModelState.IsValid)
            { 
                return BadRequest(ModelState); 
            }

            Villa modelo = _mapper.Map<Villa>(villaDTO);

            /*Villa modelo = new()
            {
                Id = villaDTO.Id,
                Nombre = villaDTO.Nombre,
                Detalle = villaDTO.Detalle,
                ImagenUrl = villaDTO.ImagenUrl,
                Ocupantes = villaDTO.Ocupantes,
                Tarifa = villaDTO.Tarifa,
                MetrosCuadrados = villaDTO.MetrosCuadrados,
                Amenidad = villaDTO.Amenidad
            };*/

            await _villaRepo.Actualizar(modelo);
            _apiResponse.StatusCode =HttpStatusCode.NoContent;

            return Ok(_apiResponse);
        }

    }
}
