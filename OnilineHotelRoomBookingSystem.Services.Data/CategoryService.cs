namespace OnilineHotelRoomBookingSystem.Services.Data;

using Microsoft.EntityFrameworkCore;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomrentingSystem.Data.Data.Common;
using OnlineHotelRoomrentingSystem.Models;

public class CategoryService : ICategoryService
{
    private readonly IRepository repository;

    public CategoryService(IRepository _repository)
    {
        this.repository = _repository;  
    }
    public async Task<IEnumerable<string>> AllCategoriesNamesAsync()
    {
        return await repository
            .AllReadOnly<Category>()
            .Select(c => c.Name)
            .Distinct()
            .ToListAsync();    
    }
}
