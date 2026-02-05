using System;

namespace API.DTOs;

public class MemberUpdateDTO
{
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
}
