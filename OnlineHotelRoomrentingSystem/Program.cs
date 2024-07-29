namespace OnlineHotelRoomrentingSystem;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnilineHotelRoomBookingSystem.Services.Data.Contacts;
using OnlineHotelRoomBookingSystem.Web.Infrastructure.Extensions;
using OnlineHotelRoomBookingSystem.Web.Infrastructure.Middlewares;
using OnlineHotelRoomBookingSystem.Web.Infrastructure.ModelBlinders;
using OnlineHotelRoomrentingSystem.Data;
using OnlineHotelRoomrentingSystem.Extensions.Contacts;
using OnlineHotelRoomrentingSystem.Hubs;
using static OnlineHotelRoomrentingSystem.Commons.GeneralApplicationConstants;

public class Program
{
    public static async Task  Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplicationDbContext(builder.Configuration);
        builder.Services.AddApplicationIdentity(builder.Configuration);

        builder.Services.AddApplicationServices(typeof(IHotelService));


        builder.Services.AddMemoryCache();
        builder.Services.AddResponseCaching();
        builder.Services.AddSignalR();

        builder.Services.AddControllersWithViews(options =>
        {
            options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
            options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
        });

         var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseMiddleware<OnlineUsersMiddleware>();
        app.EnableOnlineUsersCheck();

        app.UseResponseCaching();

        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.SeedAdministrator(DevelopmentAdminEmail);
        }
        else
        {
            builder.Services.AddDbContext<HotelRoomBookingDb>(options =>
            {
                options.UseInMemoryDatabase("ProductionDatabase");
            });
        }


        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
             name: "areas",
             pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
         );

            endpoints.MapControllerRoute(
                name: "Hotel Details",
                pattern: "/Hotel/Details/{id}",
                defaults: new { Controller = "Hotel", Action = "Details" }
            );

            endpoints.MapHub<ChatHub>("/chatHub");

            endpoints.MapDefaultControllerRoute();
            endpoints.MapRazorPages();
        });

       await app.RunAsync();
    }
}
