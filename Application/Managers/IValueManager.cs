using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Managers
{
    public interface IValueManager
    {
        Task<List<Value>> GetAllWithIdAbove(int id);
    }
}
