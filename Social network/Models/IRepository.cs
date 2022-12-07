using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        TEntity GetById(int id);
        void Remove(int id);
        TEntity Create(TEntity entity);
        void Update(TEntity entity);
        void RemoveEntity(TEntity entity);
        TEntity GetByTwoId(int id,int id2);
        IQueryable<TEntity> GetAllQuerible(Expression<Func<TEntity, bool>> expression);
    }
}
