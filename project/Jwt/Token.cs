using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebAppTrafficSign.Jwt
{
    public class Token
    {
        private readonly string _secretKey;
        private const int expriedMinutes = 60;
        public Token(string secretKey)
        {
            _secretKey = secretKey;
        }
        public string generateToken(int id,string username, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("id",id.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };
            var token = new JwtSecurityToken(
                issuer: "WebAppTrafficSign",
                audience: "WebAppTrafficSign",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expriedMinutes),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ClaimsPrincipal ExtractClaimsPrincipal(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = "WebAppTrafficSign",
                ValidAudience = "WebAppTrafficSign",
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
            try
            {
                return handler.ValidateToken(token, parameters, out _);
            }
            catch
            {
                return null;
            }

        }
        public string ExtractUsername(string token)
        {
            var principal = ExtractClaimsPrincipal(token);
            return principal?.FindFirst(ClaimTypes.Name)?.Value;
        }

        public string ExtractRole(string token)
        {
            var principal = ExtractClaimsPrincipal(token);
            return principal?.FindFirst(ClaimTypes.Role)?.Value;
        }

        public string ExtractUserId(string token)
        {
            var principal = ExtractClaimsPrincipal(token);
            return principal?.FindFirst("id")?.Value;
        }

        public bool IsTokenExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.ValidTo < DateTime.UtcNow;
        }

        public bool ValidateToken(string token, string expectedUsername)
        {
            var username = ExtractUsername(token);
            return username == expectedUsername && !IsTokenExpired(token);
        }
    }

}
