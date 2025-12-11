using System.Security.Claims;
using TimViecLam.Models.Dto.Request;
using TimViecLam.Models.Dto.Response;

namespace TimViecLam.Repository.IRepository
{
    public interface IAuthRepository
    {
        Task<AuthResult> RegisterCandidateAsync(RegisterCandidateRequest requestDto);

        Task<AuthResult> RegisterEmployerAsync(RegisterEmployerRequest requestDto);
        Task<AuthResult> RegisterAdminAsync(RegisterAdminRequest request);
        Task<AuthResult> ForgotPasswordAsync(ForgotPasswordRequest requestDto);
        Task<AuthResult> ResetPasswordAsync(ResetPasswordRequest requestDto);
        Task<AuthResult> LoginWithGoogleAsync(GoogleLoginRequest requestDto);
        Task<AuthResult> RegisterWithGoogleAsync(GoogleRegisterRequest requestDto);
        Task<AuthResult> LoginAsync(LoginRequest requestDto);
    }
}
