using Microsoft.AspNetCore.Mvc;

namespace RootsPrescriptionWin.FileStorage;
public interface IFileStorageService
{
    FileStream? GetFile(int id);
    FileStream? GetFile(string filepath);
}
