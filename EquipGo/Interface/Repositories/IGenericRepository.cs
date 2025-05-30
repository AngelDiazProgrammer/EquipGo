using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id); //busqueda por id
        Task<IEnumerable<T>> GetAllAsync(); //Listar todos los registros
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate); //Filtrar
        Task AddAsync(T entity); //Agregar
        void Update(T entity); //Actualizar
        void Remove(T entity); //Eliminar
    }
}
