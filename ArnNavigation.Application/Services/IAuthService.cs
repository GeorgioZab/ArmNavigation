namespace ArnNavigation.Application.Services
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(string login, string password, CancellationToken cancellationToken);
        Task<string> RegisterAsync(string login, string password, CancellationToken cancellationToken);
    }
}



