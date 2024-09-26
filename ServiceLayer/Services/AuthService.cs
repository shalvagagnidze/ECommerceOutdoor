﻿using DomainLayer.Entities;
using InfrastructureLayer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.Interfaces;
using ServiceLayer.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ServiceLayer.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ECommerceDbContext _db;
    private readonly IConfiguration _config;
    public AuthService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ECommerceDbContext db, IConfiguration config)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
        _config = config;
    }
    public async Task<IActionResult> Registration(UserModel userModel)
    {
        switch (userModel)
        {
            case null:
                throw new Exception("User model couldn't be null!");
            case var u when u.PasswordHash is null:
                throw new Exception("Password couldn't be null!");
        }

        var userExists = _db.Users.FirstOrDefault(x => x.UserName == userModel.UserName);

        if (userExists is not null)
        {
            return new BadRequestObjectResult("User Already Exists");
        }

        var user = new User
        {
            FirstName = userModel.FirstName,
            LastName = userModel.LastName,
            UserName = userModel.UserName
        };

        if (await _roleManager.RoleExistsAsync(userModel.Role!))
        {
            var result = await _userManager.CreateAsync(user, userModel.PasswordHash!);
            var roleResult = await _userManager.AddToRoleAsync(user, userModel.Role!);

            if (result.Succeeded && roleResult.Succeeded)
            {
                await _db.SaveChangesAsync();
                return new OkObjectResult("Registration has been successfully completed");
            }
        }
        else
        {
            return new BadRequestObjectResult("Role doesn't exists  7204");
        }
        

        return new BadRequestObjectResult("Error occured");
    }
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return new NotFoundObjectResult("User not found!");
        }
        var checkPass = await _userManager.CheckPasswordAsync(user, password);

        if (checkPass)
        {
            var tokenString = await GenerateTokenString(user);
            return new OkObjectResult(tokenString);
        }
        return new BadRequestObjectResult("Error occured");
    }

    public async Task<string> GenerateTokenString(IdentityUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email,user.Email!),
            new Claim(ClaimTypes.Name,user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt:Key").Value!));

        var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

        var securityToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMonths(12), //Change this to 2 hours
            issuer: _config.GetSection("Jwt:Issuer").Value,
            audience: _config.GetSection("Jwt:Audience").Value,
            signingCredentials: signingCred);

        string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return tokenString;
    }
}
