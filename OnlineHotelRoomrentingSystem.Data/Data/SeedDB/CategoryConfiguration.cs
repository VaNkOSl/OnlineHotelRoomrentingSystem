namespace OnlineHotelRoomrentingSystem.Data.Data.SeedDB;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineHotelRoomrentingSystem.Models;
internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        var data = new SeedData();

        builder.HasData(new Category[] {data.FirstCategory,data.SecondCategory,data.ThirdCategory,data.FourthCategory,data.FiveCategory,data.SixCategory});
    }
}
