namespace GoodsService.DTOs;

public class GoodsGetResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Count { get; set; }
    public Guid? ShopId { get; set; }
}
