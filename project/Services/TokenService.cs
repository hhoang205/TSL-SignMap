using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebAppTrafficSign.Models;
using WebAppTrafficSign.Services.Interfaces;
using WebAppTrafficSign.Jwt;

namespace WebAppTrafficSign.Services
{
    public class TokenService : ITokenService
    {
        private Token _tokenHelper;
        public TokenService(IConfiguration configuration)
        {
            string secretKey = configuration["Jwt:Key"]
               ?? "your-secret-key-minimum-32-characters-long!!!";
            _tokenHelper = new Token(secretKey);
        }

        public string GenerateToken(User user)
        {
            return _tokenHelper.generateToken(user.Id,user.Username, user.RoleId.ToString());
        }

        public string GeneratePasswordResetToken(User user)
        {
            return _tokenHelper.generateToken(user.Id,user.Email, "password-reset");
        }

        public bool ValidateToken(string token, string expectedUsername)
        {
            return _tokenHelper.ValidateToken(token, expectedUsername);
        }

        public string ExtractUsername(string token)
        {
            return _tokenHelper.ExtractUsername(token);
        }

        public string ExtractRole(string token)
        {
            return _tokenHelper.ExtractRole(token);
        }

        public string ExtractUserId(string token)
        {
            return _tokenHelper.ExtractUserId(token);
        }

        public bool IsTokenExpired(string token)
        {
            return _tokenHelper.IsTokenExpired(token);
        }
    }
}