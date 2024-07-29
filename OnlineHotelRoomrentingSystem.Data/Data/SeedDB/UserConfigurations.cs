namespace OnlineHotelRoomrentingSystem.Data.Data.SeedDB;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
internal class UserConfigurations : IEntityTypeConfiguration<IdentityUser>
{
    public void Configure(EntityTypeBuilder<IdentityUser> builder)
    {
        var data = new SeedData();

        builder.HasData(new IdentityUser[] {data.FirstUser,data.SecondUser});
    }
}
