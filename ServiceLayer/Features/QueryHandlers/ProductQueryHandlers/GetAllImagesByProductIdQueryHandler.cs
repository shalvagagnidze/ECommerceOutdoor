using AutoMapper;
using DomainLayer.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ServiceLayer.Features.Queries.ProductQueries;
using ServiceLayer.Interfaces;

namespace ServiceLayer.Features.QueryHandlers.ProductQueryHandlers;

//public class GetAllImagesByProductIdQueryHandler : IRequestHandler<GetAllImagesByProductIdQuery, List<string>>
//{
//    private readonly IMapper _mapper;
//    private readonly IUnitOfWork _unitOfWork;
//    private readonly IFileService _fileService;
//    public GetAllImagesByProductIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
//    {
//        _unitOfWork = unitOfWork;
//        _mapper = mapper;
//        _fileService = fileService;
//    }
//    public async Task<List<string>> Handle(GetAllImagesByProductIdQuery request, CancellationToken cancellationToken)
//    {
//        var product = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id);

//        string imageUrl = string.Empty;
//        List<string> images = new List<string>();

//        try
//        {
//            //string Filepath = GetFilepath(productcode);
//            List<string> imagepath = product.Images!;

//            //foreach (var image in imagepath)
//            //{
//            //    if (System.IO.File.Exists(image))
//            //    {
//            //        imageUrl = request.hostUrl + "/Upload/product/" + request.Id + "/" + request.Id + ".png";
//            //        imageUrl = $"{request.hostUrl.TrimEnd('/')}/Images/product/{request.Id}/{request.Id}.png";
//            //        images.Add(imageUrl);
//            //    }
//            //    else
//            //    {
//            //        throw new FileNotFoundException();
//            //    }
//            //}

//            return product.Images!;


//        }
//        catch (Exception ex)
//        {
//            throw;
//        }

//    }


//}

public class GetAllImagesByProductIdQueryHandler : IRequestHandler<GetAllImagesByProductIdQuery, List<string>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<GetAllImagesByProductIdQueryHandler> _logger;

    public GetAllImagesByProductIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService, IMemoryCache cache, ILogger<GetAllImagesByProductIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileService = fileService;
        _cache = cache;
        _logger = logger; 
    }

    public async Task<List<string>> Handle(GetAllImagesByProductIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"product_{request.Id}_images";


        if (_cache.TryGetValue(cacheKey, out List<string>? cachedImages))
        {
            _logger.LogInformation($"Cache hit: Retrieved images for product ID {request.Id} from cache.");
            return cachedImages!; // Return cached images
        }

        _logger.LogInformation($"Cache miss: Fetching images for product ID {request.Id} from the database.");

        var product = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id);
        if (product == null || product.Images == null)
        {
            _logger.LogWarning($"Product with ID {request.Id} not found or contains no images.");
            throw new FileNotFoundException("Product or images not found.");
        }

        try
        {
            List<string> imagePaths = product.Images!;

            // Log before caching
            _logger.LogInformation($"Caching images for product ID {request.Id}. Cache key: {cacheKey}");

            // Cache the result
            _cache.Set(cacheKey, imagePaths, TimeSpan.FromMinutes(30)); // Cache for 30 minutes

            _logger.LogInformation($"Images for product ID {request.Id} cached successfully.");

            return imagePaths;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while retrieving images for product ID {request.Id}.");
            throw new Exception("Error retrieving images.", ex);
        }
    }
}

