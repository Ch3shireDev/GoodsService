using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoodsService.Entities;

[Table("SHOPS")]
public class ShopEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IList<GoodsEntity> Goods { get; init; } = [];
}