using Application.Functions.MovieCutter.Profile.Commands.CreateProfileCommand;
using Application.Functions.MovieCutter.Profile.Commands.DeleteProfileCommand;
using Application.Functions.MovieCutter.Profile.Commands.UpdateProfileCommand;
using Application.Functions.MovieCutter.Profile.Queries.GetProfilesListQuery;
using MyMediator.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MovieCutterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("getProfileList")]
        public async Task<IActionResult> GetProfileList()
        {
            var response = await _mediator.Send(new GetProfilesListQuery());

            if (response.Success)
            {
                return Ok(response.Payload);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpPost]
        [Route("createProfile")]
        public async Task<IActionResult> CreateProfile([FromBody] CreateProfileCommand command)
        {
            var response = await _mediator.Send(command);

            if (response.Success)
            {
                return Ok(response.Payload);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpPut]
        [Route("updateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
        {
            var response = await _mediator.Send(command);

            if (response.Success)
            {
                return Ok(response.Payload);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpDelete]
        [Route("deleteProfile")]
        public async Task<IActionResult> DeleteProfile([FromBody] DeleteProfileCommand command)
        {
            var response = await _mediator.Send(command);

            if (response.Success)
            {
                return Ok(response.Payload);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }
    }
}
