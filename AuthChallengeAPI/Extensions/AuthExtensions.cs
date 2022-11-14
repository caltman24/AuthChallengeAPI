using System.Text;
using AuthChallengeAPI.Constants;
using AuthChallengeAPI.TestData;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace AuthChallengeAPI.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        return services.AddAuthorization(opts =>
        {
            opts.AddPolicy(PolicyConstants.MustBeTheOwner, policy =>
            {
                policy.RequireClaim("title", UserTitles.Owner);
            });
            
            opts.AddPolicy(PolicyConstants.MustBeTheManager, policy =>
            {
                policy.RequireClaim("title", UserTitles.Owner, UserTitles.Manager);
            });
            
            // Fallback policy
            opts.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser().Build();
        });
    }

    public static AuthenticationBuilder AddAuthenticationOptions(this IServiceCollection services,
        IConfiguration config)
    {
        return services.AddAuthentication("Bearer")
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config.GetValue<string>("Authentication:Issuer"),
                    ValidAudience = config.GetValue<string>("Authentication:Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(config.GetValue<string>("Authentication:SecretKey")))
                };
            });
    }
}