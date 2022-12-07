using Models;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class FriendService
    {
        private readonly IRepository<Friend> _FriendRepository;

        public FriendService(IRepository<Friend> repository)
        {
            _FriendRepository = repository;
        }
        public virtual IQueryable<Friend> GetAllFriends()
        {
            return _FriendRepository.GetAll();
        }
        public virtual Friend GetFriendsByTwoId(int FriendId1, int FriendId2)
        {
            return _FriendRepository.GetByTwoId(FriendId1, FriendId2);
        }
        public virtual void UpdateFriend(Friend friend)
        {
            _FriendRepository.Update(friend);
        }
        public virtual void DeleteFriend(int id)
        {
            _FriendRepository.Remove(id);
        }
        public virtual void DeleteFriendentity(Friend friend)
        {
            _FriendRepository.RemoveEntity(friend);
        }
        public virtual void CreateFriend(Friend friend)
        {
            _FriendRepository.Create(friend);
        }
        public virtual IQueryable<Friend> GetAllFriendsQuerible(Expression<Func<Friend, bool>> expression)
        {
            return _FriendRepository.GetAllQuerible(expression);
        }
    }
}
