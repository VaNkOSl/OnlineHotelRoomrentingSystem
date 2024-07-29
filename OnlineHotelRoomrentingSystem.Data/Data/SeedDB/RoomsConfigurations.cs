namespace OnlineHotelRoomrentingSystem.Data.Data.SeedDB;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineHotelRoomrentingSystem.Models;
internal class RoomsConfigurations : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        var data = new SeedData();

        builder.HasData(new Room[] {data.FirstRoom,data.SecondRoom});
    }
}
