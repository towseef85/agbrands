using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Threading.Tasks;
using AGBrand.Models.Api.Auth;
using AGBrand.Packages.Contracts;
using AGBrand.Contracts;
using AGBrand.Packages.Util;

namespace AGBrand.Api.Controllers
{
    /// <summary>
    /// Auth Api Controller
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly IWrapper _wrapper;

        /// <summary>
        /// AuthController Constructor
        /// </summary>
        public AuthController(IWrapper wrapper)
        {
            _wrapper = wrapper;
        }

        /// <summary>
        /// Forgot Password
        /// </summary>
        /// <param name="postForgotPasswordRequest"></param>
        /// <response code="200">Successfully Changed The Password</response>
        /// <response code="401">Invalid Or Expired Action Token</response>
        /// <response code="400">Email Id/Mobile Invalid or Not Verified</response>
        [HttpPost("changePassword")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostChangePasswordResponse))]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] PostChangePasswordRequest postForgotPasswordRequest)
        {
            return await _wrapper.AuthBL.ChangePasswordAsync(postForgotPasswordRequest).RespondAsync((IApiRespondService)_wrapper);
        }

        /// <summary>
        /// Get Otp For Forgot Password/Change Password
        /// </summary>
        /// <param name="postGetOtpRequest"></param>
        /// <response code="200">Successfully Sent Otp For Email Id/Mobile Validation</response>
        /// <response code="400">Invalid Email Id/Mobile</response>
        [HttpPost("getOtp")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostGetOtpResponse))]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "x-otp-to-remove", "string", "otp in header to validate till email/sms service is integrated", "123456")]
        public async Task<IActionResult> GetOtpAsync([FromBody] PostGetOtpRequest postGetOtpRequest)
        {
            return await _wrapper.AuthBL.GetOtpAsync(postGetOtpRequest).RespondAsync((IApiRespondService)_wrapper);
        }

        /// <summary>
        /// Login User
        /// </summary>
        /// <param name="postLoginRequest"></param>
        /// <response code="200">Login Successful</response>
        /// <response code="400">Invalid Email Id/Mobile</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostLoginResponse))]
        public async Task<IActionResult> LoginAsync([FromBody] PostLoginRequest postLoginRequest)
        {
            return await _wrapper.AuthBL.LoginAsync(postLoginRequest).RespondAsync((IApiRespondService)_wrapper);
        }

        /// <summary>
        /// Logout User
        /// </summary>
        /// <param name="postLogoutRequest"></param>
        /// <response code="200">Logout Successful</response>
        /// <response code="205">Refresh Token Expired/Invalid or Logout Failed</response>
        [HttpPost("logout")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostLogoutResponse))]
        public async Task<IActionResult> LogoutAsync([FromBody] PostLogoutRequest postLogoutRequest)
        {
            return await _wrapper.AuthBL.LogoutAsync(postLogoutRequest).RespondAsync((IApiRespondService)_wrapper);
        }

        /// <summary>
        /// Refresh Auth Token
        /// </summary>
        /// <param name="postRefreshTokenRequest"></param>
        /// <response code="200">Refresh Token Successful</response>
        /// <response code="205">Refresh Token Expired/Invalid or Refresh Token Failed</response>
        [HttpPost("refreshToken")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostRefreshTokenResponse))]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] PostRefreshTokenRequest postRefreshTokenRequest)
        {
            return await _wrapper.AuthBL.RefreshTokenAsync(postRefreshTokenRequest).RespondAsync((IApiRespondService)_wrapper);
        }

        /// <summary>
        /// Resend Otp
        /// </summary>
        /// <param name="postResendOtpRequest"></param>
        /// <response code="200">Resend Otp Successful</response>
        /// <response code="400">Otp Request Expired/Invalid</response>
        /// <response code="401">Otp Token Signature Invalid</response>
        [HttpPost("resendOtp")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostResendOtpResponse))]
        public async Task<IActionResult> ResendOtpAsync([FromBody] PostResendOtpRequest postResendOtpRequest)
        {
            return await _wrapper.AuthBL.ResendOtpAsync(postResendOtpRequest).RespondAsync((IApiRespondService)_wrapper);
        }

        /// <summary>
        /// Signup User
        /// </summary>
        /// <param name="postSignUpRequest"></param>
        /// <response code="200">Successfully Sent Otp For Email Id/Mobile Validation</response>
        /// <response code="400">Email Id/Mobile Invalid/Unavailable For Signup</response>
        [HttpPost("signup")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostSignUpResponse))]
        public async Task<IActionResult> SignUpAsync([FromBody] PostSignUpRequest postSignUpRequest)
        {
            return await _wrapper.AuthBL.SignUpAsync(postSignUpRequest).RespondAsync((IApiRespondService)_wrapper);
        }

        /// <summary>
        /// Validate Otp
        /// </summary>
        /// <param name="postValidateOtpRequest"></param>
        /// <response code="200">Otp Token Validated And Successfully Created Action Token</response>
        /// <response code="400">Otp Request Invalid/Expired</response>
        /// <response code="401">Otp Token Signature Invalid</response>
        [HttpPost("validateOtp")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostValidateOtpResponse))]
        public async Task<IActionResult> ValidateOtpAsync([FromBody] PostValidateOtpRequest postValidateOtpRequest)
        {
            return await _wrapper.AuthBL.ValidateOtpAsync(postValidateOtpRequest).RespondAsync((IApiRespondService)_wrapper);
        }
    }
}
