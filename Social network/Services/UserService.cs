using Models;
using System;
using System.Collections.Generic;

namespace Services
{
    public class UserService
    {
        private readonly IRepository<User> _UserRepository;

        public UserService(IRepository<User> repository)
        {
            _UserRepository = repository;
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
    }
}
