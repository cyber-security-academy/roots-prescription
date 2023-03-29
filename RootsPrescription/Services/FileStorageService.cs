using Microsoft.AspNetCore.Mvc;
using RootsPrescription.Database;
using RootsPrescription.Models;

namespace RootsPrescription.FileStorage;
public class FileStorageService : IFileStorageService
{
    private readonly ILogger<FileStorageService> _logger;
    private readonly string _filearchivepath = "FileArchive/export";
    private readonly string _hashfilename = ".db.hash.txt";

    public FileStorageService(ILogger<FileStorageService> logger)
    {
        _logger = logger;
    }

    public FileStream? GetFile(int id)
    {
        // TODO: Look up filename for the given id
        string filename = "Prescription.pdf";
        return GetFile(filename);
    }

    public FileStream? GetFile(string filename)
    {
        string filepath= Path.Combine(_filearchivepath, filename);
        if (File.Exists(filepath) || Directory.Exists(filepath))
        {
            _logger.LogInformation($"Downloading file: {filepath}");
            return System.IO.File.OpenRead(filepath);  // Use System.IO to avoid naming confusion
        } else
        {
            _logger.LogWarning($"Cannot find file file: {filepath}");
            return null;
        }
    }



    public void InitFileStorage(IDatabaseService dbservice)
    {
        string? checksum = dbservice.GetDbHash();
        if (checksum != null) return;

        // Create Base folder
        if (!Directory.Exists(_filearchivepath))
            Directory.CreateDirectory(_filearchivepath);

        // If hashvalue does not match, rebuild links
        UserDTO[]? users = dbservice.GetAllUsers();
        if (!CheckDbHashValue(checksum) && users != null) {
            // Remove existsing links
            DeleteFileArchive(_filearchivepath);

            // Create new links
            BuildFileArchive(_filearchivepath, checksum, users);
        }
    }

    private bool CheckDbHashValue(string hashvalue)
    {
        return false;
    }

    private void BuildFileArchive(string path, string hashvalue, UserDTO[] users)
    {
        string hashpath = Path.Combine(_filearchivepath, _hashfilename);
        File.WriteAllText(hashpath, hashvalue);
    }

    private void DeleteFileArchive(string path)
    {

    }
}
