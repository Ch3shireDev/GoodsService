using GoodsService.DTOs;
using GoodsService.Entities;
using GoodsService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GoodsService.Controllers;

[ApiController]
[Route("/api/v1/shops")]
public class ShopsController(ILogger<ShopsController> logger, IShopRepository shopRepository, IGoodsRepository goodsRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pageSize = 10, [FromQuery] int page = 0)
    {
        var shops = await shopRepository.ListAsync(pageSize, page).ConfigureAwait(false);

        var result = new ShopListResult
        {
            Shops = shops.ToList(),
            Page = page,
            PageSize = pageSize
        };

        return Ok(result);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        logger.LogInformation("Getting shop with id: {id}", id);

        var shop = await shopRepository.GetAsync(id).ConfigureAwait(false);

        if (shop is null)
        {
            return NotFound();
        }

        return Ok(shop);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ShopCreateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var shop = new ShopEntity
        {
            Name = request.Name
        };

        var result = await shopRepository.AddAsync(shop).ConfigureAwait(false);

        return
            CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Put(Guid id, ShopPutRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var shop = new ShopEntity
        {
            Id = id,
            Name = request.Name
        };

        var result = await shopRepository.UpdateAsync(shop).ConfigureAwait(false);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await shopRepository.DeleteAsync(id).ConfigureAwait(false);

        return NoContent();
    }
}
