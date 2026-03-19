using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MessageRepository (AppDbContext context) : IMessageRepository
{
    public void AddGroup(Group group)
    {
        context.Groups.Add(group);
    }

    public void AddMessage(Message message)
    {
        context.Messages.AddAsync(message);
    }

    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }

    public async Task<Connection?> GetConnection(string connectionId)
    {
        return await context.Connections.FindAsync(connectionId);
    }

    public async Task<Group?> GetGroupForConnection(string connectionId)
    {
        return await context.Groups
            .Include(g => g.Connections)
            .Where(g => g.Connections.Any(c => c.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
    }

    public async Task<Message?> GetMessage(string id)
    {
        return await context.Messages.FindAsync(id);
    }

    public async Task<Group?> GetMessageGroup(string groupName)
    {
        return await context.Groups
            .Include(g => g.Connections)
            .FirstOrDefaultAsync(g => g.Name == groupName);
    }

    public async Task<PaginatedResults<MessageDTO>> GetMessagesForMember(MessageParams messageParams)
    {
        var query = context.Messages
            .OrderByDescending(m => m.MessageSent)
            .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(u => u.RecipientId == messageParams.MemberId && !u.RecipientDeleted),
            "Outbox" => query.Where(u => u.SenderId == messageParams.MemberId && !u.SenderDeleted),
            _ => query.Where(u => u.RecipientId == messageParams.MemberId && !u.RecipientDeleted)
        };

        var messageQuery = query.Select(MessageExtensions.ToMessageDtoExpression());

        return await PaginationHelper.CreateAsync(messageQuery, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IReadOnlyList<MessageDTO>> GetMessageThread(string currentMemberId, string recipientId)
    {
        await context.Messages
            .Where(x => x.RecipientId == currentMemberId && x.SenderId == recipientId && x.DateRead == null)                
            .ExecuteUpdateAsync(setters => setters.SetProperty(m => m.DateRead, DateTime.UtcNow));
        
        var messages = await context.Messages
            .Where(m => (m.RecipientId == currentMemberId && !m.RecipientDeleted && m.SenderId == recipientId) ||
            (m.SenderId == currentMemberId && !m.SenderDeleted && m.RecipientId == recipientId))
            .OrderBy(m => m.MessageSent)
            .Select(MessageExtensions.ToMessageDtoExpression())
            .ToListAsync();

        return messages;
    }

    public async Task RemoveConnection(string connectionId)
    {
        await context.Connections.Where(c => c.ConnectionId == connectionId).ExecuteDeleteAsync();
    }
}
