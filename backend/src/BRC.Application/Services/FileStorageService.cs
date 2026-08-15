using BRC.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BRC.Application.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public FileStorageService(IConfiguration config)
    {
        _basePath = config["FileStorage:BasePath"] ?? "./uploads";
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<(string FilePath, long FileSize)> SaveFileAsync(IFormFile file, string subFolder)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty.");

        var folderPath = Path.Combine(_basePath, subFolder);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return (Path.Combine(subFolder, fileName).Replace("\\", "/"), file.Length);
    }

    public void DeleteFile(string filePath)
    {
        var fullPath = Path.Combine(_basePath, filePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}
