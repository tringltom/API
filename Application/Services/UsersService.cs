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

namespace Application.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IUserAccessor _userAccessor;
        private readonly IPhotoAccessor _photoAccessor;

        public UsersService(IUnitOfWork uow, IMapper mapper, IUserAccessor userAccessor, IPhotoAccessor photoAccessor)
        {
            _uow = uow;
            _mapper = mapper;
            _userAccessor = userAccessor;
            _photoAccessor = photoAccessor;
        }

        public async Task<UserRangingEnvelope> GetRangingUsers(int? limit, int? offset)
        {
            var topXpUsers = await _uow.Users.GetRangingUsers(limit, offset);

            var userRangingEnvelope = new UserRangingEnvelope
            {
                Users = _mapper.Map<IEnumerable<User>, IEnumerable<UserRangingGet>>(topXpUsers).ToList(),
                UserCount = await _uow.Users.CountAsync(),
            };

            return userRangingEnvelope;
        }

        public async Task UpdateLoggedUserAboutAsync(UserAbout userAbout)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            var user = await _uow.Users.GetAsync(userId);

            if (user == null)
                throw new NotFound("Nepostojeci korisnik");

            user.About = userAbout.About;

            await _uow.CompleteAsync();
        }

        public async Task UpdateLoggedUserImageAsync(UserImageUpdate userImage)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            var user = await _uow.Users.GetAsync(userId);

            if (user == null)
                throw new NotFound("Nepostojeci korisnik");

            var image = userImage.Images.SingleOrDefault();

            var photoResult = image != null ? await _photoAccessor.AddPhotoAsync(image) : null;

            if (photoResult == null)
                throw new NotFound("Neuspešna promena profilne slike, molimo vas pokušajte kasnije");

            if (!string.IsNullOrEmpty(user.ImagePublicId))
                await _photoAccessor.DeletePhotoAsync(user.ImagePublicId);

            user.ImagePublicId = photoResult.PublicId;
            user.ImageUrl = photoResult.Url;
            user.ImageApproved = false;

            await _uow.CompleteAsync();
        }

        public async Task<UserImageEnvelope> GetImagesForApprovalAsync(int? limit, int? offset)
        {
            var usersForImageApproval = await _uow.Users.GetUsersForImageApproval(limit, offset);

            var userImageEnvelope = new UserImageEnvelope
            {
                Users = _mapper.Map<IEnumerable<User>, IEnumerable<UserImageResponse>>(usersForImageApproval).ToList(),
                UserCount = await _uow.Users.CountUsersForImageApproval(),
            };

            return userImageEnvelope;
        }

        public async Task<bool> ResolveUserImageAsync(int userId, bool approve)
        {
            var user = await _uow.Users.GetAsync(userId);

            if (user == null)
                throw new NotFound("Nepostojeci korisnik");

            user.ImageApproved = approve;

            if (!approve)
            {
                await _photoAccessor.DeletePhotoAsync(user.ImagePublicId);
                user.ImagePublicId = null;
                user.ImageUrl = null;
            }

            return await _uow.CompleteAsync();
        }
    }
}
