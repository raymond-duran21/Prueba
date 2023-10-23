﻿using MagicVilla_API.Controllers.Datos;
using MagicVilla_API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace MagicVilla_API.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db) 
        
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public async Task Crear(T entity)
        {
            await dbSet.AddAsync(entity);
            await Grabar();
        }

        public async Task Grabar()
        {
            await _db.SaveChangesAsync();
        }

        public async Task<T> Obtener(Expression<Func<T, bool>>? filtro = null, bool tracked = true)
        {
            IQueryable<T> query = dbSet;

            if(!tracked) 
            {
                query = query.AsNoTracking();
            }
            if(filtro !=null)
            {
                query.Where(filtro);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> ObtenerTodos(Expression<Func<T, bool>>? filtro = null)
        {
            IQueryable<T> query = dbSet;

            if (filtro != null)
            {
                query.Where(filtro);
            }
            return await query.ToListAsync();
        }

        public async Task Remove(T enity)
        {
            dbSet.Remove(enity);
            await Grabar();
        }
    }
}
