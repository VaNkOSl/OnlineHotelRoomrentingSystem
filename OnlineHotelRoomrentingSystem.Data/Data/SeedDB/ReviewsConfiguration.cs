namespace OnlineHotelRoomrentingSystem.Data.Data.SeedDB;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineHotelRoomrentingSystem.Models;
internal class ReviewsConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        var data = new SeedData();

        builder.HasData(new Review[] { data.FirstReview, data.SecondReview });
    }
}
