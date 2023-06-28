﻿using Microsoft.AspNetCore.Mvc;
using WildShape_Sheets_API.Models;
using WildShape_Sheets_API.Services;
using Microsoft.AspNetCore.Authorization;

namespace WildShape_Sheets_API.Controllers {

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller {

        private readonly UserService userService;

        public UserController(UserService _userService) =>
            userService = _userService;

        [HttpGet]
        public ActionResult<List<User>> GetUsers() {
            return userService.GetUsers();
        }

        [HttpGet("{id:length(24)}")]
        public ActionResult<User> GetUser(string id) {
            var user = userService.GetUser(id);
            return Json(user);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<User> CreateUser(User user) {
            userService.CreateUser(user);
            return Json(user);
        }

        [HttpDelete("{id:length(24)}")]
        public void DeleteUser(string id) {
            userService.DeleteUser(id);
        }

        //[AllowAnonymous]
        //[Route("authenticate")]
        //[HttpPost]
        //public ActionResult Login([FromBody] User user) {
            
        //    var token = userService.Authenticate(user.Email, user.Password);

        //    if (token == null)
        //        return Unauthorized();

        //    return Ok(new { token, user });
        //}

        //[HttpGet]
        //public async Task<List<User>> Get() =>
        //    await _userService.GetAsync();

        //    [HttpPost]
        //    public async Task<IActionResult> Post(User newUser) {
        //        await _userService.CreateAsync(newUser);

        //        return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
        //    }

        //    [HttpPut("{id}")]
        //    public async Task<IActionResult> Update(string id, User updatedUser) {
        //        var user = await _userService.GetAsync(id);

        //        if (user is null) {
        //            return NotFound();
        //        }

        //        updatedUser.Id = user.Id;

        //        await _userService.UpdateAsync(id, updatedUser);

        //        return NoContent();
        //    }

        //    [HttpDelete("{id}")]
        //    public async Task<IActionResult> Delete(string id) {
        //        var user = await _userService.GetAsync(id);

        //        if (user is null) {
        //            return NotFound();
        //        }

        //        await _userService.RemoveAsync(id);

        //        return NoContent();
        //    }
        }
    }
