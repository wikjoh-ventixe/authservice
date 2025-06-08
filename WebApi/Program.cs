using Business.Interfaces;
using Business.Services;
using Grpc.Net.Client;
using Protos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSingleton(provider =>
{
    var channel = GrpcChannel.ForAddress(builder.Configuration.GetValue<string>("CustomerServiceApi")!);
    return new GrpcCustomer.GrpcCustomerClient(channel);
});

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
