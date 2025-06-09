using Business.Dtos;
using Business.Dtos.Asb;
using Business.Interfaces;
using Business.Models;
using Grpc.Core;
using Grpc.CustomerAuth;
using Grpc.UserAuth;
using System.Diagnostics;
using static Grpc.UserAuth.GrpcUserAuth;

namespace Business.Services;

public class AuthService(GrpcCustomerAuth.GrpcCustomerAuthClient grpcCustomerAuthClient, GrpcUserAuth.GrpcUserAuthClient grpcUserAuthClient, IJwtTokenService jwtTokenService, IVerificationService verificationService) : IAuthService
{
    private readonly GrpcCustomerAuth.GrpcCustomerAuthClient _grpcCustomerAuthClient = grpcCustomerAuthClient;
    private readonly GrpcUserAuth.GrpcUserAuthClient _grpcUserAuthClient = grpcUserAuthClient;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly IVerificationService _verificationService = verificationService;


    public async Task<AuthResult<AuthData>> RegisterCustomerAsync(CustomerRegistrationRequestDto request)
    {
        try
        {
            if (request == null)
                return AuthResult<AuthData>.BadRequest("Request cannot be null.");

            var grpcRequest = new CustomerRegistrationRequest
            {
                Email = request.Email,
                Password = request.Password,
            };

            var grpcResponse = await _grpcCustomerAuthClient.RegisterCustomerAsync(grpcRequest);
            if (!grpcResponse.Succeeded)
                return AuthResult<AuthData>.InternalServerError("Failed creating customer over gRPC.");

            var token = grpcResponse.AuthInfo.ConfirmEmailToken;
            await _verificationService.SendVerificationTokenAsync(new SendVerificationTokenRequest
            {
                Email = request.Email,
                Token = token
            });

            return AuthResult<AuthData>.Ok(new AuthData
            {
                UserId = grpcResponse.AuthInfo.UserId,
                UserType = "Customer"
            });
        }
        catch (RpcException ex)
        {
            Debug.WriteLine(ex.Message);
            return AuthResult<AuthData>.InternalServerError("RPC exception occured.");
        }
    }


    public async Task<AuthResult<AuthData>> CustomerLoginAsync(LoginRequestDto request)
    {
        try
        {
            var grpcRequest = new CustomerLoginRequest
            {
                Email = request.Email,
                Password = request.Password,
            };

            var grpcResponse = await _grpcCustomerAuthClient.LoginCustomerAsync(grpcRequest);

            if (grpcResponse.Succeeded)
            {
                if (grpcResponse.AuthInfo.EmailConfirmed == false)
                    return AuthResult<AuthData>.Unauthorized("Verify your email before logging in.");

                var token = _jwtTokenService.GenerateToken(
                    grpcResponse.AuthInfo.UserId,
                    grpcRequest.Email,
                    "Customer");

                return AuthResult<AuthData>.Ok(new AuthData
                {
                    Token = token,
                    UserType = "Customer",
                    UserId = grpcResponse.AuthInfo.UserId,
                    EmailConfirmed = grpcResponse.AuthInfo.EmailConfirmed
                });
            }

            return AuthResult<AuthData>.Unauthorized("Invalid login.");
        }
        catch (RpcException ex)
        {
            Debug.WriteLine($"RPC exception occured. {ex.Message}");
            return AuthResult<AuthData>.InternalServerError("RPC exception occured.");
        }
    }


    public async Task<AuthResult<AuthData>> CustomerVerifyEmailAsync(CustomerVerifyEmailRequestDto request)
    {
        try
        {
            var grpcRequest = new CustomerVerifyEmailRequest
            {
                Email = request.Email,
                Token = request.Token
            };

            var grpcResponse = await _grpcCustomerAuthClient.VerifyCustomerEmailAsync(grpcRequest);

            if (!grpcResponse.Succeeded)
                return AuthResult<AuthData>.InternalServerError("Failed verifying email.");

            return AuthResult<AuthData>.Ok(new AuthData
            {
                UserId = grpcResponse.AuthInfo.UserId,
                EmailConfirmed = grpcResponse.AuthInfo.EmailConfirmed,
            });
        }
        catch (RpcException ex)
        {
            Debug.WriteLine($"RPC exception occured. {ex.Message}");
            return AuthResult<AuthData>.InternalServerError("RPC exception occured.");
        }
    }


    public async Task<AuthResult<AuthData>> UserLoginAsync(LoginRequestDto request)
    {
        try
        {
            var grpcRequest = new UserLoginRequest
            {
                Email = request.Email,
                Password = request.Password,
            };

            var grpcResponse = await _grpcUserAuthClient.LoginUserAsync(grpcRequest);

            if (grpcResponse.Succeeded)
            {
                var token = _jwtTokenService.GenerateToken(
                    grpcResponse.AuthInfo.UserId,
                    grpcRequest.Email,
                    "User");

                return AuthResult<AuthData>.Ok(new AuthData
                {
                    Token = token,
                    UserType = "User",
                    UserId = grpcResponse.AuthInfo.UserId,
                    EmailConfirmed = grpcResponse.AuthInfo.EmailConfirmed
                });
            }

            return AuthResult<AuthData>.Unauthorized("Invalid login.");
        }
        catch (RpcException ex)
        {
            Debug.WriteLine($"RPC exception occured. {ex.Message}");
            return AuthResult<AuthData>.InternalServerError("RPC exception occured.");
        }
    }
}
