using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Managers
{
    public interface IValueManager
    {
        Task<List<Value>> GetAllWithIdAbove(int id);
    }
}
