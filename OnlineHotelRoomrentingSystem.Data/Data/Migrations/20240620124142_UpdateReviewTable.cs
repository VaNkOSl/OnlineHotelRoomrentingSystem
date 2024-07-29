using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelRoomRentingSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReviewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
             name: "Reviews",
             columns: table => new
             {
                 Id = table.Column<int>(type: "int", nullable: false)
                     .Annotation("SqlServer:Identity", "1, 1"),
                 Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                 Rating = table.Column<int>(type: "int", nullable: false),
                 ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                 UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                 RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                 HotelId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
             },
             constraints: table =>
             {
                 table.PrimaryKey("PK_Reviews", x => x.Id);
                 table.ForeignKey(
                     name: "FK_Reviews_AspNetUsers_UserId",
                     column: x => x.UserId,
                     principalTable: "AspNetUsers",
                     principalColumn: "Id",
                     onDelete: ReferentialAction.Restrict);
                 table.ForeignKey(
                     name: "FK_Reviews_Hotels_HotelId",
                     column: x => x.HotelId,
                     principalTable: "Hotels",
                     principalColumn: "Id",
                     onDelete: ReferentialAction.Restrict);
                 table.ForeignKey(
                     name: "FK_Reviews_Rooms_RoomId",
                     column: x => x.RoomId,
                     principalTable: "Rooms",
                     principalColumn: "Id",
                     onDelete: ReferentialAction.Restrict);
             });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_HotelId",
                table: "Reviews",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RoomId",
                table: "Reviews",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Agents_UserId",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Hotels");

            migrationBuilder.AlterColumn<Guid>(
                name: "RoomId",
                table: "Reviews",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "HotelId",
                table: "Reviews",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Reviews",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.CreateTable(
                name: "IdentityUser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUser", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Agents",
                columns: new[] { "Id", "DateOfBirth", "EGN", "FirstName", "LastName", "MiddleName", "PhoneNumber", "UserId" },
                values: new object[,]
                {
                    { new Guid("3c0e049e-325d-4246-b0e8-444c45905fe5"), new DateTime(1985, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "8505151234", "Ivan", "Petrov", "Stoqnov", "+359888888888", new Guid("259d7876-ce5c-4956-9bf6-d70b7ef0b1e3") },
                    { new Guid("9639e0fa-a591-480c-ae67-1333545ee981"), new DateTime(1995, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "95031756984", "Agent", "Agentov", "AgentJoe", "08459632555", new Guid("a24d63e6-1db1-4720-8e30-79541f66b243") }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Luxury" },
                    { 2, "Budget" },
                    { 3, "Business" },
                    { 4, "Family" },
                    { 5, "Hostel" },
                    { 6, "Motel" }
                });

            migrationBuilder.InsertData(
                table: "IdentityUser",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e", 0, "02736384-59df-4370-bdc4-b6326958e6f4", "secondUser@gmail.com", false, false, null, "secondUser@gmail.com", "secondUser@gmail.com", "AQAAAAIAAYagAAAAEHghMLJawpRsooq2/pXW1z2qqJP2Znpf38ekEaZA94Ya4SrejqXrbO2T+fRwWqmb6g==", null, false, "af6cbaf0-c650-4ccb-9d9d-d73ad13fe5af", false, "secondUser@gmail.com" },
                    { "dea12856-c198-4129-b3f3-b893d8395082", 0, "db714e87-03f8-4e68-82f7-37fd725762cf", "user@gmail.com", false, false, null, "user@gmail.com", "user@gmail.com", "AQAAAAIAAYagAAAAECD7nsrhs6Wy4+JKW0AhA2a5yMuvFizge7sN/QLpBQ+eh/fGhgBaqrGlS9byRl/4vA==", null, false, "b270d2a9-d71e-482f-9299-02ce1cba1fe2", false, "user@gmail.com" }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "Content", "HotelId", "Rating", "ReviewDate", "RoomId", "UserId" },
                values: new object[,]
                {
                    { 1, "Amazing experience, highly recommend!", new Guid("f2c8cf39-d79d-4b2f-a59f-0a1d1e2f5042"), 5, new DateTime(2024, 6, 12, 22, 53, 9, 746, DateTimeKind.Local).AddTicks(8862), new Guid("1b926edf-0eb1-4db8-8022-220a10c928e6"), new Guid("92c401e0-a7a9-41fe-ad2d-60d518b6a5a9") },
                    { 2, "Decent place, but could be better.", new Guid("43470819-a400-4765-8570-415ebb3f1eeb"), 3, new DateTime(2024, 6, 12, 22, 53, 9, 746, DateTimeKind.Local).AddTicks(9115), new Guid("38f0ad0a-53db-4666-8a98-cae9e967720e"), new Guid("b1830f82-ec1f-48e2-a576-015cdeffea7b") }
                });

            migrationBuilder.InsertData(
                table: "RoomType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Single" },
                    { 2, "Double" }
                });

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "Id", "Address", "CategoryId", "City", "Country", "CreatedOn", "Description", "Email", "HotelManagerId", "Image", "MaximumCapacity", "Name", "NumberOfRooms", "PhoneNumber" },
                values: new object[,]
                {
                    { new Guid("0c8e143f-5847-4c00-b136-43742d07131b"), "123 Main Street", 1, "Metropolis", "United States", null, "Luxury Palace Hotel offers the epitome of comfort and elegance with its lavish accommodations and world-class amenities.", "info@luxurypalace.com", new Guid("2c6786c4-263b-4cb9-92c2-e977a31dafb3"), "/images/HotelOne.jpg", 500, "Luxury Palace Hotel", 200, "+35965896547" },
                    { new Guid("d4ffc402-d1f5-4c05-831e-e5d9b7cf2fe1"), "789 Forest Road", 1, "Mountainsville", "United States", null, "Mountain Lodge Retreat offers a peaceful escape surrounded by breathtaking natural beauty, perfect for outdoor enthusiasts and relaxation seekers.", "info@mountainlodge.com", new Guid("e2703489-f2a9-47ea-ac75-f9f06a1f31fc"), "/images/OceansViews.jpg", 300, "Mountain Lodge Retreat", 100, "+3591914999" },
                    { new Guid("d9de3174-3180-4f48-80c1-38c607b1ff5f"), "456 Ocean Avenue", 2, "Beachtown", "United States", null, "Seaside Resort is a tranquil retreat nestled along the picturesque coastline, offering stunning ocean views and impeccable service.", "info@seasideresort.com", new Guid("2c6786c4-263b-4cb9-92c2-e977a31dafb3"), "/images/HotelOne.jpg", 400, "Seaside Resort", 150, "+3598756236514" }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "AgentId", "Description", "HotelId", "Image", "IsAvaible", "PricePerNight", "RenterId", "RoomId", "RoomNumber" },
                values: new object[,]
                {
                    { new Guid("16fd314d-0728-465a-9375-3a654d6995cf"), new Guid("0d5996fc-08e5-4657-aa4a-72649ca8b9d9"), "", new Guid("fd7b7cd1-4755-4bbc-bf0f-22468de0fa84"), "/rooms/room1.jpg", true, 150.00m, null, 1, 0 },
                    { new Guid("ca0fdbdf-f691-4f05-ac66-c7b31c0fbd6f"), new Guid("84935f65-3ea2-4e74-bf93-aea16288a8c3"), "", new Guid("6553e847-25ed-4dfd-ac86-b3abf8462be4"), "/rooms/room2.jpg", true, 75.00m, null, 2, 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agents_UserId",
                table: "Agents",
                column: "UserId");
        }
    }
}
