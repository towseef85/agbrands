using System.Threading.Tasks;
using AGBrand.Models.Api.Auth;
using AGBrand.Packages.Models;

namespace AGBrand.Contracts.Api.Auth
{
    public interface IAuthBL
    {
        Task<ServiceStatus<PostChangePasswordResponse>> ChangePasswordAsync(PostChangePasswordRequest postForgotPasswordRequest);

        Task<ServiceStatus<PostGetOtpResponse>> GetOtpAsync(PostGetOtpRequest postGetOtpRequest);

        Task<ServiceStatus<PostLoginResponse>> LoginAsync(PostLoginRequest postLoginRequest);

        Task<ServiceStatus<PostLogoutResponse>> LogoutAsync(PostLogoutRequest postLogoutRequest);

        Task<ServiceStatus<PostRefreshTokenResponse>> RefreshTokenAsync(PostRefreshTokenRequest postRefreshTokenRequest);

        Task<ServiceStatus<PostResendOtpResponse>> ResendOtpAsync(PostResendOtpRequest postResendOtpRequest);

        Task<ServiceStatus<PostSignUpResponse>> SignUpAsync(PostSignUpRequest postSignUpRequest);

        Task<ServiceStatus<PostValidateOtpResponse>> ValidateOtpAsync(PostValidateOtpRequest postValidateOtpRequest);
    }
}
