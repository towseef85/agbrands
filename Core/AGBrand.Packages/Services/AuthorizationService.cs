using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AGBrand.Packages.Contracts.JWT;
using AGBrand.Packages.Models.Configs.Services;

namespace AGBrand.Packages.Services
{
    public static class AuthService
    {
        public static AuthorizationPolicy BearerPolicy
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            }
        }

        public static void AddAuthentication(this IServiceCollection services,
            IConfiguration configuration,
            IJwtSecurityKey jwtSecurityKey,
            Action<AuthenticationOptions> authenticationAction = null,
            Action<JwtBearerOptions> jwtAction = null)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                authenticationAction?.Invoke(options);
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,

                    ValidateIssuer = true,
                    ValidIssuer = configuration["Token:ValidIssuer"],

                    ValidateAudience = true,
                    ValidAudience = configuration["Token:ValidAudience"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwtSecurityKey.PublicKey,

                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = messageReceivedContext => { return Task.CompletedTask; },
                    OnChallenge = challengeContext =>
                    {
                        challengeContext.HandleResponse();
                        challengeContext.Response.StatusCode = 419;

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = authenticationFailedContext => { return Task.CompletedTask; },
                    OnTokenValidated = tokenValidatedContext => { return Task.CompletedTask; }
                };

                jwtAction?.Invoke(options);
            });
        }

        public static void AddAuthorization(this IServiceCollection services,
                    List<AuthPolicy> authPolicies = null,
            Action<AuthorizationOptions> authorizationAction = null)
        {
            services.AddAuthorization(options =>
            {
                if (authPolicies != null)
                {
                    foreach (var authPolicy in authPolicies.Where(c => c.AuthorizationPolicy == null && !string.IsNullOrWhiteSpace(c.Claim) && c.Values != null))
                    {
                        options.AddPolicy(authPolicy.Name, action => { action.RequireClaim(authPolicy.Claim, authPolicy.Values); });
                    }

                    foreach (var authPolicy in authPolicies.Where(c => c.AuthorizationPolicy != null))
                    {
                        options.AddPolicy(authPolicy.Name, authPolicy.AuthorizationPolicy);
                    }
                }

                ////options.AddPolicy(SubscriptionPolicies.NoSubscription, policy => { policy.RequireClaim(nameof(Subscription), Subscription.NoSubscription.ToString()); });
                ////options.AddPolicy(SubscriptionPolicies.LimitedSubscription, policy => { policy.RequireClaim(nameof(Subscription), Subscription.Limited.ToString()); });
                ////options.AddPolicy(SubscriptionPolicies.FullSubscription, policy => { policy.RequireClaim(nameof(Subscription), Subscription.Full.ToString()); });

                authorizationAction?.Invoke(options);
            });
        }
    }
}
