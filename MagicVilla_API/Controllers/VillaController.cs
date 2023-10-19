﻿using MagicVilla_API.Controllers.Datos;
using MagicVilla_API.Controllers.Models;
using MagicVilla_API.Controllers.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public ActionResult<IEnumerable<VillaDTO>> GetVillas ()
        {
            return Ok(VillaStore.villaList);
        }

        [HttpGet("id:int", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<VillaDTO> GetVilla (int id)
        {
            if (id == 0) 
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

            if (villa == null) 
            { 
                return NotFound();
            }

            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<VillaDTO> CrearVilla([FromBody]VillaDTO villaDTO)
        {
            if(!ModelState.IsValid) 
            { 
                return BadRequest(ModelState);
            }

            if(VillaStore.villaList.FirstOrDefault(v=>v.Nombre.ToLower() == villaDTO.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("NombreExiste", "La villa con ese Nombre ya existe");
                return BadRequest(ModelState);
            }

            if(villaDTO == null) 
            {
                return BadRequest(villaDTO);
            }
            if(villaDTO.Id>0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            villaDTO.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villaDTO);

            return CreatedAtRoute("GetVilla",new {id=villaDTO.Id}, villaDTO);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        public IActionResult DeleteVilla(int id) 
        {
            if (id == 0) 
            {
                return BadRequest();    
            }
            var villa = VillaStore.villaList.FirstOrDefault(v=>v.Id == id);

            if (villa == null) 
            { 
                return NotFound();
            }
            VillaStore.villaList.Remove(villa);

            return NoContent();
        }
    }
}
