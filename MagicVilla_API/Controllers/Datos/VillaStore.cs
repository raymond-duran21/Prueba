using MagicVilla_API.Controllers.Models.DTO;

namespace MagicVilla_API.Controllers.Datos
{
    public static class VillaStore
    {
        public static List<VillaDTO> villaList = new List<VillaDTO>
        {
            new VillaDTO { Id = 1, Nombre="Vista de la piscina", Ocupantes=3, MetrosCuadrados=50},
            new VillaDTO { Id = 2, Nombre="Villa de la playa", Ocupantes=4, MetrosCuadrados=80}
        };

    }
}
