﻿using DomainLayer.Interfaces;
using InfrastructureLayer.Repositories;

namespace InfrastructureLayer.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ECommerceDbContext _db;
        private readonly BrandRepository _brandRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly ProductRepository _productRepository;
        private readonly FacetRepository _facetRepository;
        private readonly FacetValueRepository _facetValueRepository;
        private readonly ProductFacetValueRepository _productFacetValueRepository;

        public UnitOfWork(ECommerceDbContext db)
        {
            _db = db;
            _brandRepository = new BrandRepository(_db);
            _categoryRepository = new CategoryRepository(_db);
            _productRepository = new ProductRepository(_db);
            _facetRepository = new FacetRepository(_db);
            _facetValueRepository = new FacetValueRepository(_db);
            _productFacetValueRepository = new ProductFacetValueRepository(_db);
        }

        public IBrandRepository BrandRepository => _brandRepository;
        public ICategoryRepository CategoryRepository => _categoryRepository;
        public IProductRepository ProductRepository => _productRepository;
        public IFacetRepository FacetRepository => _facetRepository;
        public IFacetValueRepository FacetValueRepository => _facetValueRepository;
        public IProductFacetValueRepository ProductFacetValueRepository => _productFacetValueRepository;
        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
