using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.Models.Comment;
using Application.ServiceInterfaces;
using AutoMapper;
using DAL;
using Domain;
using LanguageExt;

namespace Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IUserAccessor _userAccessor;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ChatService(IUserAccessor userAccessor, IUnitOfWork uow, IMapper mapper)
        {
            _userAccessor = userAccessor;
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Either<RestError, CommentReturn>> ApplyComment(CommentCreate commentCreate)
        {
            var activity = await _uow.Activities.GetAsync(commentCreate.ActivityId);

            if (activity == null)
                return new NotFound("Aktivnost nije pronađena");

            var userId = _userAccessor.GetUserIdFromAccessToken();
            var user = await _uow.Users.GetAsync(userId);

            var comment = new Comment
            {
                User = user,
                Activity = activity,
                Body = commentCreate.Body,
                CreatedAt = DateTimeOffset.Now,
            };

            activity.Comments.Add(comment);

            await _uow.CompleteAsync();

            return _mapper.Map<CommentReturn>(comment);
        }
    }
}
