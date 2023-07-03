using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public void Actualizar(Bodega bodega)
        {

        }
        public Repositorio(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet=_db.Set<T>();
        }
        public async Task Agregar(T entidad)
        {
            await dbSet.AddAsync(entidad); //insert into table
        }

        public async Task<T> Obtener(int id)
        {
            return await dbSet.FindAsync(id); // select * from (solo por Id)
        }

        public async Task<T> ObtenerPrimero(Expression<Func<T, bool>> filtro, string incluirPropiedades, bool isTracking)
        {
            IQueryable<T> query = dbSet;
            if (filtro != null)
            {
                query = query.Where(filtro);  // select /* from where ....
            }
            if (incluirPropiedades != null)
            {
                foreach (var incluitProp in incluirPropiedades.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluitProp); //ejemplo Categoria,Marca
                }
            }
            if (!isTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> ObtenerTodos(Expression<Func<T, bool>> filtro, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, string incluirPropiedades, bool isTracking)
        {
            IQueryable<T> query = dbSet;
            if (filtro != null)
            {
                query = query.Where(filtro); // select /* from where ....
            }
            if(incluirPropiedades != null)
            {
                foreach(var incluitProp in incluirPropiedades.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluitProp); //ejemplo Categoria,Marca
                }
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (!isTracking)
            {
                query=query.AsNoTracking();
            }
            return  await query.ToListAsync();
        }

        void IRepositorio<T>.Remover(T entidad)
        {
            dbSet.Remove(entidad);
        }

        void IRepositorio<T>.RemoverRango(IEnumerable<T> entidad)
        {
            dbSet.RemoveRange(entidad);
        }
    }
}
