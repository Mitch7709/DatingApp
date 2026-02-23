using System;

namespace API.DTOs;

public class MessageDTO
{
    
    public required string Id { get; set; }
    public required string SenderId { get; set; }
    public required string SenderDisplayName { get; set; }
    public string? SenderPhotoUrl { get; set; }
    public required string RecipientId { get; set; }
    public required string RecipientDisplayName { get; set; }
    public string? RecipientPhotoUrl { get; set; }
    public required string Content { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime MessageSent { get; set; }
}
