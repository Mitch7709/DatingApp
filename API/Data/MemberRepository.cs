using System;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MemberRepository(AppDbContext context) : IMemberRepository
{
    public async Task<Member?> GetMemberByIdAsync(string id)
    {
        var member = await context.Members
            .Include(m => m.Photos)
            .FirstOrDefaultAsync(m => m.Id == id);
        
        if (member != null && member.Photos.Count > 0)
        {
            member.ImageUrl = member.Photos.First().Url;
        }
        
        return member;
    }

    public async Task<Member?> GetMemberForUpdateAsync(string id)
    {
        return await context.Members
            .Include(x => x.User)
            .SingleOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IReadOnlyList<Member>> GetMembersAsync()
    {
        var members = await context.Members
            .Include(m => m.Photos)
            .ToListAsync();
        
        foreach (var member in members)
        {
            if (member.Photos.Count > 0)
            {
                member.ImageUrl = member.Photos.First().Url;
            }
        }
        
        return members;
    }

    public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId)
    {
        return await context.Photos
            .Where(photo => photo.MemberId == memberId)
            .ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void Update(Member member)
    {
        context.Entry(member).State = EntityState.Modified;
    }
    
}
