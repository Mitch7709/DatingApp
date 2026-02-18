using System;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController(ILikesRepository likesRepository) : BaseApiController
{
    [HttpPost("{targetMemberId}")]
    public async Task<ActionResult> ToggleLike(string targetMemberId)
    {
        var sourceMemberId = User.GetMemberId();

        if (sourceMemberId == targetMemberId)
        {
            return BadRequest("You cannot like yourself.");
        }

        var existingLike = await likesRepository.GetMemberLike(sourceMemberId, targetMemberId);
        if (existingLike != null)
        {
            likesRepository.DeleteLike(existingLike);
        }
        else
        {
            var newLike = new MemberLike
            {
                SourceMemberId = sourceMemberId,
                TargetMemberId = targetMemberId
            };
            likesRepository.AddLike(newLike);;
        }

        if (await likesRepository.SaveAllChanges())
        {
            return Ok();
        }

        return BadRequest("Failed to update like.");
    }

    [HttpGet("list")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetCurrentMemberLikeIds()
    {
        return Ok(await likesRepository.GetCurrentMemberLikeIds(User.GetMemberId()));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Member>>> GetMemberLikes([FromQuery] LikesParams likesParams)
    {
        likesParams.MemberId = User.GetMemberId();
        var members = await likesRepository.GetMemberLikes(likesParams);
        return Ok(members);
    }
}
