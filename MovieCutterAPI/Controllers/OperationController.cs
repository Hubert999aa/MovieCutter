using Application.Functions.MovieCutter.Operation.Queries.GetOperationStatusesQuery;
using Microsoft.AspNetCore.Mvc;
using MyMediator.Interfaces;

namespace MovieCutterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [Route("getOperationStatuses")]
        public async Task<IActionResult> GetOperationStatuses()
        {
            var response = await _mediator.Send(new GetOperationStatusesQuery());

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
