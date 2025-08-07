using AutoMapper;
using DomainLayer.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ServiceLayer.Features.Queries.BrandQueries;
using ServiceLayer.Features.QueryHandlers.ProductQueryHandlers;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Features.QueryHandlers.BrandQueryHandlers
{
    public class GetAllImagesByBrandQueryHandler : IRequestHandler<GetAllImagesByBrandIdQuery, List<string>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<GetAllImagesByBrandQueryHandler> _logger;

        public GetAllImagesByBrandQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService, IMemoryCache cache, ILogger<GetAllImagesByBrandQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
            _cache = cache;
            _logger = logger;
        }
        public async Task<List<string>> Handle(GetAllImagesByBrandIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"brand_{request.Id}_images";


            if (_cache.TryGetValue(cacheKey, out List<string>? cachedImages))
            {
                _logger.LogInformation($"Cache hit: Retrieved images for brand ID {request.Id} from cache.");
                return cachedImages!;
            }

            _logger.LogInformation($"Cache miss: Fetching images for brand ID {request.Id} from the database.");

            var brand = await _unitOfWork.BrandRepository.GetByIdAsync(request.Id);
            if (brand == null || brand.Images == null)
            {
                _logger.LogWarning($"Brand with ID {request.Id} not found or contains no images.");
                throw new FileNotFoundException("Brand or images not found.");
            }

            try
            {
                List<string> imagePaths = brand.Images!;

                _logger.LogInformation($"Caching images for brand ID {request.Id}. Cache key: {cacheKey}");

                _cache.Set(cacheKey, imagePaths, TimeSpan.FromMinutes(30)); // Cache for 30 minutes

                _logger.LogInformation($"Images for brand ID {request.Id} cached successfully.");

                return imagePaths;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving images for brand ID {request.Id}.");
                throw new Exception("Error retrieving images.", ex);
            }
        }
    }
}
