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
    public class MessageService
    {
        private readonly IRepository<Message> _MessageRepository;

        public MessageService(IRepository<Message> repository)
        {
            _MessageRepository = repository;
        }
        public virtual IQueryable<Message> GetMessages(Expression<Func<Message, bool>> expression)
        {
            return _MessageRepository.GetAllQuerible(expression);
        }
        public virtual Message GetMessageById(int id)
        {
            return _MessageRepository.GetById(id);
        }
        public virtual void CreateMessage(Message message)
        {
            _MessageRepository.Create(message);
        }
        public virtual void UpdateMessage(Message message)
        {
            _MessageRepository.Update(message);
        }
        public virtual void RemoveMessageById(int id)
        {
            _MessageRepository.Remove(id);
        }
    }
}
