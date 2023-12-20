using Common.Classes;
using Data.ViewModels.BasicResponseModels;
using Data.ViewModels.Identity.Models;
using Data.ViewModels.Token.Models;
using Service.Abstract;

namespace Service.Interfaces
{
    public interface IIdentityService : IService
    {
        public Task<ServiceResult<BasicResponseViewModel>> LogoutAsync();

        public Task<ServiceResult<TokensResponseViewModel>> RefreshTokenAsync(TokenViewModel token);

        public Task<ServiceResult<TokensResponseViewModel>> GoogleLoginAsync(GoogleLoginViewModel googleUser);
    }
}