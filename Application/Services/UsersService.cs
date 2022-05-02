using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.InfrastructureInterfaces.Security;
using Application.Models.User;
using Application.ServiceInterfaces;
using AutoMapper;
using DAL;
using Domain;
using LanguageExt;

namespace Application.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IUserAccessor _userAccessor;
        private readonly IPhotoAccessor _photoAccessor;
        private readonly IEmailManager _emailManager;

        public UsersService(IUnitOfWork uow, IMapper mapper, IUserAccessor userAccessor, IPhotoAccessor photoAccessor, IEmailManager emailManager)
        {
            _uow = uow;
            _mapper = mapper;
            _userAccessor = userAccessor;
            _photoAccessor = photoAccessor;
            _emailManager = emailManager;
        }

        public async Task<UserRankedEnvelope> GetRankedUsersAsync(int? limit, int? offset)
        {
            var rankedUsers = await _uow.Users.GetRankedUsersAsync(limit, offset);

            var userRangingEnvelope = new UserRankedEnvelope
            {
                Users = _mapper.Map<IEnumerable<User>, IEnumerable<UserRankedGet>>(rankedUsers).ToList(),
                UserCount = await _uow.Users.CountAsync(),
            };

            return userRangingEnvelope;
        }

        public async Task<UserImageEnvelope> GetImagesForApprovalAsync(int? limit, int? offset)
        {
            var usersForImageApproval = await _uow.Users.GetUsersForImageApprovalAsync(limit, offset);

            var userImageEnvelope = new UserImageEnvelope
            {
                Users = _mapper.Map<IEnumerable<User>, IEnumerable<UserImageResponse>>(usersForImageApproval).ToList(),
                UserCount = await _uow.Users.CountUsersForImageApprovalAsync(),
            };

            return userImageEnvelope;
        }

        public async Task<Unit> UpdateAboutAsync(UserAbout userAbout)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            var user = await _uow.Users.GetAsync(userId);

            user.About = userAbout.About;
            await _uow.CompleteAsync();
            return Unit.Default;
        }

        public async Task<Either<RestError, Unit>> UpdateImageAsync(UserImageUpdate userImage)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            var user = await _uow.Users.GetAsync(userId);

            var photoResult = userImage.Image != null ? await _photoAccessor.AddPhotoAsync(userImage.Image) : null;

            if (!string.IsNullOrEmpty(user.ImagePublicId))
                await _photoAccessor.DeletePhotoAsync(user.ImagePublicId);

            user.ImagePublicId = photoResult.PublicId;
            user.ImageUrl = photoResult.Url;
            user.ImageApproved = false;

            await _uow.CompleteAsync();
            return Unit.Default;
        }

        public async Task<Either<RestError, Unit>> ResolveImageAsync(int userId, bool approve)
        {
            var user = await _uow.Users.GetAsync(userId);

            if (user == null)
                return new NotFound("Nepostojeci korisnik");

            user.ImageApproved = approve;

            if (!approve)
            {
                await _photoAccessor.DeletePhotoAsync(user.ImagePublicId);
                user.ImagePublicId = null;
                user.ImageUrl = null;
            }

            await _uow.CompleteAsync();
            await _emailManager.SendProfileImageApprovalEmailAsync(user.Email, approve);

            return Unit.Default;
        }
    }
}
