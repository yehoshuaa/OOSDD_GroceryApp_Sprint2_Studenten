using Grocery.Core.Helpers;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IClientService _clientService;
        public AuthService(IClientService clientService)
        {
            _clientService = clientService;
        }
        public Client? Login(string email, string password)
        {
            var client = _clientService.Get(email);

            if (client == null)
                return null;

            //Haal opgeslagen (gehashte) wachtwoord op
            var storedPassword = client.Password;

            //Controleer of ingevoerd wachtwoord klopt
            var isValid = PasswordHelper.VerifyPassword(password, storedPassword);

            //Bij succes: return client, anders null
            return isValid ? client : null;
        }
    }
}
