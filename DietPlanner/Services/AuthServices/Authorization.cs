﻿using System;
using System.Configuration;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Data;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using Services.ViewModels;

namespace Services.AuthServices
{
    public class Authorization
    {
        private readonly Domain.Data.DietContext _context;
        private readonly Configuration _config;

        public Authorization(Domain.Data.DietContext context, Configuration config) 
        {
            _context = context;
            _config = config;
        }

        public static string GetJWTToken(LoginModel login, IConfiguration config, string roleName)
        {
            return GenerateJWT(login, config, roleName);
        }


        private static string GenerateJWT(LoginModel login, IConfiguration config, string roleName)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var Loginclaims = new[]
            {
                new Claim(ClaimTypes.Name,login.Email),
                new Claim(ClaimTypes.Role,roleName)
            };


            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: Loginclaims,
                expires: DateTime.Now.AddHours(1), // Expiry time adjusted to 1 hour
                signingCredentials: credentials
            );



            return new JwtSecurityTokenHandler().WriteToken(token);
        }

       
        
    }
}