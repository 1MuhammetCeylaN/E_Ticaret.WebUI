using E_Ticaret.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Ticaret.Service.Abstract
{  
                                                  // Sadece core katmanında bulunan entityler olabillir yani tablolar
    public interface IService<T> where T : class , IEntity , new()
    {
        List<T> GetAll();  // Listeyi döndürür
        List<T> GetAll(Expression<Func<T,bool>> expression); // Listede sorgulama yapabilme imkanı sağlat
        IQueryable<T> GetQueryable();
        T Get(Expression<Func<T,bool>> expression);
        T Find(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        int SaveChanges();


        // ASENKRON METOTLAR

        Task<T> FindAsync(int id);
        Task<T> GetAsync(Expression<Func<T, bool>> expression);
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        Task<int> SaveChangesAsync();
    }
}
