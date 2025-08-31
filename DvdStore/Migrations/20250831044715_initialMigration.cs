using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DvdStore.Migrations
{
    /// <inheritdoc />
    public partial class initialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_Artists",
                columns: table => new
                {
                    ArtistID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArtistName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Artists", x => x.ArtistID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Category",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Category", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Producers",
                columns: table => new
                {
                    ProducerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProducerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ContactInfo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Producers", x => x.ProducerID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Suppliers",
                columns: table => new
                {
                    SupplierID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ContactInfo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Suppliers", x => x.SupplierID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Albums",
                columns: table => new
                {
                    AlbumID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ArtistID = table.Column<int>(type: "int", nullable: true),
                    CategoryID = table.Column<int>(type: "int", nullable: true),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoverImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Albums", x => x.AlbumID);
                    table.ForeignKey(
                        name: "FK_tbl_Albums_tbl_Artists_ArtistID",
                        column: x => x.ArtistID,
                        principalTable: "tbl_Artists",
                        principalColumn: "ArtistID");
                    table.ForeignKey(
                        name: "FK_tbl_Albums_tbl_Category_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "tbl_Category",
                        principalColumn: "CategoryID");
                });

            migrationBuilder.CreateTable(
                name: "tbl_PurchaseInvoices",
                columns: table => new
                {
                    PurchaseInvoiceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierID = table.Column<int>(type: "int", nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_PurchaseInvoices", x => x.PurchaseInvoiceID);
                    table.ForeignKey(
                        name: "FK_tbl_PurchaseInvoices_tbl_Suppliers_SupplierID",
                        column: x => x.SupplierID,
                        principalTable: "tbl_Suppliers",
                        principalColumn: "SupplierID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Carts",
                columns: table => new
                {
                    CartID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Carts", x => x.CartID);
                    table.ForeignKey(
                        name: "FK_tbl_Carts_tbl_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "tbl_Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Orders",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShippingAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Orders", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_tbl_Orders_tbl_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "tbl_Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Products",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlbumID = table.Column<int>(type: "int", nullable: false),
                    SupplierID = table.Column<int>(type: "int", nullable: true),
                    ProducerID = table.Column<int>(type: "int", nullable: true),
                    SKU = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Products", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK_tbl_Products_tbl_Albums_AlbumID",
                        column: x => x.AlbumID,
                        principalTable: "tbl_Albums",
                        principalColumn: "AlbumID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_Products_tbl_Producers_ProducerID",
                        column: x => x.ProducerID,
                        principalTable: "tbl_Producers",
                        principalColumn: "ProducerID");
                    table.ForeignKey(
                        name: "FK_tbl_Products_tbl_Suppliers_SupplierID",
                        column: x => x.SupplierID,
                        principalTable: "tbl_Suppliers",
                        principalColumn: "SupplierID");
                });

            migrationBuilder.CreateTable(
                name: "tbl_Payments",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Payments", x => x.PaymentID);
                    table.ForeignKey(
                        name: "FK_tbl_Payments_tbl_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "tbl_Orders",
                        principalColumn: "OrderID");
                    table.ForeignKey(
                        name: "FK_tbl_Payments_tbl_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "tbl_Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "tbl_CartItems",
                columns: table => new
                {
                    CartItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_CartItems", x => x.CartItemID);
                    table.ForeignKey(
                        name: "FK_tbl_CartItems_tbl_Carts_CartID",
                        column: x => x.CartID,
                        principalTable: "tbl_Carts",
                        principalColumn: "CartID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_CartItems_tbl_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "tbl_Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_OrderDetails",
                columns: table => new
                {
                    OrderDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_OrderDetails", x => x.OrderDetailID);
                    table.ForeignKey(
                        name: "FK_tbl_OrderDetails_tbl_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "tbl_Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_OrderDetails_tbl_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "tbl_Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_PurchaseInvoiceDetails",
                columns: table => new
                {
                    PurchaseInvoiceDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseInvoiceID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_PurchaseInvoiceDetails", x => x.PurchaseInvoiceDetailID);
                    table.ForeignKey(
                        name: "FK_tbl_PurchaseInvoiceDetails_tbl_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "tbl_Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_PurchaseInvoiceDetails_tbl_PurchaseInvoices_PurchaseInvoiceID",
                        column: x => x.PurchaseInvoiceID,
                        principalTable: "tbl_PurchaseInvoices",
                        principalColumn: "PurchaseInvoiceID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Albums_ArtistID",
                table: "tbl_Albums",
                column: "ArtistID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Albums_CategoryID",
                table: "tbl_Albums",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_CartItems_CartID",
                table: "tbl_CartItems",
                column: "CartID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_CartItems_ProductID",
                table: "tbl_CartItems",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Carts_UserID",
                table: "tbl_Carts",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_OrderDetails_OrderID",
                table: "tbl_OrderDetails",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_OrderDetails_ProductID",
                table: "tbl_OrderDetails",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Orders_UserID",
                table: "tbl_Orders",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Payments_OrderID",
                table: "tbl_Payments",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Payments_UserID",
                table: "tbl_Payments",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Products_AlbumID",
                table: "tbl_Products",
                column: "AlbumID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Products_ProducerID",
                table: "tbl_Products",
                column: "ProducerID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Products_SupplierID",
                table: "tbl_Products",
                column: "SupplierID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_PurchaseInvoiceDetails_ProductID",
                table: "tbl_PurchaseInvoiceDetails",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_PurchaseInvoiceDetails_PurchaseInvoiceID",
                table: "tbl_PurchaseInvoiceDetails",
                column: "PurchaseInvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_PurchaseInvoices_SupplierID",
                table: "tbl_PurchaseInvoices",
                column: "SupplierID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_CartItems");

            migrationBuilder.DropTable(
                name: "tbl_OrderDetails");

            migrationBuilder.DropTable(
                name: "tbl_Payments");

            migrationBuilder.DropTable(
                name: "tbl_PurchaseInvoiceDetails");

            migrationBuilder.DropTable(
                name: "tbl_Carts");

            migrationBuilder.DropTable(
                name: "tbl_Orders");

            migrationBuilder.DropTable(
                name: "tbl_Products");

            migrationBuilder.DropTable(
                name: "tbl_PurchaseInvoices");

            migrationBuilder.DropTable(
                name: "tbl_Users");

            migrationBuilder.DropTable(
                name: "tbl_Albums");

            migrationBuilder.DropTable(
                name: "tbl_Producers");

            migrationBuilder.DropTable(
                name: "tbl_Suppliers");

            migrationBuilder.DropTable(
                name: "tbl_Artists");

            migrationBuilder.DropTable(
                name: "tbl_Category");
        }
    }
}
