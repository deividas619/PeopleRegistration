﻿using System.IdentityModel.Tokens.Jwt;
using PersonRegistration.BusinessLogic.Interfaces;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace PersonRegistration.BusinessLogic.Services
{
    public class JwtService(IConfiguration configuration) : IJwtService
    {
        public string GetJwtToken(string username)
        {
            List<Claim> claims =
            [
                new(ClaimTypes.Name, username)
            ];

            var secretToken = configuration.GetSection("Jwt:Key").Value;
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretToken));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var token = new JwtSecurityToken
            (
                issuer: configuration.GetSection("Jwt:Issuer").Value,
                audience: configuration.GetSection("Jwt:Audience").Value,
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: cred
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
