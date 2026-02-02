using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.DTOs;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(AppDbContext context)
    {
        if (await context.Users.AnyAsync()) return;

        var memberData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var members = JsonSerializer.Deserialize<List<SeedUserDTO>>(memberData);

        if (members is null)
        {
            Console.WriteLine("No members to seed.");
            return;
        }

        

        foreach (var memberDto in members)
        {
            using var hmac = new HMACSHA512();

            // Create AppUser
            var appUser = new AppUser
            {
                Id = memberDto.Id,
                DisplayName = memberDto.DisplayName,
                Email = memberDto.Email,
                ImageUrl = memberDto.ImageUrl,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
                PasswordSalt = hmac.Key
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
                Country = memberDto.Country
            };

            appUser.Member = member;
            appUser.Member.Photos.Add(new Photo
            {
                Url = memberDto.ImageUrl!,
                MemberId = member.Id,
            });

            context.Users.Add(appUser);
            context.Members.Add(member);
        }

        await context.SaveChangesAsync();
    }
}
