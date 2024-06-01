using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        private IUserDal _userDal; 
        public UserManager(IUserDal userDal)
        {
            _userDal = userDal;
        }

        public void addFriend(int userId, int friendId)
        {
            _userDal.AddFriend(userId, friendId);
        }

        public void AddUser(User user)
        {
            _userDal.AddUser(user);
        }

        public void ApproveRequest(int requesterId, int receiverId)
        {
            _userDal.ApproveRequest(requesterId, receiverId);
        }

        public void DeleteFriend(int userId, int friendId)
        {
            _userDal.DeleteFriend(userId, friendId);
        }

        public List<User> GetFriends(int userId)
        {
            return _userDal.GetFriends(userId);
        }

        public List<User> GetRequests(int userId)
        {
            return _userDal.GetRequests(userId);
        }

        public void RejectRequest(int requesterId, int receiverId)
        {
            _userDal.RejectRequest(requesterId, receiverId);
        }

        public List<User> Search(int userId,string query)
        {
            return _userDal.Search(userId,query);
        }


        public void SendFriendRequest(int requesterId, int receiverId)
        {
            _userDal.SendFriendRequest(requesterId, receiverId);
        }

        public int ValidateUser(string username, string password)
        {
            return _userDal.ValidateUser(username, password);
        }
    }
}
