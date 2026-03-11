using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(UserManager<AppUser> userManager)
    {
        if (await userManager.Users.AnyAsync()) return;

        var memberData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var members = JsonSerializer.Deserialize<List<SeedUserDTO>>(memberData);

        if (members is null)
        {
            Console.WriteLine("No members to seed.");
            return;
        }

        

        foreach (var memberDto in members)
        {
            // Create AppUser
            var appUser = new AppUser
            {
                Id = memberDto.Id,
                DisplayName = memberDto.DisplayName,
                Email = memberDto.Email,
                UserName = memberDto.Email,
                ImageUrl = memberDto.ImageUrl
            };

            // Create Member
            var member = new Member
            {
                Id = appUser.Id,
                DateOfBirth = memberDto.DateOfBirth,
                DisplayName = memberDto.DisplayName,
                Created = memberDto.Created,
                LastActive = memberDto.LastActive,
                Gender = memberDto.Gender,
                Description = memberDto.Description,
                City = memberDto.City,
                ImageUrl = memberDto.ImageUrl,
                Country = memberDto.Country
            };

            appUser.Member = member;
            appUser.Member.Photos.Add(new Photo
            {
                Url = memberDto.ImageUrl!,
                MemberId = member.Id,
            });

            var result = await userManager.CreateAsync(appUser, "Pa$$w0rd");
            if (!result.Succeeded)
            {
                Console.WriteLine(result.Errors.First().Description);
            }
            await userManager.AddToRoleAsync(appUser, "Member");
        }

        var admin = new AppUser
        {
            DisplayName = "Admin",
            Email = "admin@example.com",
            UserName = "admin@example.com"
        };

        await userManager.CreateAsync(admin, "Pa$$w0rd");
        await userManager.AddToRolesAsync(admin, ["Admin", "Moderator"]);
    }
}
