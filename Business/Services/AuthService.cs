using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Protos;

namespace Business.Services;

public class AuthService(GrpcCustomer.GrpcCustomerClient grpcCustomerClient) : IAuthService
{
    private readonly GrpcCustomer.GrpcCustomerClient _grpcCustomerClient = grpcCustomerClient;


    // CREATE
    public async Task<AuthResult<string?>> RegisterCustomer(CustomerRegistrationRequestDto request)
    {
        if (request == null)
            return AuthResult<string?>.BadRequest("Request cannot be null.");

        var grpcRequest = new CustomerRegistrationRequest
        {
            Email = request.Email,
            Password = request.Password,
        };

        var grpcResponse = await _grpcCustomerClient.RegisterCustomerAsync(grpcRequest);
        if (!grpcResponse.Succeeded)
            return AuthResult<string?>.InternalServerError("Failed creating customer over gRPC.");

        return AuthResult<string?>.Ok(grpcResponse.UserId);
    }
}
