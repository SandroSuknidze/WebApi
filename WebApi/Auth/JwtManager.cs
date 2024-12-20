﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Models;
using WebApi.Utils;

namespace WebApi.Auth
{
    public interface IJwtManager
    {
        Token GetToken(User user);
    }
    public class JwtManager(IConfiguration iconfiguration) : IJwtManager
    {
        private readonly IConfiguration iconfiguration = iconfiguration;

        public Token GetToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(iconfiguration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new("UserID", user.Id.ToString(), ClaimValueTypes.Integer),
                    new("FirstName", user.FirstName, ClaimValueTypes.String),
                    new("LastName", user.LastName, ClaimValueTypes.String),
                    new("Email", user.Email, ClaimValueTypes.String),
                    new("PersonalId", user.PersonalId.ToString(), ClaimValueTypes.Integer),
                    new("Role", user.Role.ToString(), ClaimValueTypes.Integer)
                ]),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new Token { AccessToken = tokenHandler.WriteToken(token) };
        }
    }

    public class Token
    {
        public string? AccessToken { get; set; }
    }
}
