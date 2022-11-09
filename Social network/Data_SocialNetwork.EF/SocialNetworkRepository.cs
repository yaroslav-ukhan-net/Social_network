using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_SocialNetwork.EF
{
    public class SocialNetworkRepository<T> : IRepository<T> where T : class
    {
        private readonly SocialNetworkContext _context;
        protected DbSet<T> DbSet;

        public SocialNetworkRepository(SocialNetworkContext socialNetworkContext)
        {
            _context = socialNetworkContext;
            socialNetworkContext.Database.EnsureCreated();
            DbSet = _context.Set<T>();
        }
        public T Create(T entity)
        {
            var result = DbSet.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public List<T> GetAll()
        {
            return DbSet.ToList();
        }

        public T GetById(int id)
        {
            return DbSet.Find(id);
        }
        public T GetByTwoId(int id,int id2)
        {
            return DbSet.Find(id,id2);
        }

        public void Remove(int id)
        {
            var entity = GetById(id);
            RemoveEntity(entity);
        }

        public void Update(T entity)
        {
            DbSet.Update(entity);
            _context.SaveChanges();
        }
        public void RemoveEntity(T entity)
        {
            DbSet.Remove(entity);
            _context.SaveChanges();
        }
    }
}
