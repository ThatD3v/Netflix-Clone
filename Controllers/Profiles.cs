using Microsoft.AspNetCore.Mvc;
using NetflixClone.Data;
using NetflixClone.DTOs;

namespace NetflixClone.Controllers
{

    [ApiController]
    [Route("api/[controller]")]

    public class Profiles : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public Profiles(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Createprofile(CreateProfileDto dto);
        {
        var Profiles = await _context.Profiles
            .Where(p => )


    }
}
