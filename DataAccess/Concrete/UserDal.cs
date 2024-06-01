using DataAccess.Abstract;
using Entities.Concrete;
using Entities.EntityFramework.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
    public class UserDal : IUserDal
    {
        private DataContext _context;
        public UserDal(DataContext context)
        {
            _context = context;
        }

        public void AddFriend(int userId, int friendId)
        {
            var friend = new UserFriend()
            {
                UserId = userId,
                FriendId = friendId
            };
            _context.UserFriends.Add(friend);
            _context.SaveChanges();
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void ApproveRequest(int requesterId, int receiverId)
        {
            var request = _context.FriendRequests.FirstOrDefault(x => x.RequesterId == requesterId && x.ReceiverId == receiverId);
            request.State = 2;

            AddFriend(receiverId, requesterId);
            _context.FriendRequests.Update(request);
            _context.SaveChanges();
        }

        public void DeleteFriend(int userId, int friendId)
        {
            var friendToDelete = _context.UserFriends.FirstOrDefault(f => (f.UserId == userId && f.FriendId == friendId) || (f.UserId == friendId && f.FriendId == userId));

            if (friendToDelete != null)
            {
                _context.UserFriends.Remove(friendToDelete);
                _context.SaveChanges();
            }
        }

        public List<User> GetFriends(int userId)
        {
            var friendIds = _context.UserFriends
        .Where(uf => uf.UserId == userId || uf.FriendId == userId)
        .Select(uf => uf.UserId == userId ? uf.FriendId : uf.UserId)
        .ToList();

            // Arkadaş olan kullanıcıların bilgilerini alıyoruz
            var friends = _context.Users
                .Where(u => friendIds.Contains(u.UserId))
                .ToList();

            return friends;
        }

        public List<User> GetRequests(int userId)
        {
            var requesterIds = _context.FriendRequests
        .Where(f => f.ReceiverId == userId && f.State == 1)
        .Select(f => f.RequesterId)
        .ToList();

            var requesters = _context.Users
                .Where(u => requesterIds.Contains(u.UserId))
                .ToList();
            return requesters;
        }

        public void RejectRequest(int requesterId, int receiverId)
        {
            var request = _context.FriendRequests.FirstOrDefault(x => x.RequesterId == requesterId && x.ReceiverId == receiverId);
            request.State = 3;

            _context.FriendRequests.Update(request);
            _context.SaveChanges();
        }

        public List<User> Search(int userId, string query)
        {
            var users = _context.Users
                .Where(u => u.UserName.Contains(query))
                .ToList();
            var self = _context.Users.FirstOrDefault(u => u.UserId == userId);
            var userFriends = _context.UserFriends
       .Where(uf => uf.UserId == userId || uf.FriendId == userId)
       .ToList();

            // Arkadaş olan kullanıcıların UserId'lerini topluyoruz
            var friendIds = userFriends.Select(uf => uf.UserId == userId ? uf.FriendId : uf.UserId).ToList();

            // Arkadaş olan kullanıcıların Id'lerini search sonuçlarından çıkarıyoruz
            users = users.Where(u => !friendIds.Contains(u.UserId)).ToList();
            users.Remove(self);
            return users;

        }

        public void SendFriendRequest(int requesterId, int receiverId)
        {
            var request = _context.FriendRequests.FirstOrDefault(x => x.RequesterId == requesterId && x.ReceiverId == receiverId);
            if (request == null)
            {
                var newRequest = new FriendRequest()
                {
                    ReceiverId = receiverId,
                    RequesterId = requesterId,
                    State = 1
                };
                _context.FriendRequests.Add(newRequest);
                _context.SaveChanges();
            }
            else
            {
                request.State = 1;
                _context.FriendRequests.Update(request);
                _context.SaveChanges();
            }
        }

        public int ValidateUser(string mail, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Mail == mail);
            if(user == null || user.Password != password) {
                return -1;
            }

            return user.UserId;
        }
    }
}
