using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AGBrand.Contracts.Api.Auth;
using AGBrand.Models;
using AGBrand.Models.Api.Auth;
using AGBrand.Models.Domain;
using AGBrand.Packages.Contracts;
using AGBrand.Packages.Helpers.JWT;
using AGBrand.Packages.Models;
using AGBrand.Packages.Util;
using AGBrand.Repository;
using static AGBrand.Packages.Util.Hash;
using AGBrand.Implementations.Api.Base;

namespace AGBrand.Implementations.Api.Auth
{
    public class AuthBL : BaseBL, IAuthBL
    {
        private readonly IConfiguration _configuration;
        private readonly IContextLogger _contextLogger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtTokenBuilder _jwtTokenBuilder;
        private readonly SqlContext _repository;

        public AuthBL(SqlContext repository,
            IConfiguration configuration,
            JwtTokenBuilder jwtTokenBuilder,
            IHttpContextAccessor httpContextAccessor,
            IContextLogger contextLogger) : base(repository, configuration, httpContextAccessor, contextLogger)
        {
            _repository = repository;
            _configuration = configuration;
            _jwtTokenBuilder = jwtTokenBuilder;
            _httpContextAccessor = httpContextAccessor;
            _contextLogger = contextLogger;
        }

        public async Task<ServiceStatus<PostChangePasswordResponse>> ChangePasswordAsync(PostChangePasswordRequest postForgotPasswordRequest)
        {
            var (serviceStatus, actionTokenInvalid) = await VerifyActionToken<PostChangePasswordResponse>(postForgotPasswordRequest.ActionTokenSigned);

            if (actionTokenInvalid)
            {
                return serviceStatus;
            }

            var signInIdType = GetSignInIdType(postForgotPasswordRequest.ActionTokenSigned.ActionToken.SignInId);

            var hashProvider = new Hash(HashServiceProvider.Sha512)
            {
                Salt = _configuration["Settings:PasswordSalt"]
            };

            var passwordHash = hashProvider.Encrypt(postForgotPasswordRequest.Password);

            switch (signInIdType)
            {
                case SignInIdType.Email:
                    {
                        var userByEmail = await _repository.Users.FirstOrDefaultAsync(c => c.Email == postForgotPasswordRequest.ActionTokenSigned.ActionToken.SignInId && c.IsEmailVerified);

                        if (userByEmail == null)
                        {
                            return new ServiceStatus<PostChangePasswordResponse>
                            {
                                Object = null,
                                Code = HttpStatusCode.BadRequest,
                                Message = "Email Id Not Verified"
                            };
                        }

                        userByEmail.PasswordHash = passwordHash;

                        _repository.Users.Update(userByEmail);
                        break;
                    }

                case SignInIdType.Mobile:
                    {
                        var userByMobile = await _repository.Users.FirstOrDefaultAsync(c => c.Mobile == postForgotPasswordRequest.ActionTokenSigned.ActionToken.SignInId && c.IsMobileVerified);

                        if (userByMobile == null)
                        {
                            return new ServiceStatus<PostChangePasswordResponse>
                            {
                                Object = null,
                                Code = HttpStatusCode.BadRequest,
                                Message = "Mobile Number Not Verified"
                            };
                        }

                        userByMobile.PasswordHash = passwordHash;

                        _repository.Users.Update(userByMobile);
                        break;
                    }
                default:
                    {
                        _contextLogger.Log($"Invalid SignIn Id: [{postForgotPasswordRequest.ActionTokenSigned.ActionToken.SignInId}]");
                        return new ServiceStatus<PostChangePasswordResponse>
                        {
                            Object = null,
                            Code = HttpStatusCode.BadRequest,
                            Message = "Invalid Email Id/Mobile"
                        };
                    }
            }

            await _repository.SaveChangesAsync();

            return new ServiceStatus<PostChangePasswordResponse>
            {
                Code = HttpStatusCode.OK,
                Message = "Password Changed Successfully",
                Object = null
            };
        }

