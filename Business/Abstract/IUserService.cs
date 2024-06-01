using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IUserService
    {
        int ValidateUser(string username, string password);
        List<User> GetFriends(int userId);
        void addFriend(int userId, int friendId);
        void SendFriendRequest(int requesterId, int receiverId);
        void ApproveRequest(int requesterId, int receiverId);
        void RejectRequest(int requesterId, int receiverId);
        List<User> GetRequests(int userId);
        List<User> Search(int userId,string query);
        void DeleteFriend(int userId, int friendId);
        void AddUser(User user);
    }
}
