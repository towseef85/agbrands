using System.Threading.Tasks;
using AGBrand.Models.Api.Users;
using AGBrand.Packages.Models;

namespace AGBrand.Contracts.Api.Auth
{
    public interface IUsersBL
    {
        Task<ServiceStatus<PostAddUserResponse>> AddAsync(PostAddUserRequest postAddUserRequest);

        ////Task<ServiceStatus<XXX>> VerifySignInIdAsync(XXX xxx);

        ////Task<ServiceStatus<PostLoginResponse>> UpdateImageAsync(IFormFile imageStream);
    }
}
