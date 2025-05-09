using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDelivery.Migrations
{
    /// <inheritdoc />
    public partial class CartItemConfigChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Dishes_DishId",
                table: "CartItems");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Dishes_DishId",
                table: "CartItems",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "DishId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Dishes_DishId",
                table: "CartItems");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Dishes_DishId",
                table: "CartItems",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "DishId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
