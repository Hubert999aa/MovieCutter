using Application.Functions.MovieCutter.Source.Commands.CreateSourceCommand;
using Application.Functions.MovieCutter.Source.Commands.DeleteSourceCommand;
using Application.Functions.MovieCutter.Source.Commands.UpdateSourceCommand;
using Application.Functions.MovieCutter.Source.Queries.GetSourceLastVideosQuery;
using Application.Functions.MovieCutter.Source.Queries.GetSourcesListQuery;
using Application.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace MovieCutterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SourceController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("getSourceList")]
        public async Task<IActionResult> GetSourceList(int id)
        {
            var response = await _mediator.Send(new GetSourcesListQuery() { IdProfile = id });

            if (response.Success)
            {
                return Ok(response.Payload);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpGet]
        [Route("getLastVideosList")]
        public async Task<IActionResult> GetLastVideosList(int id)
        {
            var response = await _mediator.Send(new GetSourceLastVideosQuery() { IdSource = id });

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
        [Route("createSource")]
        public async Task<IActionResult> CreateSource([FromBody] CreateSourceCommand command)
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
        [Route("updateSource")]
        public async Task<IActionResult> UpdateSource([FromBody] UpdateSourceCommand command)
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
        [Route("deleteSource")]
        public async Task<IActionResult> DeleteSource([FromBody] DeleteSourceCommand command)
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
