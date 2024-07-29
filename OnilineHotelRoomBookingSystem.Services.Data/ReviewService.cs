namespace OnilineHotelRoomBookingSystem.Services.Data;

using Microsoft.EntityFrameworkCore;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.ViewModels.Review;
using OnlineHotelRoomrentingSystem.Data.Data.Common;
using OnlineHotelRoomrentingSystem.Models;


public class ReviewService : IReviewService
{
    private readonly IRepository repository;

    public ReviewService(IRepository _repository)
    {
        repository = _repository;  
    }

    public async Task<string> CreateHotelReviewAsync(HotelReviewViewModel model , string userId)
    {
        if (!Guid.TryParse(model.HotelId, out Guid hotelGuid))
        {
            throw new ArgumentException("Invalid HotelId");
        }

        var hotelExists = await repository.AllReadOnly<Hotel>().AnyAsync(h => h.Id == hotelGuid);

        if (!hotelExists)
        {
            throw new ArgumentException("HotelId does not exist");
        }

        if (!Guid.TryParse(userId, out Guid userGuid))
        {
            throw new ArgumentException("Invalid UserId format");
        }

        Review review = new Review
        {
            Content = model.Content,
            Rating = model.Rating,
            ReviewDate = model.ReviewDate == default ? DateTime.UtcNow : model.ReviewDate,
            HotelId = hotelGuid,
            UserId = userGuid
        };

        await repository.AddAsync(review);
        await repository.SaveChangesAsync();

        return review.Id.ToString();
    }

    public async Task<string> CreateRoomReviewAsync(RoomReviewViewModel model, string userId)
    {
        if (!Guid.TryParse(model.RoomId, out Guid roomGuid))
        {
            throw new ArgumentException("Invalid RoomId");
        }

        var roomExists = await repository.AllReadOnly<Room>().AnyAsync(r => r.Id == roomGuid);
        if (!roomExists)
        {
            throw new ArgumentException("RoomId does not exist");
        }

        if (!Guid.TryParse(userId, out Guid userGuid))
        {
            throw new ArgumentException("Invalid UserId format");
        }

        Review review = new Review
        {
            Content = model.Content,
            Rating = model.Rating,
            ReviewDate = model.ReviewDate == default ? DateTime.UtcNow : model.ReviewDate,
            RoomId = roomGuid,
            UserId = userGuid
        };

        await repository.AddAsync(review);
        await repository.SaveChangesAsync();

        return review.Id.ToString();
    }

    public async Task DeleteReviewsByHotelIdAsync(string hotelId)
    {
        var reviewsToDelete = await repository
          .All<Review>()
          .Where(h => h.HotelId.HasValue && h.HotelId.ToString() == hotelId)
          .ToListAsync();

        if (reviewsToDelete.Any())
        {
            await repository.DeleteRange(reviewsToDelete);
            await repository.SaveChangesAsync();
        }
    }

    public async Task DeleteReviewsByRoomIdAsync(string roomId)
    {
        var reviewsToDelete = await repository
            .All<Review>()
            .Where(r => r.RoomId.HasValue && r.RoomId.ToString() == roomId)
            .ToListAsync();

        if (reviewsToDelete.Any())
        {
            await repository.DeleteRange(reviewsToDelete);
            await repository.SaveChangesAsync();
        }
    }
}
