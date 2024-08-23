using Microsoft.AspNetCore.Hosting;
using PhotoSauce.MagicScaler;

namespace Ark.Gateway.Front.Data.FileManager
{
    public class FileManager : IFileManager
    {
        private readonly string _imagePath;

        public FileManager(IConfiguration config, IWebHostEnvironment env)
        {
            _imagePath = Path.Combine(env.WebRootPath, config["ImageUrl"] ?? "img/images");
        }

        public FileStream ImageStreamPost(string image)
        {
            return new FileStream(Path.Combine(_imagePath, image), FileMode.Open, FileAccess.Read);
        }

        public FileStream ImageStreamProd(string image)
        {
            return new FileStream(Path.Combine(_imagePath, image), FileMode.Open, FileAccess.Read);
        }

        public bool RemoveImage(string image)
        {
            try
            {
                var file = Path.Combine(_imagePath, image);
                if (File.Exists(file))
                    File.Delete(file);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public string SaveImage(IFormFile image)
        {
            try
            {
                var save_path = _imagePath;
                var mime = "";
                var fileName = "";

                if (!Directory.Exists(save_path))
                {
                    Directory.CreateDirectory(save_path);
                }

                if (image != null)
                {
                    mime = Path.GetExtension(image.FileName);
                    fileName = $"img_{DateTime.Now:dd-MM-yyyy-HH-mm-ss}{mime}";
                }

                using (var fileStream = new FileStream(Path.Combine(save_path, fileName), FileMode.Create))
                {
                    MagicImageProcessor.ProcessImage(image.OpenReadStream(), fileStream, ImageOptions());
                }

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Error Uploading Image";
            }
        }

        private ProcessImageSettings ImageOptions() => new ProcessImageSettings
        {
            Width = 1200,
            Height = 800,
            ResizeMode = CropScaleMode.Max
        };
    }
}
