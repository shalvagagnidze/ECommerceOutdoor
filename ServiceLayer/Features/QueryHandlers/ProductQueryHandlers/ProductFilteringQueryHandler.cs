using AutoMapper;
using DomainLayer.Entities;
using DomainLayer.Entities.Products;
using InfrastructureLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.Features.Queries.ProductQueries;
using ServiceLayer.Models;

namespace ServiceLayer.Features.QueryHandlers.ProductQueryHandlers;

public class ProductFilteringQueryHandler : IRequestHandler<ProductFilteringQuery, PagedList<ProductModel>>
{
    private readonly DbSet<Product> _dbSet;
    private readonly IMapper _mapper;
    public ProductFilteringQueryHandler(ECommerceDbContext db,IMapper mapper)
    {
        var dbSet = db.Set<Product>();
        _dbSet = dbSet;
        _mapper = mapper;
    }
    public async Task<PagedList<ProductModel>> Handle(ProductFilteringQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Product> products = _dbSet.AsNoTracking()
                                             .Include(x => x.Category).Include(x => x.Brand);
        //IQueryable<Product> products = _dbSet.AsNoTracking().Where(p => !p.isDeleted);

        if (request.filter.BrandIds!.Any())
        {
            products = products.Where(x => request.filter.BrandIds!.Contains(x.Brand!.Id));
        }

        if (request.filter.CategoryIds!.Any())
        {
            var categoryIds = request.filter.CategoryIds;

            //products = products.Where(x => categoryIds.Contains(x.Category!.Id) ||
            //                               categoryIds.Contains(x.Category!.ParentId!.Value));
            products = products.Where(x =>
                                        categoryIds.Contains(x.Category!.Id) ||
                                        (x.Category!.ParentId.HasValue && categoryIds.Contains(x.Category.ParentId.Value)));

        }


        if (request.filter.Condition!.Any())
        {
            products = products.Where(x => request.filter.Condition!.Contains(x.Condition));
        }

        if (request.filter.StockStatus!.HasValue)
        {
            products = products.Where(x => request.filter.StockStatus == x.Status);
        }

        if (request.filter.MinPrice.HasValue)
        {
            products = products.Where(x => x.Price >= request.filter.MinPrice);
        }

        if (request.filter.MaxPrice.HasValue)
        {
            products = products.Where(x => x.Price <= request.filter.MaxPrice);
        }

        if (request.filter.FacetFilters != null && request.filter.FacetFilters.Any())
        {
            var facetValueIds = request.filter.FacetFilters.Select(ff => ff.facetValueId).ToList();
            products = products.Where(x => x.ProductFacetValues
                                             .Any(f => facetValueIds.Contains(f.FacetValueId)));
        }

        var productModelsQuery = products.Select(b => new ProductModel
        {
            Id = b.Id,
            Name = b.Name,
            Price = b.Price,
            Status = b.Status,
            Condition = b.Condition,
            Description = b.Description,
            CategoryId = b.Category!.Id,
            BrandId = b.Brand!.Id,
            ProductFacetValues = b.ProductFacetValues.Select(pfv => new ProductFacetValueModel 
            { Id = pfv.Id, 
              FacetValueId = pfv.FacetValueId 
            }).ToList(),
            Images = b.Images
        });


        var productsQuery = await PagedList<ProductModel>.CreateAsync(productModelsQuery, request.Page, request.PageSize);

        return productsQuery;
    }
}
