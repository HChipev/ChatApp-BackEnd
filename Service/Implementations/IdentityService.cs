using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Common.Classes;
using Data.Entities;
using Data.Repository;
using Data.ViewModels.BasicResponseModels;
using Data.ViewModels.Identity.Models;
using Data.ViewModels.Token.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Service.Interfaces;

namespace Service.Implementations
{
    public class IdentityService : IIdentityService
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly IRepository<Role> _rolesRepository;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserRole> _userRolesRepository;


        public IdentityService(IRepository<User> userRepository, UserManager<User> userManager,
            SignInManager<User> signInManager,
            ITokenService tokenService, IConfiguration configuration, HttpClient httpClient, IMapper mapper,
            IRepository<UserRole> userRolesRepository, IRepository<Role> rolesRepository,
            IHostingEnvironment environment)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _configuration = configuration;
            _httpClient = httpClient;
            _mapper = mapper;
            _userRolesRepository = userRolesRepository;
            _rolesRepository = rolesRepository;
            _environment = environment;
        }

        public async Task<ServiceResult<BasicResponseViewModel>> LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return new ServiceResult<BasicResponseViewModel>
                {
                    IsSuccess = true, Message = "",
                    Data = new BasicResponseViewModel { Message = "Successfully logged out." }
                };
            }
            catch (Exception e)
            {
                return new ServiceResult<BasicResponseViewModel>
                    { IsSuccess = false, Message = e.Message, Data = null };
            }
        }

        public async Task<ServiceResult<TokensResponseViewModel>> GoogleLoginAsync(GoogleLoginViewModel googleUser)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", googleUser.GoogleId);

            var response = await _httpClient.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");

            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResult<TokensResponseViewModel>
                    { IsSuccess = false, Message = "Error getting info from Google!", Data = null };
            }

            var content = await response.Content.ReadAsStringAsync();
            var userInfo = JsonConvert.DeserializeObject<GoogleUserInfoViewModel>(content);

            var dbUser = _userRepository.FindByCondition(user => user.Email == userInfo.Email);
            if (dbUser is not null)
            {
                await _signInManager.SignInAsync(dbUser, true);


                return new ServiceResult<TokensResponseViewModel>
                {
                    IsSuccess = true, Message = "",
                    Data = new TokensResponseViewModel
                    {
                        Tokens = _tokenService.GenerateAccessToken(dbUser.Email, dbUser.Id,
                            await GetUserRolesNames(dbUser.Id), dbUser.Picture, dbUser.Name, true)
                    }
                };
            }

            var newUser = _mapper.Map<UserRegistrationViewModel>(userInfo);
            newUser.Password = _environment.IsProduction()
                ? Environment.GetEnvironmentVariable("GOOGLE_DEFAULT_PASSWORD")
                : _configuration["Google:DefaultPassword"];

            var result = await RegisterAsync(newUser);

            return result.IsSuccess
                ? await LoginAsync(result.Data)
                : new ServiceResult<TokensResponseViewModel>
                    { IsSuccess = false, Message = result.Message, Data = null };
        }

        public async Task<ServiceResult<TokensResponseViewModel>> RefreshTokenAsync(TokenViewModel token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var principal = tokenHandler.ValidateToken(token.Token, new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET") ??
                                            _configuration["JWT:Key"])),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true
                }, out var securityToken);
                var validatedToken = securityToken as JwtSecurityToken;

                if (validatedToken?.Header.Alg != SecurityAlgorithms.HmacSha256)
                {
                    return new ServiceResult<TokensResponseViewModel>
                        { IsSuccess = false, Message = "Invalid algorithm", Data = null };
                }

                var nameIdentifier = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier).Value);

                var user = _userRepository.FindByCondition(u =>
                    u.Id == nameIdentifier);

                if (user is null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                {
                    return new ServiceResult<TokensResponseViewModel>
                        { IsSuccess = false, Message = "Invalid token", Data = null };
                }

                var newTokens =
                    _tokenService.GenerateAccessToken(user.Email, user.Id, await GetUserRolesNames(user.Id),
                        user.Picture, user.Name);
                return new ServiceResult<TokensResponseViewModel>
                    { IsSuccess = true, Message = "", Data = new TokensResponseViewModel { Tokens = newTokens } };
            }
            catch (Exception ex)
            {
                return new ServiceResult<TokensResponseViewModel>
                    { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        private async Task<ServiceResult<TokensResponseViewModel>> LoginAsync(UserLoginViewModel user)
        {
            try
            {
                var dbUser = await _userManager.FindByEmailAsync(user.Email);

                if (dbUser is null)
                {
                    return new ServiceResult<TokensResponseViewModel>
                        { IsSuccess = false, Message = "Invalid email or password", Data = null };
                }

                var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, false, false);

                if (!result.Succeeded)
                {
                    return new ServiceResult<TokensResponseViewModel>
                        { IsSuccess = false, Message = "Invalid email or password", Data = null };
                }

                return new ServiceResult<TokensResponseViewModel>
                {
                    IsSuccess = true, Message = "",
                    Data = new TokensResponseViewModel
                    {
                        Tokens = _tokenService.GenerateAccessToken(dbUser.Email, dbUser.Id,
                            await GetUserRolesNames(dbUser.Id), dbUser.Picture, dbUser.Name, true)
                    }
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<TokensResponseViewModel>
                    { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        private async Task<ServiceResult<UserLoginViewModel>> RegisterAsync(UserRegistrationViewModel newUser)
        {
            var user = _mapper.Map<User>(newUser);

            try
            {
                var result = await _userManager.CreateAsync(user, newUser.Password);

                if (!result.Succeeded)
                {
                    return new ServiceResult<UserLoginViewModel>
                    {
                        IsSuccess = false, Message = string.Join("\n", result.Errors.Select(err => err.Description)),
                        Data = null
                    };
                }

                await _userManager.AddToRoleAsync(user, Roles.User);

                return new ServiceResult<UserLoginViewModel>
                {
                    IsSuccess = true, Message = "",
                    Data = new UserLoginViewModel { Email = newUser.Email, Password = newUser.Password }
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<UserLoginViewModel>
                    { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        private async Task<IEnumerable<string>> GetUserRolesNames(int id)
        {
            var userRoles = await _userRolesRepository
                .FindAllByCondition(userRole => userRole.UserId == id).ToListAsync();

            var roles = await _rolesRepository
                .FindAllByCondition(role => userRoles.Select(userRole => userRole.RoleId).Contains(role.Id))
                .ToListAsync();

            return roles.Select(role => role.Name);
        }
    }
}