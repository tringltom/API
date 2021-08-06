using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Repositories
{
    public class ValueRepository : IValueRepository
    {
        private readonly DataContext _context;

        public ValueRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Value>> GetAllValues()
        {
            return await _context.Values.ToListAsync();
        }
    }
}
