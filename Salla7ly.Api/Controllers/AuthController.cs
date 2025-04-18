using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Salla7ly.Api.Abstraction;
using Salla7ly.Application.Features.Authentication.Command.Contracts;

namespace Salla7ly.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator =  mediator ?? throw new ArgumentNullException(nameof(mediator)); 
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login ([FromBody] SignInCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Value)
                   : result.ToProblem();
        }

        [HttpPost("CraftmanSignUp")]
        public async Task<IActionResult> CraftmanSignUp([FromBody] CraftmanSignUpCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Value)
                   : result.ToProblem();
        }

        [HttpPost("UserSignUp")]
        public async Task<IActionResult> UserSignUp([FromBody] UserSignUpCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Value)
                   : result.ToProblem();
        }

        [HttpPost("SendOtp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok()
                   : result.ToProblem();
        }
    }
}
