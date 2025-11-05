using ArmNavigation.Infrastructure.Postgres.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using ArnNavigation.Application.Services;
using ArmNavigation.Services;

namespace ArmNavigation;

public class Startup(IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration;

    public void ConfigureServices(IServiceCollection service)
    {
        service.AddCors();
        service.AddSerilog();
        service.AddRouting();
        service.AddControllers();
        service.AddEndpointsApiExplorer();
        service.AddSwaggerGen();

        // JWT authentication (parameters will be finalized when issuing tokens)
        var jwtKey = _configuration["Jwt:Key"] ?? "CHANGE_ME_DEV_KEY";
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "ArmNavigation";
        var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

        service
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
                };
            });

        service.AddAuthorization();

        // Application services
        service.AddScoped<JwtTokenService>();
        service.AddScoped<IAuthService, JwtAuthService>();
        service.AddScoped<IPasswordHasher, PasswordHasher>();

        service.ConfigurePostgresInfrastructure();
    }

    public void Configure(IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.UseRouting();

        applicationBuilder.UseSwagger();
        applicationBuilder.UseSwaggerUI();
        applicationBuilder.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        applicationBuilder.UseAuthentication();
        applicationBuilder.UseAuthorization();
        applicationBuilder.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

    }
}