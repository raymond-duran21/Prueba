using MagicVilla_API.Controllers.Datos;
using MagicVilla_API.Controllers.Models;
using MagicVilla_API.Repository.IRepository;

namespace MagicVilla_API.Repository
{
    public class VillaRepository : Repository<Villa>, IVillaRepository
    {
        private readonly ApplicationDbContext _db;

        public VillaRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;  
        }

        public async Task<Villa> Actualizar(Villa entity)
        {
            entity.FechaActualizacion = DateTime.Now;
            _db.Villas.Update(entity);
           await _db.SaveChangesAsync();
            return entity;
        }
    }
}
