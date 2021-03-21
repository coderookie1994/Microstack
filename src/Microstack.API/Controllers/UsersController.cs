using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microstack.API.Abstractions;
using Microstack.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microstack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(string userId)
        {
            var profiles = await _userService.GetProfiles(userId);
            if (profiles.Count > 0)
                return Ok(profiles);
            return NotFound($"No user profiles found for user with id {userId}");
        }

        [HttpPost("{userId}/profile")]
        public async Task<IActionResult> PostProfile(string userId, [FromBody] Profile profile)
        {
            var invalidConfigurations = new List<(Boolean IsValid, string Message)>();
            var validationResult = profile.Configurations.SelectMany(p => p.Value.Select(c => c.Validate()));
            foreach(var validation in validationResult)
            {
                if (!validation.IsValid)
                    invalidConfigurations.Add(validation);
            }

            if (invalidConfigurations.Count > 0)
                return BadRequest(invalidConfigurations);

            try
            {
                await _userService.PersistProfile(userId, profile);
            }
            catch(Exception)
            {
                return StatusCode(500);
            }

            return NoContent();
        }
    }
}
