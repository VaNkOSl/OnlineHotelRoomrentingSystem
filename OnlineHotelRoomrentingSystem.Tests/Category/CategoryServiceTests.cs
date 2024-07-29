namespace OnlineHotelRoomrentingSystem.Tests.Category;

using Microsoft.EntityFrameworkCore;
using Moq;
using OnilineHotelRoomBookingSystem.Services.Data;
using OnlineHotelRoomrentingSystem.Data;
using OnlineHotelRoomrentingSystem.Data.Data.Common;
using OnlineHotelRoomrentingSystem.Models;

public class CategoryServiceTests
{
    [Test]
    public async Task AllCategoriesNamesAsync_ShouldReturnDistinctCategoryNames()
    {
        var options = new DbContextOptionsBuilder<HotelRoomBookingDb>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using (var context = new HotelRoomBookingDb(options))
        {
            var categories = new List<Category>
                {
                    new Category { Name = "Category 1" },
                    new Category { Name = "Category 1" },
                    new Category { Name = "Category 2" }
                };

            context.Categories.AddRange(categories);
            context.SaveChanges();

            var repository = new Repository(context);
            var categoryService = new CategoryService(repository);

            var result = await categoryService.AllCategoriesNamesAsync();

            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.IsTrue(result.Contains("Category 1"));
            Assert.IsTrue(result.Contains("Category 2"));
        }
    }

   // [Test]
   //// [Ignore("Performance test - Uncomment to run")]
   // public async Task PerformanceTest_AllCategoriesNamesAsync()
   // {
   //     const int NumCategories = 1000000; 

   //     var options = new DbContextOptionsBuilder<HotelRoomBookingDb>()
   //         .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
   //         .Options;

   //     using (var context = new HotelRoomBookingDb(options))
   //     {
   //         var categories = new List<Category>();
   //         for (int i = 1; i <= NumCategories; i++)
   //         {
   //             categories.Add(new Category { Id = i, Name = $"Category {i}" });
   //         }

   //         context.Categories.AddRange(categories);
   //         await context.SaveChangesAsync(); 

   //         var repository = new Repository(context);
   //         var categoryService = new CategoryService(repository);

   //         var result = await categoryService.AllCategoriesNamesAsync();

   //         Assert.IsNotNull(result);
   //         Assert.That(result.Count(), Is.EqualTo(NumCategories));
   //     }
   // }
}
