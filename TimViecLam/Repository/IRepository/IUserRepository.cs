namespace TimViecLam.Repository.IRepository
{
    public interface IUserRepository 
    {
        Task<bool> EmailExitAsync(string email);
        Task<bool> PhoneExitAsync(string phoneNumber);

    }
}
