using Common.Classes;
using Data.ViewModels.Identity.Models;
using Data.ViewModels.Token.Models;
using Service.Abstract;

namespace Service.Interfaces
{
    public interface IIdentityService : IService
    {
        public Task<ServiceResult<TokensResponseViewModel>> LoginAsync(UserLoginViewModel user);

        public Task<ServiceResult<bool>> LogoutAsync();

        public Task<ServiceResult<TokensResponseViewModel>> RefreshTokenAsync(TokenViewModel token);

        public Task<ServiceResult<TokensResponseViewModel>> GoogleLoginAsync(GoogleLoginViewModel googleUser);
    }
}