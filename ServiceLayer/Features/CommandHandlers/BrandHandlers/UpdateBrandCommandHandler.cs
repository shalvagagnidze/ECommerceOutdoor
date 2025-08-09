using DomainLayer.Interfaces;
using MediatR;
using ServiceLayer.Features.Commands.BrandCommands;

namespace ServiceLayer.Features.CommandHandlers.BrandHandlers;

public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateBrandCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Unit> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        var existingBrand = await _unitOfWork.BrandRepository.GetByIdAsync(request.model.Id);

        if (existingBrand is null)
        {
            throw new ArgumentNullException(nameof(existingBrand),"Brand not found");
        }

        existingBrand.Name = request.model.Name;
        existingBrand.Origin = request.model.Origin;
        existingBrand.Description = request.model.Description;
        existingBrand.Images = request.model.Images;

        _unitOfWork.BrandRepository.Update(existingBrand);
        await _unitOfWork.SaveAsync();

        return Unit.Value;
    }
}
