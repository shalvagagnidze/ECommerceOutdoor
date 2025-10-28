using AutoMapper;
using DomainLayer.Entities.Products;
using InfrastructureLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Features.Queries.ProductQueries;
using ServiceLayer.Models;
using System.Linq.Expressions;

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
                                             .Include(x => x.Category).Include(x => x.Brand).Include(x => x.ProductFacetValues);
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
                (x.Category != null && categoryIds!.Contains(x.Category.Id)) ||
                (x.Category != null && x.Category.ParentId.HasValue && categoryIds!.Contains(x.Category.ParentId.Value))
            );


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
            var facetGroups = request.filter.FacetFilters
                .GroupBy(ff => ff.FacetId) 
                .ToList();

            Expression<Func<Product, bool>>? combinedPredicate = null;

            foreach (var facetGroup in facetGroups)
            {
                var facetValueIds = facetGroup.Select(ff => ff.facetValueId).ToList();

                Expression<Func<Product, bool>>? facetGroupPredicate = null;

                foreach (var facetValueId in facetValueIds)
                {
                    var currentFacetValueId = facetValueId; 
                    Expression<Func<Product, bool>> predicate = x => x.ProductFacetValues!
                        .Any(f => f.FacetValueId == currentFacetValueId);

                    if (facetGroupPredicate == null)
                    {
                        facetGroupPredicate = predicate;
                    }
                    else
                    {
                        facetGroupPredicate = facetGroupPredicate.Or(predicate); 
                    }
                }

                if (facetGroupPredicate != null)
                {
                    if (combinedPredicate == null)
                    {
                        combinedPredicate = facetGroupPredicate;
                    }
                    else
                    {
                        combinedPredicate = combinedPredicate.And(facetGroupPredicate); 
                    }
                }
            }

            if (combinedPredicate != null)
            {
                products = products.Where(combinedPredicate);
            }
        }

        var productModelsQuery = products.Select(b => new ProductModel
        {
            Id = b.Id,
            Name = b.Name,
            Price = b.Price,
            Status = b.Status,
            Condition = b.Condition,
            Description = b.Description,
            CategoryId = b.Category != null ? b.Category.Id : Guid.Empty,
            BrandId = b.Brand != null ? b.Brand.Id : Guid.Empty,
            ProductFacetValues = b.ProductFacetValues!.Select(pfv => new ProductFacetValueModel 
            { Id = pfv.Id, 
              FacetValueId = pfv.FacetValueId 
            }).ToList(),
            Images = b.Images
        });


        var productsQuery = await PagedList<ProductModel>.CreateAsync(productModelsQuery, request.Page, request.PageSize);

        return productsQuery;
    }
}