        public async Task<ServiceStatus<PostGetOtpResponse>> GetOtpAsync(PostGetOtpRequest postGetOtpRequest)
        {
            var signInIdType = GetSignInIdType(postGetOtpRequest.SignInId);

            if (signInIdType == SignInIdType.Invalid)
            {
                _contextLogger.Log($"Invalid SignIn Id: [{postGetOtpRequest.SignInId}]");

                return new ServiceStatus<PostGetOtpResponse>
                {
                    Object = null,
                    Code = HttpStatusCode.BadRequest,
                    Message = "Invalid Email Id/Mobile"
                };
            }

            var otpInDb = await _repository.Otps.FirstOrDefaultAsync(c => c.SignInId == postGetOtpRequest.SignInId);

            (OtpTokenSigned otpTokenSigned, string otpCode, DateTime otpExpiresOn) = (null, string.Empty, DateTime.UtcNow);

            if (otpInDb != null)
            {
                (otpTokenSigned, otpCode, otpExpiresOn) = CreateSignedOtpToken<CreateUserActionData>(postGetOtpRequest.SignInId, otpInDb, null);

                _repository.Otps.Update(otpInDb);
            }
            else
            {
                var key = Guid.NewGuid();

                var otp = new Otp
                {
                    Id = key,
                    SignInId = postGetOtpRequest.SignInId,
                };

                (otpTokenSigned, otpCode, otpExpiresOn) = CreateSignedOtpToken<CreateUserActionData>(postGetOtpRequest.SignInId, otp, null);

                await _repository.Otps.AddAsync(otp);
            }

            await _repository.SaveChangesAsync();

            // TODO: Send otp over email or sms use the tuple (otpTokenSigned, otpCode,
            // otpExpiresOn) to get otp and expiry information

            return new ServiceStatus<PostGetOtpResponse>
            {
                Object = new PostGetOtpResponse
                {
                    OtpTokenSigned = otpTokenSigned
                },
                Code = HttpStatusCode.OK,
                Message = "Otp Sent. Validate Otp."
            };
        }

        public async Task<ServiceStatus<PostLoginResponse>> LoginAsync(PostLoginRequest postLoginRequest)
        {
            var hashProvider = new Hash(HashServiceProvider.Sha512)
            {
                Salt = _configuration["Settings:PasswordSalt"]
            };

            var passwordHash = hashProvider.Encrypt(postLoginRequest.Password);

            var signInIdType = GetSignInIdType(postLoginRequest.SignInId);

            switch (signInIdType)
            {
                case SignInIdType.Email:
                    {
                        var userByEmail = await _repository.Users.FirstOrDefaultAsync(c => c.Email == postLoginRequest.SignInId &&
                                                                                      c.IsEmailVerified &&
                                                                                      c.PasswordHash == passwordHash);

                        if (userByEmail != null)
                        {
                            var refreshToken = await CreateRefreshTokenAsync(userByEmail);

                            return GetLoginResponse(userByEmail, refreshToken);
                        }
                        break;
                    }
                case SignInIdType.Mobile:
                    {
                        var userByMobile = await _repository.Users.FirstOrDefaultAsync(c => c.Mobile == postLoginRequest.SignInId &&
                                                                                       c.IsMobileVerified &&
                                                                                       c.PasswordHash == passwordHash);

                        if (userByMobile != null)
                        {
                            var refreshToken = await CreateRefreshTokenAsync(userByMobile);

                            return GetLoginResponse(userByMobile, refreshToken);
                        }
                        break;
                    }
                case SignInIdType.Invalid:
                default:
                    {
                        return new ServiceStatus<PostLoginResponse>
                        {
                            Object = null,
                            Code = HttpStatusCode.BadRequest,
                            Message = "Invalid Email Id/Mobile"
                        };
                    }
            }

            return new ServiceStatus<PostLoginResponse>
            {
                Object = null,
                Code = HttpStatusCode.Unauthorized,
                Message = "Unauthorized"
            };


        }

