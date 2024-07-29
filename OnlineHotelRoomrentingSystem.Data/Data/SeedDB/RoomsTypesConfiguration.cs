namespace OnlineHotelRoomrentingSystem.Data.Data.SeedDB;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineHotelRoomrentingSystem.Models;
internal class RoomsTypesConfiguration : IEntityTypeConfiguration<RoomType>
{
    public void Configure(EntityTypeBuilder<RoomType> builder)
    {
        var data = new SeedData();

        builder.HasData(new RoomType[] {data.FirstRoomType, data.SecondRoomType});
    }
}
