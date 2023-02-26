namespace RootsPrescriptionWin.FileStorage;
public class FileStorageService : IFileStorageService
{
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(ILogger<FileStorageService> logger)
    {
        _logger = logger;
    }

    public string GetFile(string id)
    {
        _logger.LogInformation($"Loading file: {id}");
        return "File: " + id;
    }
}
