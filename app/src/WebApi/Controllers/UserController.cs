using Application.Dtos.Users;
using Application.Interfaces.Users;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public class UserController : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Login(
        [FromServices] ICreateUser useCase,
        [FromBody] CreateUserDto request
    )
    {
        var result = await useCase.ExecuteAsync(request);
        return Ok(result);
    }

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById(
        [FromServices] IGetUserById useCase,
        [FromQuery] GetUserByIdDto request
    )
    {
        var result = await useCase.ExecuteAsync(request);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> List(
        [FromServices] IListUsers useCase,
        [FromQuery] ListUsersDto request
    )
    {
        var result = await useCase.ExecuteAsync(request);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(
        [FromServices] IUpdateUser useCase,
        [FromBody] UpdateUserDto request
    )
    {
        var result = await useCase.ExecuteAsync(request);
        return Ok(result);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(
        [FromServices] IDeleteUser useCase,
        [FromBody] DeleteUserDto request
    )
    {
        var success = await useCase.ExecuteAsync(request);
        return Ok(success);
    }
}