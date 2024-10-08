﻿using AutoMapper;
using DomainLayer.Entities;
using DomainLayer.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using ServiceLayer.Features.Queries.ProductQueries;
using ServiceLayer.Models;
using System;
using static System.Net.Mime.MediaTypeNames;


namespace ServiceLayer.Features.QueryHandlers.ProductQueryHandlers;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductResponseModel>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUrlHelper _urlHelper;
    public GetAllProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        var actionContext = new ActionContext(
            httpContextAccessor.HttpContext!,
            httpContextAccessor.HttpContext!.GetRouteData(),
            new ActionDescriptor());

        _urlHelper = new UrlHelper(actionContext);
    }
    public async Task<IEnumerable<ProductResponseModel>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var models = await _unitOfWork.ProductRepository.GetAllAsync();

            if (models is null)
            {
                return Enumerable.Empty<ProductResponseModel>();
            }

        var products = _mapper.Map<IEnumerable<ProductResponseModel>>(models);


        //foreach(var product in products)
        //{
        //    var imgUrl = product.Images!.Select(img => _urlHelper.Content($"~/Resources/{img}")).ToList();
        //    product.Images = imgUrl;
        //}

        return products;
    }
}
