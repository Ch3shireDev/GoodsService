using FluentAssertions;
using GoodsService.DTOs;
using GoodsService.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace GoodsService.Tests.Integration;

public class ShopsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ShopsDbContext _context;

    public ShopsControllerTests(WebApplicationFactory<Program> factory)
    {
        _context = WebApplicationExtensions.CreateContext();
        _client = factory.AddContext(_context).CreateClient();
    }

    [Fact]
    public async Task GetById_WhenItemInRepository_ReturnsItem()
    {
        // Arrange
        var id = Guid.NewGuid();
        var shop = new ShopEntity
        {
            Id = id,
            Name = "Żabka"
        };
        _context.Shops.Add(shop);

        _context.SaveChanges();

        // Act
        var response = await _client.GetAsync($"/api/v1/shops/{id}");
        var stringResult = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ShopGetResult>(stringResult) ?? throw new Exception();

        // Assert
        response.EnsureSuccessStatusCode();
        result.Should().BeEquivalentTo(new ShopGetResult
        {
            Id = shop.Id,
            Name = shop.Name
        });
    }

    [Fact]
    public async Task GetById_WhenNoItemInRepository_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/shops/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_WhenNoItemsInRepository_ReturnsCreated()
    {
        // Arrange
        var shop = new ShopCreateRequest
        {
            Name = "Żabka"
        };
        var content = new StringContent(JsonConvert.SerializeObject(shop), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/shops", content);
        var stringResult = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ShopCreateResult>(stringResult) ?? throw new Exception();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Name.Should().Be(shop.Name);

        var shopInDb = await _context.Shops.FindAsync(result.Id);
        shopInDb.Should().BeEquivalentTo(new ShopEntity
        {
            Id = result.Id,
            Name = result.Name
        });
    }

    [Fact]
    public async Task Put_WhenItemInRepository_ReturnsUpdatedItem()
    {
        // Arrange
        var id = Guid.NewGuid();
        var shop = new ShopEntity
        {
            Id = id,
            Name = "Żabka"
        };

        _context.Shops.Add(shop);
        _context.SaveChanges();

        var newShop = new ShopPutRequest
        {
            Name = "Biedronka"
        };

        var content = new StringContent(JsonConvert.SerializeObject(newShop), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/v1/shops/{id}", content);
        var stringResult = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ShopCreateResult>(stringResult) ?? throw new Exception();

        // Assert
        response.EnsureSuccessStatusCode();
        result.Should().BeEquivalentTo(new ShopCreateResult
        {
            Id = shop.Id,
            Name = newShop.Name
        });

        var shopInDb = await _context.Shops.FindAsync(result.Id);
        shopInDb.Should().BeEquivalentTo(new ShopEntity
        {
            Id = result.Id,
            Name = result.Name
        });
    }

    [Fact]
    public async Task Put_WhenNoItemInRepository_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var shop = new ShopPutRequest
        {
            Name = "Biedronka"
        };

        var content = new StringContent(JsonConvert.SerializeObject(shop), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/v1/shops/{id}", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenItemInRepository_ReturnsNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var shop = new ShopEntity
        {
            Id = id,
            Name = "Żabka"
        };

        _context.Shops.Add(shop);
        _context.SaveChanges();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/shops/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_WhenNoItemInRepository_ReturnsNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/shops/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task List_WhenFewItemsInRepository_ReturnsItems()
    {
        // Arrange
        _context.Shops.Add(new ShopEntity());
        _context.Shops.Add(new ShopEntity());
        _context.Shops.Add(new ShopEntity());
        _context.SaveChanges();

        // Act
        var response = await _client.GetAsync("/api/v1/shops");
        var stringResult = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ShopListResult>(stringResult) ?? throw new Exception();

        // Assert
        response.EnsureSuccessStatusCode();
        result.Shops.Count.Should().Be(3);
        result.Page.Should().Be(0);
    }

    [Fact]
    public async Task List_WithPageSizeLessThanItemsCount_ReturnsPageSizeItemsCount()
    {
        // Arrange
        _context.Shops.Add(new ShopEntity());
        _context.Shops.Add(new ShopEntity());
        _context.Shops.Add(new ShopEntity());

        _context.SaveChanges();

        var pageSize = 2;

        // Act
        var response = await _client.GetAsync($"/api/v1/shops?pageSize={pageSize}");
        var stringResult = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ShopListResult>(stringResult) ?? throw new Exception();

        // Assert
        response.EnsureSuccessStatusCode();
        result.Shops.Count.Should().Be(pageSize);
        result.Page.Should().Be(0);
    }

    [Fact]
    public async Task List_WithPageSizeLessThanItemsCountAndNextPage_ReturnsOneItem()
    {
        // Arrange
        _context.Shops.Add(new ShopEntity());
        _context.Shops.Add(new ShopEntity());
        _context.Shops.Add(new ShopEntity());

        _context.SaveChanges();

        var pageSize = 2;
        var page = 1;

        // Act
        var response = await _client.GetAsync($"/api/v1/shops?pageSize={pageSize}&page={page}");
        var stringResult = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ShopListResult>(stringResult) ?? throw new Exception();

        // Assert
        response.EnsureSuccessStatusCode();
        result.Shops.Count.Should().Be(1);
        result.Page.Should().Be(1);
    }
}
