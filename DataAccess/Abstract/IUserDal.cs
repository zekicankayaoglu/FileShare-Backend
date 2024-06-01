using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface IUserDal
    {
        int ValidateUser(string mail, string password);
        List<User> GetFriends(int userId);
        void AddFriend(int userId, int friendId);
        void SendFriendRequest(int requesterId, int receiverId);
        void ApproveRequest(int requesterId, int receiverId);
        void RejectRequest(int requesterId, int receiverId);
        List<User> GetRequests(int userId);
        List<User> Search(int userId, string query);
        void DeleteFriend(int userId, int friendId);
        void AddUser(User user);
    }
}
