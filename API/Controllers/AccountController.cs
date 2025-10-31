using System;
using System.Security.Claims;
using API.DTOs;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(SignInManager<AppUser> signInManager):BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDTO registerDTO)
    {
        var user = new AppUser
        {
            FirstName = registerDTO.FirstName,
            LastName = registerDTO.LastName,
            Email = registerDTO.Email,
            UserName = registerDTO.Email
        };
        var result = await signInManager.UserManager.CreateAsync(user, registerDTO.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);
        return Ok();
    }
    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return NoContent();
    }
    [HttpGet("user-info")]
    public async Task<ActionResult> GetUserInfo()
    {
        if (User.Identity?.IsAuthenticated == false) return NoContent();
        var user = await signInManager.UserManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
        if (user == null) return Unauthorized();
        return Ok(new
        {
            user.FirstName,
            user.LastName,
            user.Email
        });
    }
    [HttpGet]
    public ActionResult GetAuthState()
    {
        return Ok(new { IsAuthenticated = User.Identity?.IsAuthenticated ?? false });
    }
}
