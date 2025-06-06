﻿using AutoMapper;
using DomainLayer.Entities;
using DomainLayer.Entities.Facets;
using DomainLayer.Interfaces;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.Features.Commands.FacetCommands;
using ServiceLayer.Features.Commands.ProductCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Features.CommandHandlers.FacetHandlers
{
    public class UpdateFacetCommandHandler : IRequestHandler<UpdateFacetCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateFacetCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateFacetCommand request, CancellationToken cancellationToken)
        {
            var existingFacet = await _unitOfWork.FacetRepository.GetByIdAsync(request.model.Id);

            if (existingFacet is null)
            {
                throw new ArgumentNullException(nameof(existingFacet), "Facet not found");
            }

            existingFacet.Name = request.model.Name;
            existingFacet.DisplayType = request.model.DisplayType;
            existingFacet.IsCustom = request.model.IsCustom;

            var updatedFacetValues = request.model.FacetValues;
            var existingFacetValues = await _unitOfWork.FacetValueRepository.GetAllAsync();
            var filteredFacetValues = existingFacetValues.Where(x => x.FacetId == existingFacet.Id);
            if (updatedFacetValues is not null && updatedFacetValues.Any())
            {
                var updatedFacetValueIds = updatedFacetValues
                                                            .Where(fv => fv.Id != Guid.Empty) 
                                                            .Select(fv => fv.Id)
                                                            .ToList();


                var facetValuesToRemove = filteredFacetValues
                    .Where(existing => !updatedFacetValueIds.Contains(existing.Id))
                    .ToList();

                foreach (var facetValueToRemove in facetValuesToRemove)
                {
                    await _unitOfWork.FacetValueRepository.DeleteByIdAsync(facetValueToRemove.Id);
                }

                foreach (var facetValueModel in updatedFacetValues!)
                {
                    var facetValue = _mapper.Map<FacetValue>(facetValueModel);
                    facetValue.FacetId = existingFacet.Id;
                    await _unitOfWork.FacetValueRepository.AddOrUpdateAsync(facetValue);
                }
            }
            else
            {
                existingFacet.FacetValues = [];
            }

            if (request.model.CategoryIds is not null && request.model.CategoryIds.Any())
            {
                foreach (var categoryId in request.model.CategoryIds!)
                {
                    var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId) ?? throw new Exception("Category values are not valid");
                    existingFacet.Categories?.Add(category);
                }
            }

            await _unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
