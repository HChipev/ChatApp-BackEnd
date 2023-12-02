using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Data.Entities;
using Data.Repository;
using Data.ViewModels.Token.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;

namespace Service.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<User> _repository;

        public TokenService(IRepository<User> repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public TokenViewModel GenerateAccessToken(string email, int id, IEnumerable<string> roleNames,
            string picture, string name, bool isLogin = false)
        {
            int.TryParse(_configuration["JWT:TokenValidityInDays"], out var tokenValidityInDays);
            var expiration = DateTime.UtcNow.AddDays(tokenValidityInDays);

            var accessToken = CreateJwtToken(
                CreateClaims(email, id, roleNames, picture, name),
                CreateSigningCredentials(),
                expiration
            );

            int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out var refreshTokenValidityInDays);
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(refreshTokenValidityInDays);

            var refreshToken = CreateRefreshTokenJwt(
                CreateSigningCredentials(),
                refreshTokenExpiration
            );

            var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);
            var refreshTokenString = new JwtSecurityTokenHandler().WriteToken(refreshToken);

            var dbUser = _repository.Find(id);

            if (dbUser is not null && (dbUser.RefreshTokenExpiryTime < DateTime.UtcNow || dbUser.RefreshToken is null ||
                                       isLogin))
            {
                dbUser.RefreshToken = refreshTokenString;
                dbUser.RefreshTokenExpiryTime = refreshTokenExpiration;
            }

            _repository.Update(dbUser);
            _repository.SaveChanges();

            return new TokenViewModel
            {
                Token = accessTokenString
            };
        }

        private JwtSecurityToken CreateJwtToken(Claim[] claims, SigningCredentials credentials, DateTime expiration)
        {
            return new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expiration,
                signingCredentials: credentials
            );
        }

        private JwtSecurityToken CreateRefreshTokenJwt(SigningCredentials credentials, DateTime expiration)
        {
            return new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                expires: expiration,
                signingCredentials: credentials
            );
        }

        private Claim[] CreateClaims(string email, int id, IEnumerable<string> roleNames, string picture, string name)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, name),
                new(ClaimTypes.Email, email),
                new(ClaimTypes.NameIdentifier, id.ToString()),
                new("picture", picture)
            };
            claims.AddRange(roleNames.Select(roleName => new Claim(ClaimTypes.Role, roleName)));

            return claims.ToArray();
        }

        private SigningCredentials CreateSigningCredentials()
        {
            return new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        Environment.GetEnvironmentVariable("JWT_SECRET") ?? _configuration["JWT:Key"])
                ),
                SecurityAlgorithms.HmacSha256
            );
        }
    }
}