using Asp.Versioning;
using DavidStudio.Core.Auth.Controllers;
using DavidStudio.Core.Essentials.CompleteSample.Data;
using DavidStudio.Core.Essentials.CompleteSample.Dtos.Product;
using DavidStudio.Core.Essentials.CompleteSample.Models.Product;
using DavidStudio.Core.Essentials.CompleteSample.Services;
using DavidStudio.Core.Essentials.CompleteSample.StronglyTypedIds;
using DavidStudio.Core.Pagination.InfiniteScroll;
using DavidStudio.Core.Results.Generic;
using DavidStudio.Core.Swagger.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DavidStudio.Core.Essentials.CompleteSample.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[SwaggerControllerOrder(0)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class ProductsController(IProductsService productsService) : IdentityController
{
    [Authorize(Permissions.Products.Read)]
    [Authorize(Permissions.Manufacturers.Read)]
    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType<OperationResult<InfinitePageData<ProductReadDto>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<OperationResult<InfinitePageData<ProductReadDto>>>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromQuery] InfinitePageOptions options, [FromQuery] string? orderBy)
    {
        var result = await productsService.GetAllAsync(options, orderBy,
            allowedToOrderBy:
            [
                e => e.Id,
                e => e.Price,
                e => e.ModifiedAtUtc,
                e => e.Manufacturer.Name
            ]
        );

        return !result.Succeeded
            ? BadRequest(result)
            : Ok(result);
    }

    [Authorize(Permissions.Products.Read)]
    [Authorize(Permissions.Manufacturers.Read)]
    [HttpGet]
    [Route("{id}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType<OperationResult<ProductReadDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<OperationResult<ProductReadDto>>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] ProductId id)
    {
        var result = await productsService.GetByIdAsync(id);

        return !result.Succeeded
            ? NotFound(result)
            : Ok(result);
    }

    [Authorize(Permissions.Products.Manage)]
    [HttpPost]
    [MapToApiVersion("1.0")]
    [ProducesResponseType<OperationResult<ProductReadDto>>(StatusCodes.Status201Created)]
    [ProducesResponseType<OperationResult<ProductReadDto>>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
    {
        var model = new ProductCreateModel(
            Name: dto.Name,
            Price: dto.Price,
            StockCount: dto.StockCount,
            ManufacturerId: dto.ManufacturerId,
            UserId: GetCurrentUserId()
        );

        var result = await productsService.CreateAsync(model);

        return !result.Succeeded
            ? BadRequest(result)
            : CreatedAtAction(nameof(GetById), new { result.Value.Id }, result);
    }

    [Authorize(Permissions.Products.Manage)]
    [HttpPut]
    [Route("{id}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType<OperationResult<ProductReadDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<OperationResult<ProductReadDto>>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromRoute] ProductId id, [FromBody] ProductUpdateDto dto)
    {
        var model = new ProductUpdateModel(
            Name: dto.Name,
            Price: dto.Price,
            StockCount: dto.StockCount,
            UserId: GetCurrentUserId()
        );

        var result = await productsService.UpdateAsync(id, model);

        return !result.Succeeded
            ? BadRequest(result)
            : Ok(result);
    }

    [Authorize(Permissions.Products.Manage)]
    [HttpDelete]
    [Route("{id}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType<OperationResult<ProductReadDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<OperationResult<ProductReadDto>>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] ProductId id)
    {
        var result = await productsService.DeleteAsync(id);

        return !result.Succeeded
            ? NotFound(result)
            : Ok(result);
    }
}