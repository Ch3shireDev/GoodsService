using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodsService.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder, nameof(migrationBuilder));

            migrationBuilder.CreateTable(
                name: "SHOPS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SHOPS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GOODS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    ShopId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GOODS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GOODS_SHOPS_ShopId",
                        column: x => x.ShopId,
                        principalTable: "SHOPS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GOODS_ShopId",
                table: "GOODS",
                column: "ShopId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder, nameof(migrationBuilder));

            migrationBuilder.DropTable(
                name: "GOODS");

            migrationBuilder.DropTable(
                name: "SHOPS");
        }
    }
}
