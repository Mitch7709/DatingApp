using System;

namespace API.DTOs;

public class CreateMessageDTO
{
    public required string RecipientId { get; set; }
    public required string Content { get; set; }
}
