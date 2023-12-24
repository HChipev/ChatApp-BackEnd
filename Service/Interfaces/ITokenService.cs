using Data.ViewModels.Token.Models;
using Service.Abstract;

namespace Service.Interfaces
{
    public interface ITokenService : IService
    {
        public Task<TokenViewModel> GenerateAccessTokenAsync(string email, int id, IEnumerable<string> roleNames,
            string picture, string name, bool isLogin = false);
    }
}