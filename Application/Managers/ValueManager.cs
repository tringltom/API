using System.Collections.Generic;
using Persistence;
using System.Linq;
using Domain.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.Repositories;

namespace Application.Managers
{
    public class ValueManager : IValueManager
    {

        private readonly IValueRepository _valueRepository;

        public ValueManager(IValueRepository valueRepository)
        {
            _valueRepository = valueRepository;
        }

        public async Task<List<Value>> GetAllWithIdAbove(int id)
        {
            // this is only for boilerplate code
            // this example is not ideal as it does not contain any real business logic (should be in repository as is)
            var allValues = await _valueRepository.GetAllValues();
            return allValues.Where(v => v.Id > id).ToList();
        }
    }
}
