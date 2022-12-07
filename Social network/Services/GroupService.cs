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
        private readonly IRepository<User> _UserRepository;

        public GroupService(IRepository<Group>  repository, IRepository<User> repository1)
        {
            _GroupRepository = repository;
            _UserRepository = repository1;
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

        public virtual void SetModeratorsToGroup(int groupId, IEnumerable<int> ModeratorIds)
        {
            var group = _GroupRepository.GetById(groupId);
            foreach(var allmoders in group.UserGroup)
            {
                allmoders.IsModerator = false;
            }
            foreach(var user in ModeratorIds)
            {
                group.UserGroup.FirstOrDefault(n => n.UserId == user).IsModerator = true;
            }
            group.UserGroup.FirstOrDefault(n => n.UserId == group.AdminId).IsModerator = true;
            

            _GroupRepository.Update(group);
        }
    }
}
