﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Features.Commands.ProductCommands;
using ServiceLayer.Features.Queries.ProductQueries;
using ServiceLayer.Interfaces;
using ServiceLayer.Models;

namespace ECommerceOutdoor.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController(IFileService fileService) : ApiControllerBase
{
    [HttpGet("get-all-products")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await Mediator.Send(new GetAllProductsQuery());
        return Ok(products);
    }

    [HttpGet("get-products-by-{id}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var product = await Mediator.Send(new GetProductByIdQuery(id));

        return Ok(product);
    }

    [HttpGet("get-product-by-searching")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetProductBySearching(
        string? searchTerm,
        string? sortColumn,
        string? sortOrder,
        int page,
        int pageSize)
    {
        var products = await Mediator.Send(new ProductSearchingQuery(searchTerm, sortColumn, sortOrder, page, pageSize));

        return Ok(products);
    }

    [HttpGet("get-products-by-filtering")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetProductsByFiltering(
        FilterModel filter,
        int page,
        int pageSize)
    {
        var products = await Mediator.Send(new ProductFilteringQuery(filter, page, pageSize));

        return Ok(products);
    }

    [HttpPost("upload-images")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> UploadImage([FromForm] List<IFormFile> imageFiles)
    {
        try
        {
            string[] allowedFileExtensions = new[] { ".jpg", ".jpeg", ".png" };

            // Save each image and add the file name to the list
            var createdImageNames = await fileService.SaveFileAsync(imageFiles, allowedFileExtensions);

            return Ok(createdImageNames);
        }
        catch (Exception)
        {

            throw;
        }
    }

    [HttpPost("add-product")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> CreateProduct(CreateProductCommand command)
    {
        try
        {
            var productId = await Mediator.Send(command);

            return Ok(productId);
        }
        catch (Exception)
        {

            throw;
        }
    }

    [HttpPut("update-product")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> UpdateProduct(ProductModel model)
    {
        await Mediator.Send(new UpdateProductCommand(model));

        return Ok("Product updated successfully!");
    }

    [HttpPut("delete-product")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(ProductModel model)
    {
        await Mediator.Send(new DeleteProductCommand(model));

        return Ok("Product deleted successfully!");
    }


    [HttpPut("delete-product-by-{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProductById(Guid id)
    {
        await Mediator.Send(new DeleteProductByIdCommand(id));

        return Ok("Product deleted successfully!");
    }
}