        public async Task<ServiceStatus<PostLogoutResponse>> LogoutAsync(PostLogoutRequest postLogoutRequest)
        {
            var user = await _repository.Users
               .Include(c => c.RefreshTokens)
               .FirstOrDefaultAsync(c => c.Id == postLogoutRequest.UserId);

            // user doesn't exist with userId
            if (user == null)
            {
                return new ServiceStatus<PostLogoutResponse>
                {
                    Code = HttpStatusCode.ResetContent,
                    Message = "Logout Failed"
                };
            }

            var refreshToken = user.RefreshTokens.FirstOrDefault(c => c.Token == postLogoutRequest.RefreshToken);

            // refresh token doesn't exist
            if (refreshToken == null)
            {
                return new ServiceStatus<PostLogoutResponse>
                {
                    Code = HttpStatusCode.ResetContent,
                    Message = "Invalid Refresh Token"
                };
            }

            // refresh token exists but expired
            if (refreshToken.ExpiresOn < DateTime.UtcNow)
            {
                user.RefreshTokens.Remove(refreshToken);

                await _repository.SaveChangesAsync();

                return new ServiceStatus<PostLogoutResponse>
                {
                    Code = HttpStatusCode.ResetContent,
                    Message = "Refresh Token Expired"
                };
            }

            if (postLogoutRequest.LogoutFromAllSessions)
            {
                user.RefreshTokens.Clear();

                await _repository.SaveChangesAsync();
            }

            return new ServiceStatus<PostLogoutResponse>
            {
                Code = HttpStatusCode.OK,
                Message = postLogoutRequest.LogoutFromAllSessions ? "Successfully Logged Out From All Sessions" : "Logged Out From Current Session"
            };
        }

        public async Task<ServiceStatus<PostRefreshTokenResponse>> RefreshTokenAsync(PostRefreshTokenRequest postRefreshTokenRequest)
        {
            var user = await _repository.Users
                .Include(c => c.RefreshTokens)
                .FirstOrDefaultAsync(c => c.Id == postRefreshTokenRequest.UserId);

            // user doesn't exist with userId
            if (user == null)
            {
                return new ServiceStatus<PostRefreshTokenResponse>
                {
                    Code = HttpStatusCode.ResetContent,
                    Message = "Refresh Token Failed"
                };
            }

            var refreshToken = user.RefreshTokens.FirstOrDefault(c => c.Token == postRefreshTokenRequest.RefreshToken);

            // refresh token doesn't exist
            if (refreshToken == null)
            {
                return new ServiceStatus<PostRefreshTokenResponse>
                {
                    Code = HttpStatusCode.ResetContent,
                    Message = "Invalid Refresh Token"
                };
            }

            // refresh token exists but expired
            if (refreshToken.ExpiresOn < DateTime.UtcNow)
            {
                user.RefreshTokens.Remove(refreshToken);

                await _repository.SaveChangesAsync();

                return new ServiceStatus<PostRefreshTokenResponse>
                {
                    Code = HttpStatusCode.ResetContent,
                    Message = "Refresh Token Expired"
                };
            }

            // update same refresh token with new refresh token update the expiry time
            refreshToken.Token = Guid.NewGuid().ToString().Replace("-", string.Empty);
            refreshToken.ExpiresOn = DateTime.UtcNow.AddYears(1);

            await _repository.SaveChangesAsync();

            (JwtToken jwtToken, DateTime jwtTokenExpiresOn) = GenerateLoginToken(user);

            return new ServiceStatus<PostRefreshTokenResponse>
            {
                Object = new PostRefreshTokenResponse
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Mobile = user.Mobile,
                    Name = string.Empty,
                    SessionToken = new SessionToken
                    {
                        ExpiresOn = jwtTokenExpiresOn,
                        Token = jwtToken.Value,
                        Scheme = JwtBearerDefaults.AuthenticationScheme
                    },
                    RefreshToken = new SessionToken
                    {
                        ExpiresOn = refreshToken.ExpiresOn,
                        Token = refreshToken.Token,
                        Scheme = Packages.Models.Constants.ApplicationAccessToken
                    }
                },
                Code = HttpStatusCode.OK,
                Message = "Token Refreshed Successfully"
            };
        }

