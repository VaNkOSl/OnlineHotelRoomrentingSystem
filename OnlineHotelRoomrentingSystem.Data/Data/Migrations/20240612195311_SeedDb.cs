using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelRoomRentingSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Agents",
                columns: new[] { "Id", "DateOfBirth", "EGN", "FirstName", "LastName", "MiddleName", "PhoneNumber", "UserId" },
                values: new object[,]
                {
                    { new Guid("3c0e049e-325d-4246-b0e8-444c45905fe5"), new DateTime(1985, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "8505151234", "Ivan", "Petrov", "Stoqnov", "+359888888888", new Guid("3F94A3B6-5400-4BC0-8993-23A6CCC226DF") },
                    { new Guid("9639e0fa-a591-480c-ae67-1333545ee981"), new DateTime(1995, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "9503176984", "Agent", "Agentov", "AgentJoe", "08459632555", new Guid("3F94A3B6-5400-4BC0-8993-23A6CCC226DF") }
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
                    { new Guid("0c8e143f-5847-4c00-b136-43742d07131b"), "123 Main Street", 1, "Metropolis", "United States", null, "Luxury Palace Hotel offers the epitome of comfort and elegance with its lavish accommodations and world-class amenities.", "info@luxurypalace.com", new Guid("9639e0fa-a591-480c-ae67-1333545ee981"), "/images/HotelOne.jpg", 500, "Luxury Palace Hotel", 200, "+35965896547" },
                    { new Guid("d4ffc402-d1f5-4c05-831e-e5d9b7cf2fe1"), "789 Forest Road", 1, "Mountainsville", "United States", null, "Mountain Lodge Retreat offers a peaceful escape surrounded by breathtaking natural beauty, perfect for outdoor enthusiasts and relaxation seekers.", "info@mountainlodge.com", new Guid("9639e0fa-a591-480c-ae67-1333545ee981"), "/images/OceansViews.jpg", 300, "Mountain Lodge Retreat", 100, "+3591914999" },
                    { new Guid("d9de3174-3180-4f48-80c1-38c607b1ff5f"), "456 Ocean Avenue", 2, "Beachtown", "United States", null, "Seaside Resort is a tranquil retreat nestled along the picturesque coastline, offering stunning ocean views and impeccable service.", "info@seasideresort.com", new Guid("9639e0fa-a591-480c-ae67-1333545ee981"), "/images/HotelOne.jpg", 400, "Seaside Resort", 150, "+3598756236514" }
                });

            migrationBuilder.InsertData(
    table: "Rooms",
    columns: new[] { "Id", "AgentId", "Description", "HotelId", "Image", "IsAvaible", "PricePerNight", "RenterId", "RoomId", "RoomNumber" },
    values: new object[,]
    {
                    { new Guid("16fd314d-0728-465a-9375-3a654d6995cf"), new Guid("3c0e049e-325d-4246-b0e8-444c45905fe5"), "", new Guid("d9de3174-3180-4f48-80c1-38c607b1ff5f"), "/rooms/room1.jpg", true, 150.00m, null, 1, 0 },
                    { new Guid("ca0fdbdf-f691-4f05-ac66-c7b31c0fbd6f"), new Guid("3c0e049e-325d-4246-b0e8-444c45905fe5"), "", new Guid("d4ffc402-d1f5-4c05-831e-e5d9b7cf2fe1"), "/rooms/room2.jpg", true, 75.00m, null, 2, 0 }
    });

            migrationBuilder.InsertData(
                    table: "Reviews",
                    columns: new[] { "Id", "Content", "HotelId", "Rating", "ReviewDate", "RoomId", "UserId" },
                    values: new object[,]
                    {
                                    { 1, "Amazing experience, highly recommend!", new Guid("0c8e143f-5847-4c00-b136-43742d07131b"), 5, new DateTime(2024, 6, 12, 22, 53, 9, 746, DateTimeKind.Local).AddTicks(8862), new Guid("16fd314d-0728-465a-9375-3a654d6995cf"), new Guid("3F94A3B6-5400-4BC0-8993-23A6CCC226DF") },
                                    { 2, "Decent place, but could be better.", new Guid("d4ffc402-d1f5-4c05-831e-e5d9b7cf2fe1"), 3, new DateTime(2024, 6, 12, 22, 53, 9, 746, DateTimeKind.Local).AddTicks(9115), new Guid("16fd314d-0728-465a-9375-3a654d6995cf"), new Guid("3F94A3B6-5400-4BC0-8993-23A6CCC226DF") }
                    });

       
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityUser");

            migrationBuilder.DeleteData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: new Guid("3c0e049e-325d-4246-b0e8-444c45905fe5"));

            migrationBuilder.DeleteData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: new Guid("9639e0fa-a591-480c-ae67-1333545ee981"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: new Guid("0c8e143f-5847-4c00-b136-43742d07131b"));

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: new Guid("d4ffc402-d1f5-4c05-831e-e5d9b7cf2fe1"));

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: new Guid("d9de3174-3180-4f48-80c1-38c607b1ff5f"));

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("16fd314d-0728-465a-9375-3a654d6995cf"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: new Guid("ca0fdbdf-f691-4f05-ac66-c7b31c0fbd6f"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RoomType",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RoomType",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
