using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models.Activity;
using Domain;

namespace Application.ManagerInterfaces
{
    public interface IActivityCounterManager
    {
        Task<List<ActivityCount>> GetActivityCountsAsync(User user);
    }
}
