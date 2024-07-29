namespace OnlineHotelRoomrentingSystem.Data.Data.SeedDB;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineHotelRoomrentingSystem.Models;
internal class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        var data = new SeedData();

        builder.HasData(new Hotel[] { data.FirstHotel, data.SecondHotel, data.ThirdHotel });
    }
}
