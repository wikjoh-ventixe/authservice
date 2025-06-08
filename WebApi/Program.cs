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

var app = builder.Build();
app.MapOpenApi();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
