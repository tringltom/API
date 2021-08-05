
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Repositories
{
    public interface IValueRepository
    {
        Task<List<Value>> GetAllValues();
    }
}
