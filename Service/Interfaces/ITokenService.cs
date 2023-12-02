using Data.ViewModels.Token.Models;
using Service.Abstract;

namespace Service.Interfaces
{
    public interface ITokenService : IService
    {
        public TokenViewModel GenerateAccessToken(string email, int id, IEnumerable<string> roleNames,
            string picture, string name, bool isLogin = false);
    }
}