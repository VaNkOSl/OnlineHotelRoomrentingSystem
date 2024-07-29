namespace OnlineHotelRoomrentingSystem.Extensions.Contacts
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, IWebHostEnvironment webHostEnvironment, string folderName = "images");
        Task<string> UploadFileRoomsAsync(IFormFile file, IWebHostEnvironment webHostEnvironment, string folderName = "rooms");
    }
}
