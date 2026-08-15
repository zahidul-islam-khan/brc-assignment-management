using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BRC.Application.Services.Interfaces;

public interface IFileStorageService
{
    Task<(string FilePath, long FileSize)> SaveFileAsync(IFormFile file, string subFolder);
    void DeleteFile(string filePath);
}
