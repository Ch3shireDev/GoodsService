using GoodsService.DTOs;
using GoodsService.Entities;
using GoodsService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GoodsService.Controllers;

[ApiController]
[Route("/api/v1/goods")]
public class GoodsController(ILogger<GoodsController> logger, IGoodsRepository goodsRepository, IShopRepository shopRepository) : ControllerBase
{
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        logger.LogInformation("Getting goods with id: {id}", id);

        var goods = await goodsRepository.GetAsync(id).ConfigureAwait(false);

        if (goods is null)
        {
            return NotFound();
        }

        var response = new GoodsGetResponse
        {
            Id = goods.Id,
            Name = goods.Name,
            Price = goods.Price,
            Count = goods.Count,
            ShopId = goods.ShopId
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(GoodsCreateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        logger.LogInformation("Creating goods with name: {Name}", request.Name);


        if (request.ShopId != null)
        {
            var shopExists = await shopRepository.Exists(request.ShopId.Value).ConfigureAwait(false);
            if (!shopExists)
            {
                return NotFound($"Shop with id: {request.ShopId} does not exist");
            }
        }

        var goods = new GoodsEntity
        {
            Name = request.Name,
            Price = request.Price,
            Count = request.Count,
            ShopId = request.ShopId
        };

        var result = await goodsRepository.AddAsync(goods).ConfigureAwait(false);

        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Put(Guid id, GoodsPutRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var goods = new GoodsEntity
        {
            Id = id,
            Name = request.Name,
            Price = request.Price,
            Count = request.Count,
            ShopId = request.ShopId
        };

        var result = await goodsRepository.UpdateAsync(goods).ConfigureAwait(false);

        if (result is null)
        {
            return NotFound();
        }

        var response = new GoodsGetResponse
        {
            Id = result.Id,
            Name = result.Name,
            Price = result.Price,
            Count = result.Count,
            ShopId = result.ShopId
        };

        return Ok(response);
    }
}
