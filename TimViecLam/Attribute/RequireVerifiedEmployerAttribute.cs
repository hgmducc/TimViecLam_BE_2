using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TimViecLam.Data;

namespace TimViecLam.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireVerifiedEmployerAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;

            // Lấy userId từ claims
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    isSuccess = false,
                    message = "Không thể xác thực người dùng."
                });
                return;
            }

            // Lấy DbContext từ DI
            var dbContext = httpContext.RequestServices.GetService<ApplicationDbContext>();

            if (dbContext == null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

            // Kiểm tra employer có verified không
            var employer = await dbContext.Employers
                .FirstOrDefaultAsync(e => e.EmployerID == userId);

            if (employer == null)
            {
                context.Result = new NotFoundObjectResult(new
                {
                    isSuccess = false,
                    errorCode = "EMPLOYER_NOT_FOUND",
                    message = "Không tìm thấy thông tin nhà tuyển dụng."
                });
                return;
            }

            if (employer.VerificationStatus != "Verified")
            {
                context.Result = new ObjectResult(new
                {
                    isSuccess = false,
                    errorCode = "NOT_VERIFIED",
                    message = "Tài khoản của bạn chưa được xác thực.  Vui lòng chờ admin duyệt giấy phép kinh doanh.",
                    verificationStatus = employer.VerificationStatus
                })
                {
                    StatusCode = 403
                };
                return;
            }

            // Cho phép tiếp tục
            await next();
        }
    }
}