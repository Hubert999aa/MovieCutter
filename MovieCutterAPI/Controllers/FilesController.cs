using Application.Functions.MovieCutter.Files.Queries.GetVideoListFromDiskQuery;
using MyMediator.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MovieCutterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("getFilesList")]
        public async Task<IActionResult> GetFilesList()
        {
            var response = await _mediator.Send(new GetVideoListFromDiskQuery());

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
