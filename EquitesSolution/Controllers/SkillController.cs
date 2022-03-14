using System.Threading.Tasks;
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

        [HttpGet("{userId}")]
        public async Task<SkillData> GetSkillsDataAsync(int userId)
        {
            return await _skillService.GetSkillsDataAsync(userId);
        }

        [HttpPut("reset")]
        public async Task<ActionResult> ResetSkillsDataAsync()
        {
            await _skillService.ResetSkillsDataAsync();

            return Ok("Uspešno ste poništili vaše odabrane poene");
        }

        [HttpPut("update")]
        public async Task<ActionResult> UpdateSkillsDataAsync(SkillData skillData)
        {
            await _skillService.UpdateSkillsDataAsync(skillData);

            return Ok("Uspešno ste izabrali dodatne poene");
        }
    }
}
