namespace OnlineHotelRoomrentingSystem.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineHotelRoomrentingSystem.Data.Data.SeedDB;
using OnlineHotelRoomrentingSystem.Models;

public class HotelRoomBookingDb : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public HotelRoomBookingDb()
      : base()
    {
    }
    public HotelRoomBookingDb(DbContextOptions<HotelRoomBookingDb> options)
        : base(options)
    {
    }

    public virtual DbSet<Agent> Agents { get; set; } = null!;
    public virtual DbSet<Hotel> Hotels { get; set; } = null!;
    public virtual DbSet<Room> Rooms { get; set; } = null!;
    public virtual DbSet<Review> Reviews { get; set; } = null!;
    public virtual DbSet<Category> Categories { get; set; } = null!;
    public virtual DbSet<RoomType> RoomType { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        //builder.ApplyConfiguration(new AgentConfiguration());
        //builder.ApplyConfiguration(new UserConfigurations());
        //builder.ApplyConfiguration(new CategoryConfiguration());
        //builder.ApplyConfiguration(new HotelConfiguration());
        //builder.ApplyConfiguration(new ReviewsConfiguration());
        //builder.ApplyConfiguration(new RoomsTypesConfiguration());
        //builder.ApplyConfiguration(new RoomsConfigurations());


        base.OnModelCreating(builder);

        //builder.Entity<Hotel>()
        //    .HasOne(h => h.Category)
        //    .WithMany(c => c.Hotels)
        //    .HasForeignKey(h => h.CategoryId)
        //    .OnDelete(DeleteBehavior.Restrict);

        //builder.Entity<Hotel>()
        //    .HasOne(h => h.HotelManager)
        //    .WithMany(a => a.OwnedHotels)
        //    .HasForeignKey(h => h.HotelManagerId)
        //    .OnDelete(DeleteBehavior.Restrict);

        //builder.Entity<Review>()
        //    .HasOne(r => r.Room)
        //    .WithMany(rm => rm.Reviews)
        //    .HasForeignKey(r => r.RoomId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //builder.Entity<Review>()
        //    .HasOne(r => r.Hotel)
        //    .WithMany(h => h.Reviews)
        //    .HasForeignKey(r => r.HotelId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //builder.Entity<Review>()
        //    .HasOne(r => r.User)
        //    .WithMany(u => u.Reviews)
        //    .HasForeignKey(r => r.UserId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //builder.Entity<Room>()
        //    .HasOne(r => r.Renter)
        //    .WithMany(u => u.RentedRooms)
        //    .HasForeignKey(r => r.RenterId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //builder.Entity<Room>()
        //    .HasOne(r => r.Hotel)
        //    .WithMany(h => h.Rooms)
        //    .HasForeignKey(r => r.HotelId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //builder.Entity<Room>()
        //    .HasOne(r => r.Agent)
        //    .WithMany(a => a.OwnedRooms)
        //    .HasForeignKey(r => r.AgentId)
        //    .OnDelete(DeleteBehavior.Restrict);
    }
}
