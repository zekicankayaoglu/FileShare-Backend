using Business.Abstract;
using Entities.Concrete;
using FileTransfer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FileTransfer.Controllers
{
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            int userId = _userService.ValidateUser(model.Mail, model.Password);
            if (userId == -1)
            {
                return Unauthorized();
            }

            return Ok(userId);
        }

        [HttpPost("Sign")]
        public async Task<IActionResult> Sign([FromBody] SignModel model)
        {
            var user = new User()
            {
                UserName = model.UserName,
                Mail = model.Mail,
                Password = model.Password
            };
            _userService.AddUser(user);

            return Ok();
        }

        [HttpGet("GetFriends/{userId}")]
        public async Task<IActionResult> GetFriends(int userId)
        {
            return Ok(_userService.GetFriends(userId));
        }

        [HttpPost("AddFriend/{userId}")]
        public async Task<IActionResult> AddFriend(int userId, int friendId)
        {
            _userService.addFriend(userId, friendId);
            return Ok();
        }

        [HttpGet("GetRequests/{userId}")]
        public async Task<IActionResult> GetRequests(int userId)
        {
            return Ok(_userService.GetRequests(userId));
        }

        [HttpPost("FriendRequest/{userId}")]
        public async Task<IActionResult> FriendRequest(int userId, [FromQuery] int receiverId)
        {
            _userService.SendFriendRequest(userId, receiverId);
            return Ok();
        }

        [HttpPost("ApproveRequest/{userId}")]
        public async Task<IActionResult> ApproveRequest(int userId, [FromQuery] int receiverId)
        {
            _userService.ApproveRequest(receiverId, userId);
            return Ok();
        }

        [HttpPost("RejectRequest/{userId}")]
        public async Task<IActionResult> RejectRequest(int userId, [FromQuery] int receiverId)
        {
            _userService.RejectRequest(receiverId, userId);
            return Ok();
        }

        [HttpGet("searchUsers/{userId}")]
        public async Task<IActionResult> SearchUsers(int userId, [FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                query = "";
            }

            
            return Ok(_userService.Search(userId,query));
        }

        [HttpDelete("DeleteFriend/{userId}")]
        public async Task<IActionResult> DeleteFriend(int userId, [FromQuery] int friendId)
        {
            _userService.DeleteFriend(userId, friendId);
            return Ok();
        }
    }
}
