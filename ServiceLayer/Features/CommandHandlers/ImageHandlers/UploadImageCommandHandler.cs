using Amazon.S3;
using Amazon.S3.Model;
using DomainLayer.Entities;
using DomainLayer.Entities.Products;
using DomainLayer.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using ServiceLayer.Features.Commands.ImageCommands;
using ServiceLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Features.CommandHandlers.ImageHandlers
{
    public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand, List<string>>
    {
        private readonly IAmazonS3 _s3;
        private readonly IOptions<S3Settings> _s3Settings;
        private readonly IUnitOfWork _unitOfWork;

        public UploadImageCommandHandler(IAmazonS3 s3Client, IOptions<S3Settings> s3Settings, IUnitOfWork unitOfWork)
        {
            _s3 = s3Client;
            _s3Settings = s3Settings;
            _unitOfWork = unitOfWork;
        }
        public async Task<List<string>> Handle(UploadImageCommand request, CancellationToken cancellationToken)
        {
            Brand brand = new Brand();
            Product product = new Product();
            if (request.IsBrand)
            {
                brand = await _unitOfWork.BrandRepository.GetByIdAsync(request.productId);
                if (brand is null)
                {
                    throw new Exception("Brand not found.");
                }
            }
            else
            {
                product = await _unitOfWork.ProductRepository.GetByIdAsync(request.productId);

                if (product is null)
                {
                    throw new Exception("Product not found.");
                }
            }


            var imageUrls = new List<string>();

            foreach (var file in request.files)
            {
                if (file.Length > 4194304)
                {
                    throw new ArgumentException("File size should not exceed 4 MB");
                }

                string key = "";

                if (request.IsBrand)
                {
                    key = $"images/{brand.Id}/{Guid.NewGuid()}";
                }
                else
                {
                    key = $"images/{product.Id}/{Guid.NewGuid()}";
                }

                using var stream = file.OpenReadStream();

                var putRequest = new PutObjectRequest
                {
                    BucketName = _s3Settings.Value.BucketName,
                    Key = key,
                    InputStream = stream,
                    ContentType = file.ContentType,
                    Metadata = { ["file-name"] = file.FileName }
                };

                await _s3.PutObjectAsync(putRequest, cancellationToken);

                var imageUrl = $"https://{_s3Settings.Value.BucketName}.s3.{_s3Settings.Value.Region}.amazonaws.com/{key}";
                imageUrls.Add(imageUrl);
            }

            if (request.IsBrand)
            {
                brand.Images = brand.Images != null
                ? brand.Images.Concat(imageUrls).ToList()
                : imageUrls;
            }
            else
            {
                product.Images = product.Images != null
               ? product.Images.Concat(imageUrls).ToList()
               : imageUrls;
            }

            await _unitOfWork.SaveAsync();

            return imageUrls;
        }
    }
}
