using Microsoft.AspNetCore.Http;
using System.IO;

namespace Ark.Gateway.Front.Data.FileManager
{
    public interface IFileManager
    {
        FileStream ImageStreamPost(string image);
        FileStream ImageStreamProd(string image);
        string SaveImage(IFormFile image);
        bool RemoveImage(string image);
    }
}
