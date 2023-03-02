using Microsoft.AspNetCore.Mvc;

namespace RootsPrescription.FileStorage;
public interface IFileStorageService
{
    FileStream? GetFile(int id);
    FileStream? GetFile(string filepath);
}
