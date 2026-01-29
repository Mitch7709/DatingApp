using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(AppDbContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]  // POST: api/account/register
    public async Task<ActionResult<UserDTO>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.Email))
        {
            return BadRequest("Email is already taken");
        }

        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            Email = registerDto.Email,
            DisplayName =  registerDto.DisplayName,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user.ToDto(tokenService);
    }

    [HttpPost("login")]  // POST: api/account/login
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDto)
    {
        var user = await context.Users.SingleOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower());

        if (user == null)
        {
            return Unauthorized("Invalid email");
        }

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i])
            {
                return Unauthorized("Invalid password");
            }
        }

        return user.ToDto(tokenService);
    }

    private async Task<bool> UserExists(string email)
    {
        return await context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }
}
