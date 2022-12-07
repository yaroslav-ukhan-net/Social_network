using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public interface IRepository<TEntity> where TEntity : class
    {
        List<TEntity> GetAll();
        TEntity GetById(int id);
        void Remove(int id);
        TEntity Create(TEntity entity);
        void Update(TEntity entity);
        void RemoveEntity(TEntity entity);
        TEntity GetByTwoId(int id,int id2);
    }
}
