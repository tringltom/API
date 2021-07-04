using System.Collections.Generic;
using Persistence;
using System.Linq;
using Domain.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Managers
{
    public class ValueManager : IValueManager
    {
        private readonly DataContext _context;

        public ValueManager(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Value>> GetAllWithIdAbove(int id)
        {
            // this is only for boilerplate code
            // this example is not ideal as it does not contain any real business logic (should be in repository as is)
            return await _context.Values.Where(v => v.Id > id).ToListAsync();
        }
    }
}
