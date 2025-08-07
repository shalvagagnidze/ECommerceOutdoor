using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Features.Commands.BrandCommands;
using ServiceLayer.Features.Commands.ImageCommands;
using ServiceLayer.Features.Queries.BrandQueries;
using ServiceLayer.Models;

namespace ECommerceOutdoor.Controllers;

[ApiController]
[Route("[controller]")]
public class BrandController : ApiControllerBase
{
    [HttpGet("get-all-brands")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAllBrands()
    {
        var brands = await Mediator.Send(new GetAllBrandsQuery());

        return Ok(brands);
    }

    [HttpGet("get-brand-by-{id}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetBrandByIds(Guid id)
    {
        var brand = await Mediator.Send(new GetBrandByIdQuery(id));

        return Ok(brand);
    }

    [HttpGet("get-brands-by-searching")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetBrandsBySearching(
        string? searchTerm,
        string? sortColumn,
        string? sortOrder,
        int page,
        int pageSize)
    {
        var brands = await Mediator.Send(new BrandSearchingQuery(searchTerm, sortColumn, sortOrder, page, pageSize));

        return Ok(brands);
    }

    [HttpPost("add-brand")]
    [ProducesResponseType(200)]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> CreateBrand(CreateBrandCommand command)
    {
        var brandId = await Mediator.Send(command);

        return Ok(brandId);
    }

    [HttpPut("update-brand")]
    [ProducesResponseType(200)]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> UpdateBrand(BrandModel model)
    {
        await Mediator.Send(new UpdateBrandCommand(model));

        return Ok("Brand updated successfully!");
    }

    [HttpPost("images")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> UploadImage([FromForm] UploadImageCommand command)
    {
        try
        {
            var modifiedCommand = command with { IsBrand = true };

            var imageUrls = await Mediator.Send(modifiedCommand);

            return Ok(imageUrls);
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPut("delete-brand")]
    [ProducesResponseType(200)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBrand(BrandModel model)
    {
        await Mediator.Send(new DeleteBrandCommand(model));

        return Ok("Brand deleted successfully!");
    }

    [HttpPut("delete-brand-by-{id}")]
    [ProducesResponseType(200)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBrandById(Guid id)
    {
        await Mediator.Send(new DeleteBrandByIdCommand(id));

        return Ok($"Brand with Id:{id} deleted successfully!");
    }

    [HttpDelete("delete-image-{key}-by-brand-{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteImage(Guid id, string key)
    {
        await Mediator.Send(new DeleteImageCommand(id, key, true));

        return Ok("Image deleted successfully!");
    }
}
