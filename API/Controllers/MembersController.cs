using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]  
    public class MembersController(IMemberRepository memberRepo, IPhotoService photoService) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
        {
            var members = await memberRepo.GetMembersAsync();
            return Ok(members);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(string id)
        {
            var member = await memberRepo.GetMemberByIdAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return Ok(member);
        }

        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetPhotosForMember(string id)
        {
            var photos = await memberRepo.GetPhotosForMemberAsync(id);
            return Ok(photos);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateMember(MemberUpdateDTO memberUpdateDto)
        {
            var memberId = User.GetMemberId();

            var member = await memberRepo.GetMemberForUpdateAsync(memberId);
            if (member == null)
            {
                return BadRequest("Member not found");
            }

            member.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;
            member.Description = memberUpdateDto.Description ?? member.Description;
            member.City = memberUpdateDto.City ?? member.City;
            member.Country = memberUpdateDto.Country ?? member.Country;

            member.User.DisplayName = memberUpdateDto.DisplayName ?? member.User.DisplayName;

            memberRepo.Update(member);
            if (await memberRepo.SaveAllAsync())
            {
                return NoContent();
            }
            return BadRequest("Failed to update member");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<Photo>> AddPhoto([FromForm] IFormFile file)
        {
            var member = await memberRepo.GetMemberByIdAsync(User.GetMemberId());
            if (member == null)
            {
                return BadRequest("Member not found");
            }

            var result = await photoService.AddPhotoAsync(file);
            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                MemberId = User.GetMemberId()
            };
            
            if (member.ImageUrl == null)
            {
                member.ImageUrl = photo.Url;
                member.User.ImageUrl = photo.Url;
            }

            member.Photos.Add(photo);
            if (await memberRepo.SaveAllAsync())
            {
                return photo;
            }
            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var member = await memberRepo.GetMemberForUpdateAsync(User.GetMemberId());
            if (member == null)
            {
                return BadRequest("Member not found");
            }

            var photo = member.Photos.SingleOrDefault(p => p.Id == photoId);
            
            if (member.ImageUrl == photo?.Url || photo == null)
            {
                return BadRequest("Photo not found or already main");
            }

            member.ImageUrl = photo.Url;
            member.User.ImageUrl = photo.Url;
            if (await memberRepo.SaveAllAsync())
            {
                return NoContent();
            }
            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var member = await memberRepo.GetMemberForUpdateAsync(User.GetMemberId());
            if (member == null)
            {
                return BadRequest("Member not found");
            }

            var photo = member.Photos.SingleOrDefault(p => p.Id == photoId);
            if (photo == null || member.ImageUrl == photo.Url)
            {
                return BadRequest("Photo cannot be deleted");
            }            

            if (photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null)
                {
                    return BadRequest(result.Error.Message);
                }
            }            

            member.Photos.Remove(photo);
            if (await memberRepo.SaveAllAsync())
            {
                return Ok();
            }
            return BadRequest("Failed to delete photo");
        }
    }
}
