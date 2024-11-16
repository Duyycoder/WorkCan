using BaseApp.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace BaseApp.Repository
{
    public interface IRepositoryBase<T>
    {

        // IQueryable provides functionality to evaluate queries against a specific datasource
        // wherein the type of the data is known
        IQueryable<T> FindAll();

        // Expression<Func<T, bool>> represents a strongly typed lambda epxression as a data structure
        // in the form of an expression tree

        // Func<T, bool> returns a function that takes an object as the parameter
        // and return bool as the result
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> condition);

        Task Create(T entity);

        void Update(T entity);

        void Delete(T entity);

    }

    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly BaseAppDBContext _dbContext;

        public RepositoryBase(BaseAppDBContext dbContext) => _dbContext = dbContext;

        // as no tracking helps to improve efficiency since it doesn't detect
        // changes in entites while reading it

        public IQueryable<T> FindAll() => _dbContext.Set<T>().AsNoTracking<T>();

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> condition) => _dbContext.Set<T>().Where(condition).AsNoTracking<T>();

        public async Task Create(T entity) => await _dbContext.AddAsync(entity);

        public void Update(T entity) => _dbContext.Update(entity);

        public void Delete(T entity) => _dbContext.Remove(entity);

            
    }
}