        public async Task<ServiceStatus<PostResendOtpResponse>> ResendOtpAsync(PostResendOtpRequest postResendOtpRequest)
        {
            var otpTokenHash = new Hash(HashServiceProvider.Sha512)
            {
                Salt = _configuration["Settings:PasswordSalt"]
            }.Encrypt(JsonConvert.SerializeObject(postResendOtpRequest.OtpTokenSigned.OtpToken, Formatting.None));

            if (otpTokenHash != postResendOtpRequest.OtpTokenSigned.OtpTokenHash)
            {
                return new ServiceStatus<PostResendOtpResponse>
                {
                    Code = HttpStatusCode.Unauthorized,
                    Message = "Otp Token Signature Invalid"
                };
            }

            var otp = await _repository.Otps.FirstOrDefaultAsync(c => c.Id == postResendOtpRequest.OtpTokenSigned.OtpToken.OtpId &&
                                                                 c.SignInId == postResendOtpRequest.OtpTokenSigned.OtpToken.SignInId);

            if (otp == null)
            {
                return new ServiceStatus<PostResendOtpResponse>
                {
                    Code = HttpStatusCode.BadRequest,
                    Message = "Invalid Otp Request"
                };
            }

            if (otp.ExpiresOn < DateTime.UtcNow)
            {
                _repository.Otps.Remove(otp);

                await _repository.SaveChangesAsync();

                return new ServiceStatus<PostResendOtpResponse>
                {
                    Code = HttpStatusCode.BadRequest,
                    Message = "Otp Request Expired"
                };
            }

            (OtpTokenSigned otpTokenSigned, string otpCode, DateTime otpExpiresOn) = CreateSignedOtpToken<CreateUserActionData>(postResendOtpRequest.OtpTokenSigned.OtpToken.SignInId, otp, null);

            _repository.Otps.Update(otp);

            await _repository.SaveChangesAsync();

            // TODO: send otp over email or sms

            return new ServiceStatus<PostResendOtpResponse>
            {
                Code = HttpStatusCode.OK,
                Object = new PostResendOtpResponse
                {
                    OtpTokenSigned = otpTokenSigned
                }
            };
        }

        public async Task<ServiceStatus<PostSignUpResponse>> SignUpAsync(PostSignUpRequest postSignUpRequest)
        {
            var signInIdType = GetSignInIdType(postSignUpRequest.SignInId);

            switch (signInIdType)
            {
                case SignInIdType.Email:
                    var userByEmail = await _repository.Users.FirstOrDefaultAsync(c => c.Email == postSignUpRequest.SignInId && c.IsEmailVerified);

                    if (userByEmail != null)
                    {
                        return new ServiceStatus<PostSignUpResponse>
                        {
                            Object = null,
                            Code = HttpStatusCode.BadRequest,
                            Message = "Email Id Unavailable For Signup"
                        };
                    }
                    break;

                case SignInIdType.Mobile:
                    var userByMobile = await _repository.Users.FirstOrDefaultAsync(c => c.Mobile == postSignUpRequest.SignInId && c.IsMobileVerified);

                    if (userByMobile != null)
                    {
                        return new ServiceStatus<PostSignUpResponse>
                        {
                            Object = null,
                            Code = HttpStatusCode.BadRequest,
                            Message = "Mobile Number Unavailable For Signup"
                        };
                    }
                    break;

                default:
                    _contextLogger.Log($"Invalid SignIn Id: [{postSignUpRequest.SignInId}]");
                    return new ServiceStatus<PostSignUpResponse>
                    {
                        Object = null,
                        Code = HttpStatusCode.BadRequest,
                        Message = "Invalid Email Id/Mobile"
                    };
            }

            var otpInDb = await _repository.Otps.FirstOrDefaultAsync(c => c.SignInId == postSignUpRequest.SignInId && c.ExpiresOn < DateTime.UtcNow);

            (OtpTokenSigned otpTokenSigned, string otpCode, DateTime otpExpiresOn) = (null, string.Empty, DateTime.UtcNow);

            if (otpInDb != null)
            {
                (otpTokenSigned, otpCode, otpExpiresOn) = CreateSignedOtpToken<CreateUserActionData>(postSignUpRequest.SignInId, otpInDb, new CreateUserActionData
                {
                    Password = postSignUpRequest.Password
                });

                _repository.Otps.Update(otpInDb);
            }
            else
            {
                var key = Guid.NewGuid();

                var otp = new Otp
                {
                    Id = key,
                    SignInId = postSignUpRequest.SignInId
                };

                (otpTokenSigned, otpCode, otpExpiresOn) = CreateSignedOtpToken<CreateUserActionData>(postSignUpRequest.SignInId, otp, new CreateUserActionData
                {
                    Password = postSignUpRequest.Password
                });

                await _repository.Otps.AddAsync(otp);
            }

            await _repository.SaveChangesAsync();

            // TODO: Send otp over email or sms use the tuple (otpTokenSigned, otpCode,
            // otpExpiresOn) to get otp and expiry information

            return new ServiceStatus<PostSignUpResponse>
            {
                Object = new PostSignUpResponse
                {
                    OtpTokenSigned = otpTokenSigned
                },
                Code = HttpStatusCode.OK,
                Message = "Otp Sent. Validate Otp."
            };
        }

