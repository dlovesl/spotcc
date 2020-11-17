using Microsoft.Extensions.Configuration;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Spotcc.Services
{
    public interface IRepository<T> : IDisposable
    {
        T Insert(T model);
        T Update(T model);
        T UpdateParentOnly(T model);
        bool Delete(T model);
        T Get(int pk);
        IList<T> Get(Expression<Func<T, bool>> predicate = null, bool recursive = false);
        IList<T> GetAll();
        T FindFirst(Expression<Func<T, bool>> predicate);
    }

    public class Repository<T> : IRepository<T> where T : class, new()
    {
        protected SQLiteConnection Context { get; set; }

        public Repository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SpotCC");
            Context = new SQLiteConnection(connectionString);
            Context.CreateTable<T>();
        }

        public T Insert(T model)
        {
            Context.InsertWithChildren(model, true);
            return model;
        }

        public T Update(T model)
        {
            Context.UpdateWithChildren(model);
            return model;
        }

        public T UpdateParentOnly(T model)
        {
            Context.Update(model);
            return model;
        }

        public bool Delete(T model)
        {
            int iRes = Context.Delete(model);
            return iRes.Equals(1);
        }

        public T Get(int pk)
        {
            var map = Context.GetMapping(typeof(T));
            return Context.Query<T>(map.GetByPrimaryKeySql, pk).First();
        }

        public IList<T> GetAll()
        {
            return new TableQuery<T>(Context).ToList();
        }

        public T FindFirst(Expression<Func<T, bool>> predicate)
        {
            var entity = Context.Find(predicate);
            if(entity == null)
            {
                return null;
            }

            Context.GetChildren(entity, true);
            return entity;
        }

        public IList<T> Get(Expression<Func<T, bool>> predicate = null, bool recursive = false)
        {
            return Context.GetAllWithChildren(predicate, recursive);
        }

        #region IDispose Region
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }        
        #endregion

    }
}
