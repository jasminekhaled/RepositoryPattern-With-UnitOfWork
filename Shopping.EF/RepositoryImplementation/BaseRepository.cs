﻿using Microsoft.EntityFrameworkCore;
using Shopping.Core.IRepository;
using Shopping.Core.Models.AuthModule;
using Shopping.EF.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.EF.RepositoryImplementation 
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;                                          
        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }


        public async Task<IEnumerable<T>> GetAllAsync() =>
            await _dbSet.ToListAsync();

        public async Task<T> GetByIdAsync(int id) =>
            await _dbSet.FindAsync(id);


        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
             await _dbSet.AddRangeAsync(entities);
            return entities;
        }

        public void Remove(T entity) =>
            _dbSet.Remove(entity);

        public void RemoveRange(IEnumerable<T> entities) =>
            _dbSet.RemoveRange(entities);

        public void Update(T entity) =>
            _dbSet.Update(entity);


        public void UpdateRange(IEnumerable<T> entities) =>
            _dbSet.UpdateRange(entities);

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> check)
        {

            return await _dbSet.AnyAsync(check);
        }
        public async Task<bool> FilterAndAnyAsync(Expression<Func<T, bool>> check, Expression<Func<T, bool>> filter )
        {
            
            return await _dbSet.Where(filter).AnyAsync(check);
        }


        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> criteria) =>
            await _dbSet.SingleOrDefaultAsync(criteria);


        public async Task<int> CountAsync(Expression<Func<T, bool>> criteria) =>
            await _dbSet.CountAsync(criteria);


        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> criteria) =>
            await _dbSet.FirstOrDefaultAsync(criteria);


        public async Task<IEnumerable<T>> Where(Expression<Func<T, bool>> criteria) =>
             await _dbSet.AsNoTracking().Where(criteria).ToListAsync();


        public async Task<IEnumerable<TResult>> Select<TResult>(Expression<Func<T, TResult>> criteria) =>
            await _dbSet.AsNoTracking().Select(criteria).ToListAsync();


        public async Task<IEnumerable<TResult>> GetSpecificItems<TResult>(Expression<Func<T, bool>> filter, Expression<Func<T, TResult>> select) =>
            await _dbSet.AsNoTracking().Where(filter).Select(select).ToListAsync();


        public async Task<T> GetSpecificItem(Expression<Func<T, bool>> filter, Expression<Func<T, bool>> single) =>
            await _dbSet.AsNoTracking().Where(filter).SingleOrDefaultAsync(single);


        public async Task<T> GetFirstItem(Expression<Func<T, bool>> filter) =>
            await _dbSet.AsNoTracking().Where(filter).FirstOrDefaultAsync();


        


        public async Task<IEnumerable<T>> OrderByDescendingTheTop5(Expression<Func<T, object>> orderBy, int? take)
        {
            IQueryable<T> query = _dbSet.AsNoTracking().OrderByDescending(orderBy);

            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TResult>> GetSelectedAsync<TKey, TResult>(
            Expression<Func<T, bool>> filter,
            Expression<Func<T, TKey>> groupBy,
            Expression<Func<IGrouping<TKey, T>, TResult>> select,
            Expression<Func<TResult, object>> orderBy,
            int take) =>
        
            await _dbSet.Where(filter).GroupBy(groupBy).Select(select).OrderByDescending(orderBy)
                .Take(take).ToListAsync();


        public async Task<TResult> GetOneAsync<TResult>(
            Expression<Func<T, bool>> filter,
            Expression<Func<T, TResult>> select,
            Expression<Func<TResult, object>> orderBy) =>

            await _dbSet.AsNoTracking().Where(filter).Select(select).OrderByDescending(orderBy).FirstOrDefaultAsync();


        public async Task<TResult> GetMultiSelectAsync<TKey, TResult>(
            Expression<Func<T, bool>> filter,
            Expression<Func<T, TResult>> firstSelect,
            Expression<Func<TResult, TKey>> groupBy,
            Expression<Func<IGrouping<TKey, TResult>, TResult>> secondSelect,
            Expression<Func<TResult, object>> orderBy,
            Expression<Func<TResult, TResult>> thirdSelect) =>

            await _dbSet.AsNoTracking().Where(filter).Select(firstSelect).GroupBy(groupBy).Select(secondSelect).OrderByDescending(orderBy)
            .Select(thirdSelect).FirstOrDefaultAsync();
                
    }
}
