using System.Threading.Tasks;
using Application.Models;
using Application.Models.User;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("skills")]
    public class SkillController : BaseController
    {
        private readonly ISkillService _skillService;

        public SkillController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        [HttpGet("{userId}")]
        public async Task<SkillData> GetSkillsDataAsync(int userId)
        {
            return await _skillService.GetSkillsDataAsync(userId);
        }

        [HttpPut("users/me")]
        public async Task<ActionResult<UserBaseResponse>> UpdateSkillsDataAsync(SkillData skillData)
        {
            return await _skillService.ResetSkillsDataAsync();
        }

        //[HttpPut("reset")]
        //public async Task<ActionResult<UserBaseResponse>> ResetSkillsDataAsync()
        //{
        //    return await _skillService.ResetSkillsDataAsync();
        //}

        //[HttpPut("update")]
        //public async Task<ActionResult<UserBaseResponse>> UpdateSkillsDataAsync(SkillData skillData)
        //{
        //    return await _skillService.UpdateSkillsDataAsync(skillData);
        //}
    }
}
