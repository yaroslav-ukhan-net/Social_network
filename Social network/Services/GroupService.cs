using Models;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class GroupService
    {
        private readonly IRepository<Group> _GroupRepository;
        public GroupService(IRepository<Group>  repository)
        {
            _GroupRepository = repository;
        }
        public virtual List<Group> GetAllGroups()
        {
           return _GroupRepository.GetAll();
        }
        public virtual Group GetGroupById(int id)
        {
            return _GroupRepository.GetById(id);
        }
        public virtual void UpdateGroup(Group group)
        {
            _GroupRepository.Update(group);
        }
        public virtual void RemoveGroupById(int id)
        {
            _GroupRepository.Remove(id);
        }
        public virtual void CreateGroup(Group group)
        {
            _GroupRepository.Create(group);
        }
    }
}
