using GoodsService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GoodsService.Controllers;

[ApiController]
[Route("/api/v1/shops/{shopId}/goods")]
public class ShopsGoodsController(ILogger<ShopsGoodsController> logger, IGoodsRepository goodsRepository) : ControllerBase
{

    [HttpGet]
    [Route("{goodsId}")]
    public async Task<IActionResult> Get(Guid shopId, Guid goodsId)
    {
        logger.LogInformation("Getting goods with id: {GoodsId}", goodsId);

        var goods = await goodsRepository.GetAsync(goodsId).ConfigureAwait(false);

        if (goods is null)
        {
            return NotFound();
        }

        return Ok(goods);
    }
}