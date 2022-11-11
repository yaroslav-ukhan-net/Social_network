using Models;
using Models.Models;
using System;
using System.Collections.Generic;

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
        public virtual List<User> GetAllUsers()
        {
            return _UserRepository.GetAll();
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
            var myuser = _UserRepository.GetById(userId);

            if(myuser == null)
            {
                throw new ArgumentException($"There is no user with id {userId}");
            }

            var UserGroups = myuser.UserGroup;
            foreach(var group in UserGroups)
            {
                if (group.GroupId == groupId)
                {
                    myuser.UserGroup.Remove(group);
                    break;
                }
            }
            _UserRepository.Update(myuser);
        }
    }
}
