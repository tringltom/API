using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Application.Errors;
using Application.Models.Activity;
using DAL.Query;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public interface IHappeningService
    {
        Task<HappeningEnvelope> GetHappeningsForApprovalAsync(QueryObject queryObject);
        Task<Either<RestError, Unit>> AttendToHappeningAsync(int id, bool attend);
        Task<Either<RestError, Unit>> ConfirmAttendenceToHappeningAsync(int id);
        Task<Either<RestError, Unit>> CompleteHappeningAsync(int id, HappeningUpdate happeningUpdate);
        Task<Either<RestError, Unit>> ApproveHappeningCompletitionAsync(int id, bool approve);
    }
}
