using Business.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Business.Dtos.Asb;
using Business.Models;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Caching.Memory;
using Grpc.CustomerAuth;

namespace Business.Services;

public class VerificationService(IConfiguration configuration, IMemoryCache cache, GrpcCustomerAuth.GrpcCustomerAuthClient grpcCustomerAuthClient) : IVerificationService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IMemoryCache _cache = cache;
    private readonly GrpcCustomerAuth.GrpcCustomerAuthClient _grpcCustomerAuthClient = grpcCustomerAuthClient;

    public async Task<VerificationResult<string>> SendVerificationTokenAsync(SendVerificationTokenRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Token))
            return VerificationResult<string>.BadRequest("Request cannot be null.");

        var connectionString = _configuration["ASB:ConnectionString"];
        var queueName = _configuration["ASB:Queue"];

        var client = new ServiceBusClient(connectionString);
        var sender = client.CreateSender(queueName);

        try
        {

            var messageContent = new SendVerificationEmailDto { Email = request.Email, Token = request.Token };
            string serializedMessageContent = JsonSerializer.Serialize(messageContent);

            var message = new ServiceBusMessage(serializedMessageContent);
            await sender.SendMessageAsync(message);
            await sender.DisposeAsync();
            await client.DisposeAsync();

            SaveVerificationToken(new SaveVerificationTokenRequest { Email = request.Email, Token = request.Token, ValidFor = TimeSpan.FromMinutes(5) });
            return VerificationResult<string>.Ok("Verification email sent successfully.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return VerificationResult<String>.InternalServerError("Failed to send verification email.");
        }
    }

    public void SaveVerificationToken(SaveVerificationTokenRequest request)
    {
        _cache.Set(request.Email.ToLowerInvariant(), request.Token, request.ValidFor);
    }

    public async Task<VerificationResult<string>> VerifyVerificationToken(VerifyVerificationTokenRequest request)
    {
        var key = request.Email.ToLowerInvariant();
        var decodedCode = WebUtility.UrlDecode(request.Token);

        if (_cache.TryGetValue(key, out string? storedToken))
        {
            if (storedToken == request.Token)
            {
                _cache.Remove(key);
                // Send UserId and decoded token to CustomerService over gRPC
                var result = await _grpcCustomerAuthClient.ValidateEmailTokenAsync(new ValidateEmailTokenRequest
                {
                    Email = key,
                    Token = decodedCode,
                });
                if (result.StatusCode == 401)
                    return VerificationResult<string>.Unauthorized("Token validation failed.");

                if (!result.Succeeded)
                    return VerificationResult<string>.InternalServerError("Failed validating token.");

                return VerificationResult<string>.Ok("Verification successful.");
            }
        }

        return VerificationResult<string>.BadRequest("Invalid or expired verification code.");
    }
}
