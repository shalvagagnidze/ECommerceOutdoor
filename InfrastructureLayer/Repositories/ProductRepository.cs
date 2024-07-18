﻿using DomainLayer.Entities;
using DomainLayer.Interfaces;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DbSet<Product> _dbSet;

        public ProductRepository(ECommerceDbContext db)
        {
            var dbSet = db.Set<Product>();
            _dbSet = dbSet;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            var product = await _dbSet.FindAsync(id);

            return product!;
        }

        public async Task AddAsync(Product product)
        {
            await _dbSet.AddAsync(product);
        }

        public void Delete(Product product)
        {
            _dbSet.Remove(product);
        }

        public async Task DeleteByIdAsync(Guid id)
        {
            var product = await _dbSet.FindAsync(id);

            _dbSet.Remove(product!);
        }

        public void Update(Product product)
        {
            _dbSet.Update(product);
        }
    }
}