        public async Task<ServiceStatus<PostValidateOtpResponse>> ValidateOtpAsync(PostValidateOtpRequest postValidateOtpRequest)
        {
            var otpTokenSerialized = JsonConvert.SerializeObject(postValidateOtpRequest.OtpTokenSigned.OtpToken, Formatting.None);

            var otpTokenHash = new Hash(HashServiceProvider.Sha512)
            {
                Salt = _configuration["Settings:PasswordSalt"]
            }.Encrypt(otpTokenSerialized);

            if (otpTokenHash != postValidateOtpRequest.OtpTokenSigned.OtpTokenHash)
            {
                return new ServiceStatus<PostValidateOtpResponse>
                {
                    Code = HttpStatusCode.Unauthorized,
                    Message = "Otp Token Signature Invalid"
                };
            }

            var otp = await _repository.Otps.FirstOrDefaultAsync(c => c.Id == postValidateOtpRequest.OtpTokenSigned.OtpToken.OtpId &&
                                                                 c.SignInId == postValidateOtpRequest.OtpTokenSigned.OtpToken.SignInId);

            if (otp == null)
            {
                return new ServiceStatus<PostValidateOtpResponse>
                {
                    Code = HttpStatusCode.Unauthorized,
                    Message = "Invalid Otp"
                };
            }

            if (otp.Code != postValidateOtpRequest.Otp)
            {
                return new ServiceStatus<PostValidateOtpResponse>
                {
                    Code = HttpStatusCode.Unauthorized,
                    Message = "Incorrect Otp"
                };
            }

            if (otp.ExpiresOn < DateTime.UtcNow)
            {
                return new ServiceStatus<PostValidateOtpResponse>
                {
                    Code = HttpStatusCode.Unauthorized,
                    Message = "Otp Expired"
                };
            }

            _repository.Otps.Remove(otp);

            var actionToken = new ActionToken
            {
                ActionId = Guid.NewGuid(),
                Data = postValidateOtpRequest.OtpTokenSigned.OtpToken.Data,
                ExpiresOn = DateTime.UtcNow.AddMinutes(10),
                SignInId = postValidateOtpRequest.OtpTokenSigned.OtpToken.SignInId
            };

            await _repository.Actions.AddAsync(new Models.Domain.Action
            {
                ExpiresOn = actionToken.ExpiresOn,
                SignInId = actionToken.SignInId,
                Id = actionToken.ActionId
            });

            await _repository.SaveChangesAsync();

            var actionTokenHash = new Hash(HashServiceProvider.Sha512)
            {
                Salt = _configuration["Settings:PasswordSalt"]
            }.Encrypt(JsonConvert.SerializeObject(actionToken, Formatting.None));

            return new ServiceStatus<PostValidateOtpResponse>
            {
                Code = HttpStatusCode.OK,
                Object = new PostValidateOtpResponse
                {
                    ActionTokenSigned = new ActionTokenSigned
                    {
                        ActionToken = actionToken,
                        ActionTokenHash = actionTokenHash
                    }
                }
            };
        }

        #region PrivateMethods

        ////private async Task<ServiceStatus<PostValidateOtpResponse>> CreateUser(PostValidateOtpRequest postValidateOtpRequest, Otp userOtp)
        ////{
        ////    var user = new User
        ////    {
        ////        Email = postValidateOtpRequest.TempToken.UserIdType == UserIdType.Email ? postValidateOtpRequest.TempToken.UserId : null,
        ////        Mobile = postValidateOtpRequest.TempToken.UserIdType == UserIdType.Mobile ? postValidateOtpRequest.TempToken.UserId : null,
        ////        Id = Guid.NewGuid(),
        ////        IsDeleted = false,
        ////        IsEmailVerified = postValidateOtpRequest.TempToken.UserIdType == UserIdType.Email,
        ////        IsMobileVerified = postValidateOtpRequest.TempToken.UserIdType == UserIdType.Mobile,
        ////        PasswordHash = ""
        ////    };

