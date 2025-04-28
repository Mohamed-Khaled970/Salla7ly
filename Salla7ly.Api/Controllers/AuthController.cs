using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Salla7ly.Api.Abstraction;
using Salla7ly.Application.Features.Authentication.Command.Contracts;

namespace Salla7ly.Api.Controllers
{
    [Route("[controller]")]
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

        [HttpPost("SendVreficationOtp")]
        public async Task<IActionResult> SendVreficationOtp([FromBody] SendVreficationOtpCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok()
                   : result.ToProblem();
        }

        [HttpPost("SendForgetPasswordOtp")]
        public async Task<IActionResult> SendForgetPasswordOtp([FromBody] SendForgetPasswordOtpCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok()
                   : result.ToProblem();
        }

        [HttpPost("ValidateForgetPasswordOtp")]
        public async Task<IActionResult> ValidateForgetPasswordOtp([FromBody] ValidateForgePasswordOtpCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok()
                   : result.ToProblem();
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok()
                   : result.ToProblem();
        }

        [HttpPost("GoogleSignIn")]
        public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Value)
                   : result.ToProblem();
        }

        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Value)
                   : result.ToProblem();
        }
    }
}
