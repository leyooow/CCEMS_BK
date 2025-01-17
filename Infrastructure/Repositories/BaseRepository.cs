using Application.Contracts.Repositories;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly CcemQatContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public BaseRepository(CcemQatContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"{typeof(TEntity).Name} with ID {id} not found.");
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalCountAsync(string? searchTerm)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Get string properties of TEntity
                var stringProperties = typeof(TEntity).GetProperties()
                                                       .Where(p => p.PropertyType == typeof(string));

                // Build a combined predicate
                var parameter = Expression.Parameter(typeof(TEntity), "e");
                Expression? combinedExpression = null;

                foreach (var property in stringProperties)
                {
                    // Access the property (e.PropertyName)
                    var propertyAccess = Expression.Property(parameter, property.Name);

                    // Ensure case-insensitive comparison: e.PropertyName.ToLower().Contains(searchTerm.ToLower())
                    var toLowerExpression = Expression.Call(propertyAccess, nameof(string.ToLower), Type.EmptyTypes);
                    var containsExpression = Expression.Call(
                        toLowerExpression,
                        nameof(string.Contains),
                        Type.EmptyTypes,
                        Expression.Constant(searchTerm.ToLower(), typeof(string))
                    );

                    // Combine expressions using OR
                    combinedExpression = combinedExpression == null
                        ? containsExpression
                        : Expression.OrElse(combinedExpression, containsExpression);
                }

                if (combinedExpression != null)
                {
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(combinedExpression, parameter);
                    query = query.Where(lambda);
                }
            }

            return await query.CountAsync();
        }
    }
}
