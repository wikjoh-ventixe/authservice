using Business.Interfaces;
using Business.Services;
using Grpc.Net.Client;
using Grpc.CustomerAuth;
using Grpc.UserAuth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(provider =>
{
    var channel = GrpcChannel.ForAddress(builder.Configuration["CustomerAuthApi"]!);
    return new GrpcCustomerAuth.GrpcCustomerAuthClient(channel);
});
builder.Services.AddSingleton(provider =>
{
    var channel = GrpcChannel.ForAddress(builder.Configuration["UserAuthApi"]!);
    return new GrpcUserAuth.GrpcUserAuthClient(channel);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Customer", policy =>
        policy.RequireClaim("UserType", "Customer"));
    options.AddPolicy("User", policy =>
        policy.RequireClaim("UserType", "User"));
});

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IVerificationService, VerificationService>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth Service API");
    c.RoutePrefix = string.Empty;
});

app.MapOpenApi();
app.UseHttpsRedirection();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
