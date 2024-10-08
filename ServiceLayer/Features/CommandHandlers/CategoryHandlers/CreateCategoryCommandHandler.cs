﻿using AutoMapper;
using DomainLayer.Entities;
using DomainLayer.Interfaces;
using MediatR;
using ServiceLayer.Features.Commands.CategoryCommands;
using ServiceLayer.Models;

namespace ServiceLayer.Features.CommandHandlers.CategoryHandlers;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var model = new CategoryModel
        {
            Name = request.Name,
            Description = request.Description,
            ParentId = request.ParentId,
        };

        var category = _mapper.Map<Category>(model);

        category.CreatedDate = DateTime.UtcNow;

        await _unitOfWork.CategoryRepository.AddAsync(category);
        await _unitOfWork.SaveAsync();

        return category.Id;
    }
}
