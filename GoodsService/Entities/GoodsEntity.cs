using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoodsService.Entities;

[Table("GOODS")]
public class GoodsEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }
    public int Count { get; set; }
    [ForeignKey("ShopId")]
    public Guid? ShopId { get; set; }
}
