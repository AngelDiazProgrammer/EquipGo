using Interface.Repositories;
using Microsoft.EntityFrameworkCore;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly EquipGoDbContext _context;
        private readonly DbSet<T> _dbSet; // Obtiene dinámicamente el DbSet<T> de la entidad T

        public GenericRepository(EquipGoDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.Where(predicate).ToListAsync();

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Remove(T entity) => _dbSet.Remove(entity);
    }
}
