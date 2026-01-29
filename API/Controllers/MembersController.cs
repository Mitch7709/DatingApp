using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class MembersController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetMembers()
        {
            var members = await context.Users.ToListAsync();
            return members;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetMember(string id)
        {
            var member = await context.Users.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return member;
        }
    }
}
