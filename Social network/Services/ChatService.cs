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
    public class ChatService
    {
        private readonly IRepository<Chat> _ChatRepository;

        public ChatService(IRepository<Chat> repository)
        {
            _ChatRepository = repository;
        }
        public virtual IQueryable<Chat> GetChats(Expression<Func<Chat, bool>> expression)
        {
            return _ChatRepository.GetAllQuerible(expression);
        }
        public virtual void CreateChat(Chat chat)
        {
            _ChatRepository.Create(chat);
        }
        public virtual void DeleteChat(int id)
        {
            _ChatRepository.Remove(id);
        }
        public virtual void UpdateChat(Chat chat)
        {
            _ChatRepository.Update(chat);
        }
        public virtual Chat GetChatById(int id)
        {
            return _ChatRepository.GetById(id);
        }
    }
}
