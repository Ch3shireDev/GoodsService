using FluentAssertions;
using GoodsService.DTOs;
using GoodsService.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace GoodsService.Tests.Integration;

public class GoodsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ShopsDbContext _context;

    public GoodsControllerTests(WebApplicationFactory<Program> factory)
    {
        _context = WebApplicationExtensions.CreateContext();
        _client = factory.AddContext(_context).CreateClient();
    }

    [Fact]
    public async Task GetGoods_WhenGoodsInDatabase_ReturnsGoods()
    {
        // Arrange
        var goodsId = Guid.NewGuid();
        var goods = new GoodsEntity
        {
            Id = goodsId,
            Name = "Test Goods",
            Price = 100
        };

        await _context.AddAsync(goods);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/goods/{goodsId}");
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GoodsGetResponse>(content) ?? throw new Exception();

        // Assert
        response.EnsureSuccessStatusCode();
        result.Should().BeEquivalentTo(new GoodsGetResponse
        {
            Id = goods.Id,
            Name = goods.Name,
            Price = goods.Price
        });
    }

    [Fact]
    public async Task PostGoods_WhenEmptyDatabse_ReturnsCreatedGoods()
    {
        // Arrange
        var goods = new GoodsCreateRequest
        {
            Name = "Test Goods",
            Price = 100,
            Count = 10,
        };
        var content = new StringContent(JsonConvert.SerializeObject(goods), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync($"/api/v1/goods", content);
        var stringResult = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GoodsGetResponse>(stringResult) ?? throw new Exception();

        // Assert
        response.EnsureSuccessStatusCode();
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("Test Goods");
        result.Price.Should().Be(100);
        result.Count.Should().Be(10);
    }

    [Fact]
    public async Task PostGoods_WhenShopIdIsNotNullAndShopExists_ReturnsCreatedGoods()
    {
        // Arrange
        var shopId = Guid.NewGuid();
        var shop = new ShopEntity
        {
            Id = shopId,
            Name = "Test Shop"
        };

        _context.Add(shop);
        _context.SaveChanges();

        var goods = new GoodsCreateRequest
        {
            Name = "Test Goods",
            Price = 100,
            Count = 10,
            ShopId = shopId
        };

        var content = new StringContent(JsonConvert.SerializeObject(goods), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync($"/api/v1/goods", content);
        var stringResult = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GoodsGetResponse>(stringResult) ?? throw new Exception();

        // Assert
        response.EnsureSuccessStatusCode();
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("Test Goods");
        result.Price.Should().Be(100);
        result.Count.Should().Be(10);
        result.ShopId.Should().Be(shopId);
    }

    [Fact]
    public async Task PostGoods_WhenShopIdIsNotNullButShopDoesntExist_ReturnsNotFoundStatusCode()
    {
        // Arrange
        var shopId = Guid.NewGuid();
        var goods = new GoodsCreateRequest
        {
            Name = "Test Goods",
            Price = 100,
            Count = 10,
            ShopId = shopId
        };
        var content = new StringContent(JsonConvert.SerializeObject(goods), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync($"/api/v1/goods", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PutGoods_WhenGoodsExistInDatabase_ReturnsChangedGoods()
    {
        // Arrange
        var goodsId = Guid.NewGuid();
        var goods = new GoodsEntity
        {
            Id = goodsId,
            Name = "Test Goods",
            Price = 100
        };
        _context.Add(goods);
        _context.SaveChanges();

        var newGoods = new GoodsPutRequest
        {
            Name = "New Goods",
            Price = 200
        };

        var content = new StringContent(JsonConvert.SerializeObject(newGoods), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/v1/goods/{goodsId}", content);
        var stringResult = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GoodsGetResponse>(stringResult) ?? throw new Exception();

        // Assert
        response.EnsureSuccessStatusCode();

        result.Should().BeEquivalentTo(new GoodsGetResponse
        {
            Id = goods.Id,
            Name = newGoods.Name,
            Price = newGoods.Price
        });

        var goodsInDb = await _context.Goods.FindAsync(result.Id);
        goodsInDb.Should().BeEquivalentTo(new GoodsEntity
        {
            Id = result.Id,
            Name = result.Name,
            Price = result.Price
        });
    }
}
