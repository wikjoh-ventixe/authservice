using Business.Dtos.Asb;
using Business.Models;

namespace Business.Interfaces;

public interface IVerificationService
{
    void SaveVerificationToken(SaveVerificationTokenRequest request);
    Task<VerificationResult<string>> SendVerificationTokenAsync(SendVerificationTokenRequest request);
    Task<VerificationResult<string>> VerifyVerificationToken(VerifyVerificationTokenRequest request);
}
