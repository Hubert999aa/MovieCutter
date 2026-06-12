using Application.Functions.MovieCutter.VideoProcessing.Requests.CutVideoIntoFramesRequest;
using Application.Functions.MovieCutter.VideoProcessing.Requests.CutVideoIntoPicesRequest;
using Application.Functions.MovieCutter.VideoProcessing.Requests.DownloadAndCutVideoRequest;
using Application.Functions.MovieCutter.VideoProcessing.Requests.DownloadVideoRequest;
using Microsoft.AspNetCore.Mvc;
using MyMediator.Interfaces;

namespace MovieCutterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoProcessingController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("runVideoDownloading")]
        public async Task<IActionResult> RunVideoDownloading([FromBody] DownloadVideoRequest request)
        {
            var response = await _mediator.Send(request);

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
        [Route("runVideoCuttingIntoPices")]
        public async Task<IActionResult> RunVideoCuttingIntoPices([FromBody] CutVideoIntoPicesRequest request)
        {
            var response = await _mediator.Send(request);

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
        [Route("runVideoCuttingIntoFrames")]
        public async Task<IActionResult> RunVideoCuttingIntoFrames([FromBody] CutVideoIntoFramesRequest request)
        {
            var response = await _mediator.Send(request);

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
        [Route("runDownloadAndCutVideo")]
        public async Task<IActionResult> RunDownloadAndCutVideo([FromBody] DownloadAndCutVideoRequest request)
        {
            var response = await _mediator.Send(request);

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
