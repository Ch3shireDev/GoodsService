using GoodsService.Entities;

namespace GoodsService.DTOs;

public class ShopListResult
{
    public IList<ShopEntity> Shops { get; init; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; } = 10;
}