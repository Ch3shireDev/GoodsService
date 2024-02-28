using FluentAssertions;
using GoodsService.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace GoodsService.Tests.Integration;

public class ShopsGoodsController : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ShopsDbContext _context;
    public ShopsGoodsController(WebApplicationFactory<Program> factory)
    {
        _context = WebApplicationExtensions.CreateContext();
        _client = factory.AddContext(_context).CreateClient();
    }

    [Fact]
    public async Task GetGoods_WhenShopAndGoodsInDatabase_ShouldReturnGoods()
    {
        // Arrange
        var shopId = Guid.NewGuid();
        var goodsId = Guid.NewGuid();

        var shop = new ShopEntity
        {
            Id = shopId,
            Name = "Test Shop",
            Goods =
            [
                new()
                {
                    Id = goodsId,
                    Name = "Test Goods",
                    Price = 100
                }
            ]
        };

        await _context.AddAsync(shop);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/shops/{shopId}/goods/{goodsId}");
        var content = await response.Content.ReadAsStringAsync();
        var goods = JsonConvert.DeserializeObject<GoodsEntity>(content) ?? throw new Exception();

        // Assert
        response.EnsureSuccessStatusCode();
        goods.Should().NotBeNull();
        goods.Id.Should().Be(goodsId);
        goods.Name.Should().Be("Test Goods");
        goods.Price.Should().Be(100);
        goods.ShopId.Should().Be(shopId);
    }
}