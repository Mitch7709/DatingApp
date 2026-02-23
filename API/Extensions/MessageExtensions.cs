using System;
using System.Linq.Expressions;
using API.DTOs;
using API.Entities;

namespace API.Extensions;

public static class MessageExtensions
{
    public static MessageDTO ToDto(this Message message)
    {
        return new MessageDTO
        {
            Id = message.Id,
            SenderId = message.SenderId,
            SenderDisplayName = message.Sender.DisplayName,
            SenderPhotoUrl = message.Sender.ImageUrl,
            RecipientId = message.RecipientId,
            RecipientDisplayName = message.Recipient.DisplayName,
            RecipientPhotoUrl = message.Recipient.ImageUrl,
            Content = message.Content,
            DateRead = message.DateRead,
            MessageSent = message.MessageSent
        };
    }

    public static Expression<Func<Message, MessageDTO>> ToMessageDtoExpression()
    {
        return message => new MessageDTO
        {
            Id = message.Id,
            SenderId = message.SenderId,
            SenderDisplayName = message.Sender.DisplayName,
            SenderPhotoUrl = message.Sender.ImageUrl,
            RecipientId = message.RecipientId,
            RecipientDisplayName = message.Recipient.DisplayName,
            RecipientPhotoUrl = message.Recipient.ImageUrl,
            Content = message.Content,
            DateRead = message.DateRead,
            MessageSent = message.MessageSent
        };
    }
}
