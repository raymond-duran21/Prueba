using MagicVilla_API.Controllers.Models;

namespace MagicVilla_API.Repository.IRepository
{
    public interface IVillaRepository : IRepository<Villa>
    {
        Task<Villa> Actualizar(Villa entity);
    }
}