        ////    await _repository.Users.AddAsync(user);

        ////    _repository.Otps.Remove(userOtp);

        ////    await _repository.SaveChangesAsync();

        ////    return new ServiceStatus<PostValidateOtpResponse>
        ////    {
        ////        Code = HttpStatusCode.Created,
        ////        Message = "User Created Successfully",
        ////        Object = new PostValidateOtpResponse
        ////        {
        ////            Id = user.Id,
        ////            TempToken = null
        ////        }
        ////    };
        ////}

        private async Task<RefreshToken> CreateRefreshTokenAsync(User user)
        {
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString().Replace("-", string.Empty),
                ExpiresOn = DateTime.UtcNow.AddYears(1),
                Id = Guid.NewGuid(),
                UserId = user.Id
            };

            await _repository.RefreshTokens.AddAsync(refreshToken);

            await _repository.SaveChangesAsync();

            return refreshToken;
        }

        private ServiceStatus<PostLoginResponse> GetLoginResponse(User user, RefreshToken refreshToken)
        {
            (JwtToken jwtToken, DateTime jwtTokenExpiresOn) = GenerateLoginToken(user);

            return new ServiceStatus<PostLoginResponse>
            {
                Object = new PostLoginResponse
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Mobile = user.Mobile,
                    Name = string.Empty,
                    SessionToken = new SessionToken
                    {
                        ExpiresOn = jwtTokenExpiresOn,
                        Token = jwtToken.Value,
                        Scheme = JwtBearerDefaults.AuthenticationScheme
                    },
                    RefreshToken = new SessionToken
                    {
                        ExpiresOn = refreshToken.ExpiresOn,
                        Token = refreshToken.Token,
                        Scheme = Packages.Models.Constants.ApplicationAccessToken
                    }
                },
                Code = HttpStatusCode.OK,
                Message = "Token Refreshed Successfully"
            };
        }

        private (OtpTokenSigned otpTokenSigned, string otpCode, DateTime otpExpiresOn) CreateSignedOtpToken<T>(string signInId, Otp otp, T data)
        {
            otp.Code = new Random().Next(99999, 999999).ToString();
            otp.ExpiresOn = DateTime.UtcNow.AddHours(1);

            // TODO: remove this line once otp is sent over email/sms
            _httpContextAccessor.HttpContext.Response.Headers.Add("x-otp-to-remove", otp.Code);

            var otpToken = new OtpToken
            {
                SignInId = signInId,
                Data = JsonConvert.SerializeObject(data),
                ExpiresOn = otp.ExpiresOn,
                OtpId = otp.Id
            };

            var otpTokenSerialized = JsonConvert.SerializeObject(otpToken, Formatting.None);

            var otpTokenHash = new Hash(HashServiceProvider.Sha512)
            {
                Salt = _configuration["Settings:PasswordSalt"]
            }.Encrypt(otpTokenSerialized);

            return (new OtpTokenSigned
            {
                OtpToken = otpToken,
                OtpTokenHash = otpTokenHash
            }, otp.Code, otp.ExpiresOn);
        }

        private (JwtToken jwtToken, DateTime jwtTokenExpiresOn) GenerateLoginToken(User user)
        {
            // session token expiry time
            var jwtTokenExpiresOn = DateTime.UtcNow.AddMinutes(60);
            var jwtToken = _jwtTokenBuilder.AddClaim(nameof(User.Email), user.Email)
                                        .AddClaim(nameof(User.Mobile), user.Mobile)
                                        .AddClaim(nameof(AuthSessionTokenResponse.UserId), user.Id)
                                        .AddExpiryByTime(jwtTokenExpiresOn)
                                        .AddExpiryByMinutes(60)
                                        .AddSubject(nameof(AGBrand))
                                        .Build(byMinutes: false);

            return (jwtToken, jwtTokenExpiresOn);
        }

        #endregion PrivateMethods
    }
}
