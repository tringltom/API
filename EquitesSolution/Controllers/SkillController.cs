using System.Threading.Tasks;
using API.Validations;
using Application.Models;
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

        [HttpGet("user/{id}")]
        [IdValidation]
        public async Task<IActionResult> GetSkillsData(int id)
        {
            var result = await _skillService.GetSkillsDataAsync(id);
            return result.Match(
                skillData => Ok(skillData),
                err => err.Response());
        }

        [HttpPut("user/me")]
        public async Task<IActionResult> UpdateSkillsData(SkillData skillData)
        {
            var result = await _skillService.UpdateSkillsDataAsync(skillData);
            return result.Match(
                user => Ok(user),
                err => err.Response());
        }
    }
}
