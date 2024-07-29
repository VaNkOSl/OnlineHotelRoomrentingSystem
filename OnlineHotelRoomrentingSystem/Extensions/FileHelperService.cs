using OnlineHotelRoomrentingSystem.Extensions.Contacts;

namespace OnlineHotelRoomrentingSystem.Extensions
{
    public class FileHelperService : IFileService
    {
        public async Task<string> UploadFileAsync(IFormFile file, IWebHostEnvironment webHostEnvironment, string folderName = "images")
        {
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, folderName);

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return "/images/" + uniqueFileName;
            }
            else
            {
                throw new ArgumentException("Please select an image file.");
            }
        }

        public async Task<string> UploadFileRoomsAsync(IFormFile file, IWebHostEnvironment webHostEnvironment, string folderName = "rooms")
        {
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, folderName);

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return "/rooms/" + uniqueFileName;
            }
            else
            {
                throw new ArgumentException("Please select an image file.");
            }
        }
    }
}
