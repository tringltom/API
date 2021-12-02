using Domain.Entities;

namespace Application.Repositories;

public interface IActivityRepository
{
    Task CreateActivityAsync(PendingActivity activity);
}
