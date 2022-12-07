using Models;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Services
{
    public class UserService
    {
        private readonly IRepository<User> _UserRepository;
        private readonly IRepository<Group> _GroupRepository;

        public UserService(IRepository<User> repository, IRepository<Group> repository1)
        {
            _UserRepository = repository;
            _GroupRepository = repository1;
        }
        public virtual IQueryable<User> GetAllUsers()
        {
            return _UserRepository.GetAll();
        }
        public virtual IQueryable<User> GetAllUsersQuerible(Expression<Func<User, bool>> expression)
        {
            return _UserRepository.GetAllQuerible(expression);
        }
        public virtual User GetUserById(int UserId)
        {
            return _UserRepository.GetById(UserId);
        }
        public virtual void UpdateUser(User user)
        {
            _UserRepository.Update(user);
        }
        public virtual void DeleteUser(int id)
        {
            _UserRepository.Remove(id);
        }
        public virtual void CreateUser(User user)
        {
            _UserRepository.Create(user);
        }
        public virtual void UserUnsubscribe(int userId, int groupId)
        {
            var myUser = _UserRepository.GetById(userId);

            if (myUser == null)
            {
                throw new ArgumentException($"There is no user with id {userId}");
            }

            var UserGroups = myUser.UserGroup;
            foreach (var group in UserGroups)
            {
                if (group.GroupId == groupId)
                {
                    myUser.UserGroup.Remove(group);
                    if (group.ConsistInGroup)
                    {
                        var editedGroup = _GroupRepository.GetById(groupId);
                        _GroupRepository.Update(editedGroup);
                    }
                    break;
                }
            }
            _UserRepository.Update(myUser);
            
        }
    }
}
